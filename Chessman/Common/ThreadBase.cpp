#include "pch.h"
#include "ThreadBase.h"

using namespace Core;

static DWORD WINAPI ThreadProc( LPVOID lpParam )
{
	ThreadBase* pThis = (ThreadBase*)lpParam;
	pThis->Run();
 	return 0;
}

ThreadBase::ThreadBase(void)
	: m_hThread( NULL )
{
}


ThreadBase::~ThreadBase(void)
{
	if ( m_hThread ) 
	{
		CloseHandle( m_hThread );
		m_hThread = NULL;
	}
}


void ThreadBase::Start()
{
	m_hThread = CreateThread( NULL, 0, ThreadProc, this, 0, NULL );
}


bool ThreadBase::Join()
{
	return WaitForSingleObject( m_hThread, INFINITE ) == WAIT_OBJECT_0;
}
