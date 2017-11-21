#pragma once
#include <windows.h>

namespace Core
{
	class CriticalSection
	{
		// disable copy ctor & assignment operator //
		CriticalSection( const CriticalSection& );
		CriticalSection& operator=( const CriticalSection& );

	public:
		CriticalSection(void);
		~CriticalSection(void);
		
		void Lock();
		void Unlock();

	private:
		CRITICAL_SECTION m_lock;
	};

	class AutoLock 
	{
	public:
		AutoLock ( CriticalSection* pLock );
		~AutoLock();

	private:
		CriticalSection*	m_pLock;
	};
}

