#include "ntos.h"
#include "scan.hpp"

PDEVICE_OBJECT g_pDeviceObject; 
UNICODE_STRING dev, dos; // Driver registry paths


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


