#pragma once

#include "call.h"
#include "ExportCustom.h"

// Create and free declare
API_DLL IExportCustom* CALLTYPE ExportCustom_Create(int magic);
API_DLL void CALLTYPE ExportCustom_Free(IExportCustom* ptr);

// other functions
API_DLL void CALLTYPE ExportCustom_Start(IExportCustom* ptr);