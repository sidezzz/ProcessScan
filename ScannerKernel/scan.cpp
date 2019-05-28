#include <intrin.h>
#include "scan.hpp"
#include <ntimage.h>

typedef struct _RTL_PROCESS_MODULE_INFORMATION
{
	HANDLE Section;
	PVOID MappedBase;
	PVOID ImageBase;
	ULONG ImageSize;
	ULONG Flags;
	USHORT LoadOrderIndex;
	USHORT InitOrderIndex;
	USHORT LoadCount;
	USHORT OffsetToFileName;
	UCHAR  FullPathName[256];
} RTL_PROCESS_MODULE_INFORMATION, * PRTL_PROCESS_MODULE_INFORMATION;

typedef struct _RTL_PROCESS_MODULES
{
	ULONG NumberOfModules;
	RTL_PROCESS_MODULE_INFORMATION Modules[1];
} RTL_PROCESS_MODULES, * PRTL_PROCESS_MODULES;

typedef enum _SYSTEM_INFORMATION_CLASS {
	SystemBasicInformation,
	SystemProcessorInformation,
	SystemPerformanceInformation,
	SystemTimeOfDayInformation,
	SystemPathInformation,
	SystemProcessInformation,
	SystemCallCountInformation,
	SystemDeviceInformation,
	SystemProcessorPerformanceInformation,
	SystemFlagsInformation,
	SystemCallTimeInformation,
	SystemModuleInformation,
	SystemLocksInformation,
	SystemStackTraceInformation,
	SystemPagedPoolInformation,
	SystemNonPagedPoolInformation,
	SystemHandleInformation,
	SystemObjectInformation,
	SystemPageFileInformation,
	SystemVdmInstemulInformation,
	SystemVdmBopInformation,
	SystemFileCacheInformation,
	SystemPoolTagInformation,
	SystemInterruptInformation,
	SystemDpcBehaviorInformation,
	SystemFullMemoryInformation,
	SystemLoadGdiDriverInformation,
	SystemUnloadGdiDriverInformation,
	SystemTimeAdjustmentInformation,
	SystemSummaryMemoryInformation,
	SystemNextEventIdInformation,
	SystemEventIdsInformation,
	SystemCrashDumpInformation,
	SystemExceptionInformation,
	SystemCrashDumpStateInformation,
	SystemKernelDebuggerInformation,
	SystemContextSwitchInformation,
	SystemRegistryQuotaInformation,
	SystemExtendServiceTableInformation,
	SystemPrioritySeperation,
	SystemPlugPlayBusInformation,
	SystemDockInformation,
	SystemPowerInformation1,
	SystemProcessorSpeedInformation,
	SystemCurrentTimeZoneInformation,
	SystemLookasideInformation


} SYSTEM_INFORMATION_CLASS, * PSYSTEM_INFORMATION_CLASS;


// if MaintainTypeList is 1 TypeList contains the entries. Sadly it's not maintained for IoDriverObjectType
typedef struct _OBJECT_TYPE_INITIALIZER
{
	USHORT Length;
	UCHAR ObjectTypeFlags;
	ULONG CaseInsensitive : 1;
	ULONG UnnamedObjectsOnly : 1;
	ULONG UseDefaultObject : 1;
	ULONG SecurityRequired : 1;
	ULONG MaintainHandleCount : 1;
	ULONG MaintainTypeList : 1;
	ULONG ObjectTypeCode;
	ULONG InvalidAttributes;
	GENERIC_MAPPING GenericMapping;
	ULONG ValidAccessMask;
	POOL_TYPE PoolType;
	ULONG DefaultPagedPoolCharge;
	ULONG DefaultNonPagedPoolCharge;
	PVOID DumpProcedure;
	LONG* OpenProcedure;
	PVOID CloseProcedure;
	PVOID DeleteProcedure;
	LONG* ParseProcedure;
	LONG* SecurityProcedure;
	LONG* QueryNameProcedure;
	UCHAR* OkayToCloseProcedure;
} OBJECT_TYPE_INITIALIZER, * POBJECT_TYPE_INITIALIZER;

// OBJECT_TYPE is an OBJECT of Type TypeObject
typedef struct _OBJECT_TYPE
{
	// ERESOURCE Mutex; -> not in WinDbg probably negative offset or removed
	LIST_ENTRY TypeList;
	UNICODE_STRING Name;
	PVOID DefaultObject;
	UCHAR Index;
	ULONG TotalNumberOfObjects;
	ULONG TotalNumberOfHandles;
	ULONG HighWaterNumberOfObjects;
	ULONG HighWaterNumberOfHandles;
	OBJECT_TYPE_INITIALIZER TypeInfo;
	EX_PUSH_LOCK TypeLock;
	ULONG Key;
	LIST_ENTRY CallbackList;
} OBJECT_TYPE, * POBJECT_TYPE;

typedef struct _DEVICE_MAP* PDEVICE_MAP;

typedef struct _OBJECT_DIRECTORY_ENTRY
{
	_OBJECT_DIRECTORY_ENTRY* ChainLink;
	PVOID Object;
	ULONG HashValue;
} OBJECT_DIRECTORY_ENTRY, * POBJECT_DIRECTORY_ENTRY;

typedef struct _OBJECT_DIRECTORY
{
	POBJECT_DIRECTORY_ENTRY HashBuckets[37];
	EX_PUSH_LOCK Lock;
	PDEVICE_MAP DeviceMap;
	ULONG SessionId;
	PVOID NamespaceEntry;
	ULONG Flags;
} OBJECT_DIRECTORY, * POBJECT_DIRECTORY;

