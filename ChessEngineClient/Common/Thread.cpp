#include "pch.h"
#include "Thread.h"

using namespace Core;

static DWORD WINAPI ThreadProc( LPVOID lpParam )
{
	Thread* pThis = (Thread*)lpParam;
	pThis->Run();
	return 0;
}

Thread::Thread(void)
	: m_hThread( NULL )
{
}


Thread::~Thread(void)
{
	if ( m_hThread ) 
	{
		CloseHandle( m_hThread );
		m_hThread = NULL;
	}
}


void Thread::Start()
{
	m_hThread = CreateThread( NULL, 0, ThreadProc, this, 0, NULL );
}


bool Thread::Join()
{
	return WaitForSingleObject( m_hThread, INFINITE ) == WAIT_OBJECT_0;
}
