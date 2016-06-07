/* Copyright © 2000
AUTHOR      : Vlad Varna
DESCRIPTION : General purpose data types definitions
*/
#pragma once

#ifndef _MFC_VER	//use win32SDK
  //#define STRICT
  #include <windows.h>
#else	//use MFC
  #include "stdafx.h"
#endif

#define UNS unsigned
#define nat unsigned int
#define NAT unsigned int
#define SINGLE float
#define FAIL int //function return: 0=succes, P(<0)=failure level, N(>0)=special situation
#define FLAGS unsigned long //DWORD

//default numeric parameter that is passed on stack (used for vararg functions)
#if defined(_M_X64)
  #define PARAM unsigned __int64
#else
  #define PARAM unsigned int
#endif

#define CSTR const TCHAR*
#define TSTR std::basic_string<TCHAR>
#ifdef  _UNICODE
  #define TCHARC char		//pointer to TCHAR's complement
#else
  #define TCHARC WCHAR	//pointer to TCHAR's complement
#endif

#define ifn(condition)				if(!(condition))

#define COPY(d,s) CopyMemory(&d,&s,sizeof(d))

  template<typename SOURCE_TYPE>inline void CAST_COPY(void*pDestination,SOURCE_TYPE source_literal)
    {
    CopyMemory(pDestination,& source_literal,sizeof(SOURCE_TYPE));
    // return source_literal;
    }
  
  //// not safe if returned object must be freed 
  //template<typename DEST_TYPE,typename SOURCE_TYPE=int>inline DEST_TYPE CAST(SOURCE_TYPE source_literal)
  //  {
  //  return *((DEST_TYPE*)&source_literal);
  //  }

#if _MSC_VER>=1400	// >= VC2005
  typedef unsigned long long QWORD;
  typedef long long LLONG;
  //#define QWORD unsigned long long
  //#define LLONG long long
#else
  typedef unsigned __int64 QWORD;
  typedef __int64 LLONG;
  //#define QWORD unsigned __int64
  //#define LLONG __int64
#endif

  typedef struct
    {
    unsigned char bit0 : 1;
    unsigned char bit1 : 1;
    unsigned char bit2 : 1;
    unsigned char bit3 : 1;
    unsigned char bit4 : 1;
    unsigned char bit5 : 1;
    unsigned char bit6 : 1;
    unsigned char bit7 : 1;
    }BITS8;

//#define BITS8  unsigned __int8
#define BITS16 unsigned __int16
#define BITS32 unsigned __int32
#define BITS64 unsigned __int64

//Performance timer ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
class Ceas
    {
    public:
        double res;
        QWORD freq,mark,last;


        Ceas()
            {
            if(QueryPerformanceFrequency((LARGE_INTEGER*)&freq))
                res=(double)1.0/freq;
            else
                res=0.;
            mark=last=0;
            }


        void Abs()
            {
            QueryPerformanceCounter((LARGE_INTEGER*)&mark);
            }


        NAT Rel() //ticks count
            {
            QueryPerformanceCounter((LARGE_INTEGER*)&last);
            return (NAT)(last-mark);
            }


        double Sec() 
            {
            QueryPerformanceCounter((LARGE_INTEGER*)&last);
            return res*(last-mark);
            }

    };