typedef struct _KLDR_DATA_TABLE_ENTRY
{
	LIST_ENTRY InLoadOrderLinks;
	PVOID ExceptionTable;
	ULONG ExceptionTableSize;
	// ULONG padding on IA64
	PVOID GpValue;
	PNON_PAGED_DEBUG_INFO NonPagedDebugInfo;
	PVOID DllBase;
	PVOID EntryPoint;
	ULONG SizeOfImage;
	UNICODE_STRING FullDllName;
	UNICODE_STRING BaseDllName;
	ULONG Flags;
	USHORT LoadCount;
	USHORT __Unused5;
	PVOID SectionPointer;
	ULONG CheckSum;
	// ULONG padding on IA64
	PVOID LoadedImports;
	PVOID PatchInformation;
} KLDR_DATA_TABLE_ENTRY, * PKLDR_DATA_TABLE_ENTRY;

EXTERN_C PLIST_ENTRY NTKERNELAPI PsLoadedModuleList;

#define POOL_TAG 'SCAN'

void* AllocatePool(SIZE_T size)
{
	return ExAllocatePoolWithTag(NonPagedPool, size, POOL_TAG);
}

void FreePool(void* pool)
{
	if (pool)
	{
		ExFreePoolWithTag(pool, POOL_TAG);
	}
}


NTSTATUS IoControl(PDEVICE_OBJECT pDeviceObject, PIRP pIrp)
{
	NTSTATUS Status = STATUS_UNSUCCESSFUL;
	ULONG BytesIo = 0;

	PIO_STACK_LOCATION Stack = IoGetCurrentIrpStackLocation(pIrp);

	ULONG ControlCode = Stack->Parameters.DeviceIoControl.IoControlCode;
	ScanRequest* Buffer= (ScanRequest*)pIrp->AssociatedIrp.SystemBuffer;
	
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

PRTL_PROCESS_MODULES GetKernelModules()
{
	ULONG bytes = 0;
	NTSTATUS status = ZwQuerySystemInformation(SystemModuleInformation, 0, bytes, &bytes);

	if (!bytes)
	{
		DbgPrint("[ScannerKernel] %s: first NtQuerySystemInformation failed, status: 0x%x", __FUNCTION__, status);
		return nullptr;
	}

	PRTL_PROCESS_MODULES modules = (PRTL_PROCESS_MODULES)AllocatePool(bytes);

	status = ZwQuerySystemInformation(SystemModuleInformation, modules, bytes, &bytes);

	if (!NT_SUCCESS(status))
	{
		FreePool(modules);
		DbgPrint("[ScannerKernel] %s: second NtQuerySystemInformation failed, status: 0x%x", __FUNCTION__, status);
		return nullptr;
	}

	return modules;
}

bool IsInValidImage(PRTL_PROCESS_MODULES modules, void* address)
{
	if (!address)
		return 1;
	for (int i = 0; i < modules->NumberOfModules; i++)
	{
		auto& mdl = modules->Modules[i];
		if (address > mdl.ImageBase && address < (void*)((UINT64)mdl.ImageBase + mdl.ImageSize))
		{
			return 1;
		}
	}
	return 0;
}

void Scan()
{
	HANDLE handle;
	OBJECT_ATTRIBUTES attributes;
	UNICODE_STRING path;
	PVOID directory;
	RtlInitUnicodeString(&path, L"\\Driver");
	InitializeObjectAttributes(&attributes, &path, OBJ_KERNEL_HANDLE | OBJ_CASE_INSENSITIVE, 0, 0);
	auto status = ZwOpenDirectoryObject(&handle, DIRECTORY_ALL_ACCESS, &attributes);
	if (NT_SUCCESS(status))
	{
		status = ObReferenceObjectByHandle(handle, DIRECTORY_QUERY, 0, KernelMode, &directory, 0);
		if (NT_SUCCESS(status))
		{
			if (auto modules = GetKernelModules()) //Getting list of kernel modules
			{
				const auto directory_object = POBJECT_DIRECTORY(directory);
				ExAcquirePushLockExclusiveEx(&directory_object->Lock, 0); //Locking the directory before iterating

				__try
				{
					for (auto entry : directory_object->HashBuckets)
					{
						if (entry)
						{
							while (entry != nullptr && entry->Object != nullptr)
							{
								auto driver = PDRIVER_OBJECT(entry->Object);

								for (int a = 0; a <= IRP_MJ_MAXIMUM_FUNCTION; a++)
								{
									if (!IsInValidImage(modules, driver->MajorFunction[a]))
									{
										DbgPrint("[ScannerKernel] found hijacked driver %wZ, function %d, %p\n", driver->DriverName, a, driver->MajorFunction[a]);
									}
									else
									{
										DbgPrint("[ScannerKernel] driver %wZ, valid function %d\n", driver->DriverName, a);
									}
								}

								entry = entry->ChainLink;
							}
						}

					}
				}
				__except (EXCEPTION_EXECUTE_HANDLER)
				{
					DbgPrint("[ScannerKernel] %s: exception 0x%X\n", __FUNCTION__, GetExceptionCode());
				}

				ExReleasePushLockExclusiveEx(&directory_object->Lock, 0); //Releasing the lock

				FreePool(modules); //Releasing memory allocated for modules
			}
			else
			{
				DbgPrint("[ScannerKernel] %s: GetKernelModules failed\n", __FUNCTION__);
			}


			ObDereferenceObject(directory);
		}
		else
		{
			DbgPrint("[ScannerKernel] %s: ObReferenceObjectByHandle failed, status 0x%X\n", __FUNCTION__, status);
		}
		ZwClose(handle);
	}
	else
	{
		DbgPrint("[ScannerKernel] %s: ZwOpenDirectoryObject failed, status 0x%X\n", __FUNCTION__, status);
	}
}