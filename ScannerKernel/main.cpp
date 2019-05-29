#include "ntos.h"
#include "scan.hpp"

PDEVICE_OBJECT g_pDeviceObject; 
UNICODE_STRING dev, dos; // Driver registry paths

#define IO_SCANNER_KERNEL_REQUEST CTL_CODE(FILE_DEVICE_UNKNOWN, 0x1337, METHOD_BUFFERED, FILE_SPECIAL_ACCESS)

enum EScanResult
{
	EUnknownError = 0,
	ESafe = 1,
	EDetected = 2,
	EInvalidProcess = 3,
	EUnknownIOCTL = 4
};


struct ScanRequest
{
	int m_ProcessId;
	EScanResult m_Result;
};

NTSTATUS IoControl(PDEVICE_OBJECT pDeviceObject, PIRP pIrp)
{
	NTSTATUS Status = STATUS_UNSUCCESSFUL;
	ULONG BytesIo = 0;

	PIO_STACK_LOCATION Stack = IoGetCurrentIrpStackLocation(pIrp);

	ULONG ControlCode = Stack->Parameters.DeviceIoControl.IoControlCode;
	ScanRequest* Buffer = (ScanRequest*)pIrp->AssociatedIrp.SystemBuffer;

	DbgPrint("[ScannerKernel] IoControl called\n");

	if (Buffer && Stack->Parameters.DeviceIoControl.InputBufferLength == sizeof(ScanRequest)
		&& Stack->Parameters.DeviceIoControl.OutputBufferLength == sizeof(ScanRequest))
	{
		if (ControlCode = IO_SCANNER_KERNEL_REQUEST)
		{
			/*PEPROCESS Process;
			Status = PsLookupProcessByProcessId((HANDLE)Buffer->m_ProcessId, &Process);
			if (NT_SUCCESS(Status))
			{
				Buffer->m_Result=ScanProcess(Process);
				ObDereferenceObject(Process);
			}
			else //unable to get EPROCESS from id
			{
				DbgPrint("[ScannerKernel] unable to get EPROCESS\n");
				Buffer->m_Result = EScanResult::EInvalidProcess;
			}*/
		}
		else //wrong IOCTL
		{
			DbgPrint("[ScannerKernel] Wrong IOCTL\n");
			Buffer->m_Result = EScanResult::EUnknownIOCTL;
			Status = STATUS_NOT_IMPLEMENTED;
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

NTSTATUS UnloadDriver(PDRIVER_OBJECT pDriverObject)
{
	IoDeleteSymbolicLink(&dos);
	IoDeleteDevice(pDriverObject->DeviceObject);

	DbgPrint("[ScannerKernel] Driver Unloaded\n");
	return STATUS_SUCCESS;
}


NTSTATUS DispatchNotImplemented(PDEVICE_OBJECT pDeviceObject, PIRP pIrp)
{
	pIrp->IoStatus.Status = STATUS_INVALID_DEVICE_REQUEST;
	pIrp->IoStatus.Information = 0;

	IoCompleteRequest(pIrp, IO_NO_INCREMENT);
	return STATUS_INVALID_DEVICE_REQUEST;
}



NTSTATUS DriverEntry(PDRIVER_OBJECT pDriverObject,
	PUNICODE_STRING pRegistryPath)
{
	RtlInitUnicodeString(&dev, L"\\Device\\ScannerKernel");
	RtlInitUnicodeString(&dos, L"\\DosDevices\\ScannerKernel");

	auto status = IoCreateDevice(pDriverObject, 0, &dev, FILE_DEVICE_UNKNOWN, FILE_DEVICE_SECURE_OPEN, FALSE, &g_pDeviceObject);
	if (NT_SUCCESS(status))
	{
		status = IoCreateSymbolicLink(&dos, &dev);
		if (NT_SUCCESS(status))
		{
			for (int a = 0; a <= IRP_MJ_MAXIMUM_FUNCTION; a++) {
				pDriverObject->MajorFunction[a] = DispatchNotImplemented;
			}
			pDriverObject->MajorFunction[IRP_MJ_DEVICE_CONTROL] = IoControl;
			pDriverObject->DriverUnload = (PDRIVER_UNLOAD)UnloadDriver;

			g_pDeviceObject->Flags |= DO_DIRECT_IO;
			g_pDeviceObject->Flags &= ~DO_DEVICE_INITIALIZING;

			DbgPrint("[ScannerKernel] Driver Loaded\n");

			Scan();
		}
	}
	return status;
}


