#pragma once

enum EResult : int
{
	Success = 0,
	Hijacked = 1
};

struct DriverObject
{
	wchar_t Name[100];
	EResult Result;
};

struct DriverObjectArray
{
	int Count;
	int Max;
	DriverObject Array[1];
};

int Scan(DriverObject* array, int max);