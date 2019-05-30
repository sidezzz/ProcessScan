#include "ntos.h"
#include "scan.hpp"

PDEVICE_OBJECT g_pDeviceObject; 
UNICODE_STRING dev, dos; // Driver registry paths

#define IO_SCANNER_KERNEL_REQUEST CTL_CODE(FILE_DEVICE_UNKNOWN, 0x1337, METHOD_BUFFERED, FILE_SPECIAL_ACCESS)

NTSTATUS IoControl(PDEVICE_OBJECT pDeviceObject, PIRP pIrp)
{
	NTSTATUS status = STATUS_UNSUCCESSFUL;
	ULONG bytes_io = 0;

	PIO_STACK_LOCATION stack = IoGetCurrentIrpStackLocation(pIrp);

	ULONG control_code = stack->Parameters.DeviceIoControl.IoControlCode;
	if (control_code = IO_SCANNER_KERNEL_REQUEST)
	{
		DriverObjectArray* buffer = (DriverObjectArray*)pIrp->AssociatedIrp.SystemBuffer;
		auto input_len = stack->Parameters.DeviceIoControl.InputBufferLength;
		if (input_len >= sizeof(DriverObjectArray) && input_len >= (buffer->Max * sizeof(DriverObject) + 8))
		{
			DbgPrint("[ScannerKernel] %s: buffer_size %d, max objects %d\n", __FUNCTION__, input_len, buffer->Max);
			buffer->Count = Scan(buffer->Array, buffer->Max);
			bytes_io = input_len;
			status = STATUS_SUCCESS;
		}
		else
		{
			DbgPrint("[ScannerKernel] %s: buffer size mismatch, size %d\n", __FUNCTION__, input_len);
		}
	}
	else //wrong IOCTL
	{
		DbgPrint("[ScannerKernel] %s: Wrong IOCTL\n", __FUNCTION__);
		status = STATUS_NOT_IMPLEMENTED;
	}

	pIrp->IoStatus.Status = status;
	pIrp->IoStatus.Information = bytes_io;
	IoCompleteRequest(pIrp, IO_NO_INCREMENT);

	return status;
}

NTSTATUS IoRead(PDEVICE_OBJECT pDeviceObject, PIRP pIrp)
{
	NTSTATUS status = STATUS_UNSUCCESSFUL;
	ULONG bytes_io = 0;


	auto buffer = (DriverObject*)MmGetSystemAddressForMdlSafe(pIrp->MdlAddress, LowPagePriority);
	if (buffer)
	{
		auto output_size = MmGetMdlByteCount(pIrp->MdlAddress);
		if (auto max_objects = output_size / sizeof(DriverObject))
		{
			bytes_io = Scan(buffer, max_objects) * sizeof(DriverObject);
			status = STATUS_SUCCESS;
			DbgPrint("[ScannerKernel] %s: successfully read %d bytes\n", __FUNCTION__, bytes_io);
		}
		else
		{
			DbgPrint("[ScannerKernel] %s: too small buffer %d\n", __FUNCTION__, output_size);
			status = STATUS_BUFFER_TOO_SMALL;
		}
	}
	else
	{
		DbgPrint("[ScannerKernel] %s: buffer nullptr\n", __FUNCTION__);
	}

	pIrp->IoStatus.Status = status;
	pIrp->IoStatus.Information = bytes_io;
	IoCompleteRequest(pIrp, IO_NO_INCREMENT);

	return status;
}

NTSTATUS UnloadDriver(PDRIVER_OBJECT pDriverObject)
{
	IoDeleteSymbolicLink(&dos);
	IoDeleteDevice(pDriverObject->DeviceObject);

	DbgPrint("[ScannerKernel] Driver Unloaded\n");
	return STATUS_SUCCESS;
}


NTSTATUS MjSuccess(PDEVICE_OBJECT pDeviceObject, PIRP pIrp)
{
	pIrp->IoStatus.Status = STATUS_SUCCESS;
	pIrp->IoStatus.Information = 0;

	IoCompleteRequest(pIrp, IO_NO_INCREMENT);
	return STATUS_SUCCESS;
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
			pDriverObject->MajorFunction[IRP_MJ_CREATE] = MjSuccess;
			pDriverObject->MajorFunction[IRP_MJ_CLOSE] = MjSuccess;
			pDriverObject->MajorFunction[IRP_MJ_READ] = IoRead;
			pDriverObject->MajorFunction[IRP_MJ_DEVICE_CONTROL] = IoControl;
			pDriverObject->DriverUnload = (PDRIVER_UNLOAD)UnloadDriver;

			g_pDeviceObject->Flags |= DO_DIRECT_IO;
			//g_pDeviceObject->Flags &= ~DO_DEVICE_INITIALIZING;

			DbgPrint("[ScannerKernel] Driver Loaded\n");
		}
		else
		{
			IoDeleteDevice(g_pDeviceObject);
			DbgPrint("[ScannerKernel] %s: IoCreateSymbolicLink failed, status 0x%X\n", __FUNCTION__, status);
		}
	}
	else
	{
		DbgPrint("[ScannerKernel] %s: IoCreateDevice failed, status 0x%X\n", __FUNCTION__, status);
	}

	return status;
}


