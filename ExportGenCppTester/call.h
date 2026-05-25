#ifndef CALL_H
#define CALL_H
#pragma once

#ifdef WIN32
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#endif

#include <string>
#include <tchar.h>

#define CALLTYPE __cdecl // Windows default
#define API_DLL extern "C" __declspec( dllimport )

#include "ExportCustom.h"

// Create and free declare
API_DLL IExportCustom* CALLTYPE ExportCustom_Create(int magic);
API_DLL void CALLTYPE ExportCustom_Free(IExportCustom* ptr);

// other functions
API_DLL void CALLTYPE ExportCustom_Start(IExportCustom* ptr);

#endif // CALL_H