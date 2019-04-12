#pragma once
#include "ntos.h"

#define IO_SCANNER_KERNEL_REQUEST CTL_CODE(FILE_DEVICE_UNKNOWN, 0x1337, METHOD_BUFFERED, FILE_SPECIAL_ACCESS)

enum EScanResult
{
	EUnknownError=0,
	ESafe=1,
	EDetected=2,
	EInvalidProcess=3,
	EUnknownIOCTL=4
};


struct ScanRequest
{
	int m_ProcessId;
	EScanResult m_Result;
};

NTSTATUS IoControl(PDEVICE_OBJECT pDeviceObject, PIRP pIrp);