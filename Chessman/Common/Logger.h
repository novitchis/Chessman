#pragma once

//#include <windows.h>
#include <tchar.h>
//#include "atlstr.h"
//#undef DEBUG
//USES_CONVERSION;

#ifndef STRINGIFY
#define STRINGIFY(x)	#x
#endif
#define STRING_PRE(x) STRINGIFY(x) //this extra intermediary step is needed to expand preprocessor defines
#define AT						__FUNCTION__ " : " STRING_PRE(__LINE__) " : " __FILE__ 

#ifdef DEBUG  //defined by <atlbase.h>
    __pragma(message(__FILE__ " : WARNING : 'DEBUG' is used internally by easylogging++.h"));
    #undef DEBUG //easylogging uses this as literal
#endif

#define _ELPP_DISABLE_DEFAULT_CRASH_HANDLING
#define _ELPP_STL_LOGGING
#define _ELPP_THREAD_SAFE
#define _ELPP_UNICODE
#define _ELPP_NO_DEFAULT_LOG_FILE
//#define _ELPP_DEFAULT_LOG_FILE "BkTester.log"
#include "easylogging++.h"

void LOGGER_UNIT_TESTS();

void PrintBox(TCHAR *formstr,...);
DWORD IsLastError(TCHAR* errmsg=_T(""),bool mb=false);

void InitLogger(std::string fullpath);    

#pragma warning(disable:4018) //signed/unsigned mismatch
#pragma warning(disable:4100) //unreferenced parameter
#pragma warning(disable:4101) //unreferenced variable
#pragma warning(error  :4129) //unrecognized character escape sequence
#pragma warning(disable:4189) //unreferenced variable
#pragma warning(disable:4201) //nameless struct/union
#pragma warning(disable:4244) //conversion from '__int64' to 'int' possible loss of data
#pragma warning(disable:4245) //return signed/unsigned mismatch
#pragma warning(disable:4267) //conversion from 'size_t' to 'int', possible loss of data
#pragma warning(disable:4305) //double to float conversion
#pragma warning(disable:4309) //'return' : truncation of constant value
#pragma warning(disable:4311) //This warning detects 64-bit portability issues. For example, if code is compiled on a 64-bit platform, the value of a pointer (64 bits) will be truncated if it is assigned to an int (32 bits).
#pragma warning(disable:4312) //'type cast' : pointer truncation from '' to ''
#pragma warning(disable:4389) //signed/unsigned mismatch
#pragma warning(disable:4447) //'main' signature found without threading model.
#pragma warning(disable:4482) //nonstandard extension used: enum type used in qualified name
#pragma warning(disable:4554) //clarify operator precedence
#pragma warning(disable:4566) //character not representable in current code page
#pragma warning(error	:	4700) //variable used without been initialized
#pragma warning(1			:	4701) //potentially uninitialized local variable
#pragma warning(disable:4706) //assignment in if
#pragma warning(disable:4731) //frame pointer register 'ebp' modified by inline assembly code //VS always reserves EBP for local arguments access so it can't be used anymore in functions with local variables
#pragma warning(disable:4793) //function compiled as native
#pragma warning(disable:4800) //conversion to bool (performance warning)
#pragma warning(disable:4995) //declared deprecated
#pragma warning(disable:4996) //declared deprecated

inline void GetExePath(char strExePath[MAX_PATH])
    {
    GetModuleFileNameA( GetModuleHandle(NULL), strExePath, MAX_PATH );
    char*pos=strrchr(strExePath,'\\');
    if(pos)
        pos[0]='\0'; //cut filename
    strcat_s(strExePath,MAX_PATH,"/");
    }


inline void GetLogPath(char strLogPath[MAX_PATH],char*name="Chess")
    {
    GetExePath(strLogPath);
    strcat_s(strLogPath,MAX_PATH,name);
    strcat_s(strLogPath,MAX_PATH,".log");
    }


inline void InitLoggerExePath(char*name="Chess")
    {
    char strExePath[MAX_PATH];
    GetLogPath(strExePath,name);
    InitLogger(strExePath);
    }


inline bool OpenLog(char*name="Chess")
    {
    char strExePath[MAX_PATH];
    GetLogPath(strExePath,name);
    return 32<(int)ShellExecuteA(NULL,NULL,"notepad.exe",strExePath,NULL,SW_SHOWMAXIMIZED);
    }


inline bool DeleteLog(char*name="Chess")
    {
    char strExePath[MAX_PATH];
    char strOldPath[MAX_PATH];
    GetLogPath(strExePath,name);
    GetExePath(strOldPath);
    strcat_s(strOldPath,MAX_PATH,name);
    strcat_s(strOldPath,MAX_PATH,".old");
    ::DeleteFileA(strOldPath);
    return !!::MoveFileA(strExePath,strOldPath);
    }

