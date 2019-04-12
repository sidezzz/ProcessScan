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

NTSTATUS MjCreate(PDEVICE_OBJECT pDeviceObject, PIRP pIrp)
{
	pIrp->IoStatus.Status = STATUS_SUCCESS;
	pIrp->IoStatus.Information = 0;

	IoCompleteRequest(pIrp, IO_NO_INCREMENT);
	return STATUS_SUCCESS;
}

NTSTATUS MjClose(PDEVICE_OBJECT pDeviceObject, PIRP pIrp)
{
	pIrp->IoStatus.Status = STATUS_SUCCESS;
	pIrp->IoStatus.Information = 0;

	IoCompleteRequest(pIrp, IO_NO_INCREMENT);
	return STATUS_SUCCESS;
}



NTSTATUS DriverEntry(PDRIVER_OBJECT pDriverObject,
	PUNICODE_STRING pRegistryPath)
{
	/*RtlInitUnicodeString(&dev, L"\\Device\\ScannerKernel");
	RtlInitUnicodeString(&dos, L"\\DosDevices\\ScannerKernel");

	IoCreateDevice(pDriverObject, 0, &dev, FILE_DEVICE_UNKNOWN, FILE_DEVICE_SECURE_OPEN, FALSE, &g_pDeviceObject);
	IoCreateSymbolicLink(&dos, &dev);

	pDriverObject->MajorFunction[IRP_MJ_CREATE] = MjCreate;
	pDriverObject->MajorFunction[IRP_MJ_CLOSE] = MjClose;
	pDriverObject->MajorFunction[IRP_MJ_DEVICE_CONTROL] = IoControl;
	pDriverObject->DriverUnload = (PDRIVER_UNLOAD)UnloadDriver;

	g_pDeviceObject->Flags |= DO_DIRECT_IO;
	g_pDeviceObject->Flags &= ~DO_DEVICE_INITIALIZING;

	DbgPrint("[ScannerKernel] Driver Loaded\n");

	return STATUS_SUCCESS;*/
	return STATUS_UNSUCCESSFUL;
}


