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

#include "Flat.h"

#endif // CALL_H