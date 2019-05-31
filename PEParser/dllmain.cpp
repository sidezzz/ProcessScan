// dllmain.cpp : Определяет точку входа для приложения DLL.
#include <Windows.h>
#include <iostream>


template<class T = void>
inline T* RVA(PVOID image, ULONG offset)
{
	return (T*)((char*)image + offset);
}

PIMAGE_DOS_HEADER GetDosHeader(PVOID image)
{
	auto header = (PIMAGE_DOS_HEADER)image;
	if (header && header->e_magic != IMAGE_DOS_SIGNATURE)
	{
		return nullptr;
	}
	return header;
}

PIMAGE_NT_HEADERS64 GetNtHeader(PVOID image)
{
	if (const auto dos_header = GetDosHeader(image))
	{
		auto nt_header = RVA<IMAGE_NT_HEADERS64>(image, dos_header->e_lfanew);
		if (nt_header->Signature == IMAGE_NT_SIGNATURE && nt_header->OptionalHeader.Magic == IMAGE_NT_OPTIONAL_HDR64_MAGIC)
		{
			return nt_header;
		}
	}
	return nullptr;
}

struct SECTION
{
	PVOID SectionData;
	SIZE_T SectionSize;
	PIMAGE_SECTION_HEADER Header;
};

extern "C" __declspec(dllexport) SECTION GetSectionByIndex(PVOID image, int idx)
{
	SECTION ret = { 0 };
	if (auto nt_header = GetNtHeader(image))
	{
		if (idx == -1)
		{
			idx = nt_header->FileHeader.NumberOfSections - 1;
		}
		if (idx < nt_header->FileHeader.NumberOfSections)
		{
			ret.Header = &((PIMAGE_SECTION_HEADER)(nt_header + 1))[idx];
			ret.SectionData = RVA(image, ret.Header->PointerToRawData);
			ret.SectionSize = ret.Header->SizeOfRawData;
		}
	}
	return ret;
}

extern "C" __declspec(dllexport) DWORD GetEntryRawOffset(PVOID image)
{
	DWORD ret = 0;
	if (auto nt_header = GetNtHeader(image))
	{
		auto entry_offset = nt_header->OptionalHeader.AddressOfEntryPoint;
		int i = 0;
		auto section = GetSectionByIndex(image, i);
		while (section.SectionData)
		{
			if (entry_offset > section.Header->VirtualAddress &&
				entry_offset < section.Header->VirtualAddress + section.Header->SizeOfRawData)
			{
				ret = (entry_offset - section.Header->VirtualAddress + section.Header->PointerToRawData);
				break;
			}
			i++;
			section = GetSectionByIndex(image, i);
		}
	}
	return ret;
}


struct DriverObject
{
	wchar_t Name[100];
	int Result;
};

struct DriverArray
{
	int Count;
	DriverObject Array[3];
};

extern "C" __declspec(dllexport) void TestFunc(DriverObject* object_array, int count)
{
	char buf[123];
	sprintf_s(buf, "%p", object_array);
	MessageBoxA(0, buf, buf, 0);
	for (int a = 0; a < count; a++)
	{
		sizeof(DriverArray);
		memcpy(object_array[a].Name, L"suka", sizeof(L"suka"));
		object_array[a].Result = a;
	}
}


BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

