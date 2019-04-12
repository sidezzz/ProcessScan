#include <intrin.h>
#include "scan.hpp"

#define PFN_TO_PAGE(pfn) ( pfn << 12 )
#pragma pack(push, 1)
typedef union CR3_
{
	UINT64 value;
	struct
	{
		UINT64 ignored_1 : 3;
		UINT64 write_through : 1;
		UINT64 cache_disable : 1;
		UINT64 ignored_2 : 7;
		UINT64 pml4_p : 40;
		UINT64 reserved : 12;
	};
} PTE_CR3;

typedef union VIRT_ADDR_
{
	UINT64 value;
	void *pointer;
	struct
	{
		UINT64 offset : 12;
		UINT64 pt_index : 9;
		UINT64 pd_index : 9;
		UINT64 pdpt_index : 9;
		UINT64 pml4_index : 9;
		UINT64 reserved : 16;
	};
} VIRT_ADDR;

typedef UINT64 PHYS_ADDR;

typedef union PML4E_
{
	UINT64 value;
	struct
	{
		UINT64 present : 1;
		UINT64 rw : 1;
		UINT64 user : 1;
		UINT64 write_through : 1;
		UINT64 cache_disable : 1;
		UINT64 accessed : 1;
		UINT64 ignored_1 : 1;
		UINT64 reserved_1 : 1;
		UINT64 ignored_2 : 4;
		UINT64 pdpt_p : 40;
		UINT64 ignored_3 : 11;
		UINT64 xd : 1;
	};
} PML4E;

#pragma pack(pop)

EScanResult ScanProcess(PEPROCESS Process)
{
	EScanResult ret = EScanResult::ESafe;

	PTE_CR3 Cr3;
	KAPC_STATE state;
	KeStackAttachProcess(Process, &state);
	Cr3.value = __readcr3(); //aquirinig cr3 value from process
	KeUnstackDetachProcess(&state);

	MM_COPY_ADDRESS adr;
	for (int i = 256; i < 512; i++) //scanning kernel pml4's, from 0x8000 to 0xffff
	{
		PML4E Pml4e = { 0 };
		adr.PhysicalAddress.QuadPart = PFN_TO_PAGE(Cr3.pml4_p) + sizeof(PML4E) * i;

		SIZE_T BytesRead;
		if (NT_SUCCESS(MmCopyMemory(&Pml4e, adr, 8, MM_COPY_MEMORY_PHYSICAL, &BytesRead)))
		{
			if (Pml4e.user)
			{
				ret = EScanResult::EDetected;
			}
		}
	}

	DbgPrint("[ScannerKernel] Scan result %d\n",ret);

	return ret;
}

NTSTATUS IoControl(PDEVICE_OBJECT pDeviceObject, PIRP pIrp)
{
	NTSTATUS Status = STATUS_UNSUCCESSFUL;
	ULONG BytesIo = 0;

	PIO_STACK_LOCATION Stack = IoGetCurrentIrpStackLocation(pIrp);

	ULONG ControlCode = Stack->Parameters.DeviceIoControl.IoControlCode;
	ScanRequest* Buffer= (ScanRequest*)pIrp->AssociatedIrp.SystemBuffer;
	
	DbgPrint("[ScannerKernel] IoControl called\n");

	if (Buffer && Stack->Parameters.DeviceIoControl.InputBufferLength==sizeof(ScanRequest)
		&& Stack->Parameters.DeviceIoControl.OutputBufferLength==sizeof(ScanRequest))
	{
		if (ControlCode = IO_SCANNER_KERNEL_REQUEST)
		{
			PEPROCESS Process;
			Status=PsLookupProcessByProcessId((HANDLE)Buffer->m_ProcessId, &Process);
			if (NT_SUCCESS(Status))
			{
				Buffer->m_Result=ScanProcess(Process);
				ObDereferenceObject(Process);
			}
			else //unable to get EPROCESS from id
			{
				DbgPrint("[ScannerKernel] unable to get EPROCESS\n");
				Buffer->m_Result = EScanResult::EInvalidProcess;
			}
		}
		else //wrong IOCTL
		{
			DbgPrint("[ScannerKernel] Wrong IOCTL\n");
			Buffer->m_Result = EScanResult::EUnknownIOCTL;
			Status = STATUS_INVALID_PARAMETER;
		}
		BytesIo = sizeof(ScanRequest);
	}
	else //smth wrong with IO buffer
	{
		DbgPrint("[ScannerKernel] Error with buffer\n");
		Status = STATUS_INVALID_PARAMETER;
	}


	pIrp->IoStatus.Status = Status;
	pIrp->IoStatus.Information = BytesIo;
	IoCompleteRequest(pIrp, IO_NO_INCREMENT);

	return Status;
}