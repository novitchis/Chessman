#include "pch.h"
#include "AutoLock.h"

using namespace Core;

CriticalSection::CriticalSection(void)
{
	InitializeCriticalSection( &m_lock );
}


CriticalSection::~CriticalSection(void)
{
	DeleteCriticalSection( &m_lock );
}

void CriticalSection::Lock()
{
	EnterCriticalSection ( &m_lock );
}


void CriticalSection::Unlock()
{
	LeaveCriticalSection( &m_lock );
}


AutoLock::AutoLock ( CriticalSection* pLock )
	: m_pLock ( pLock )
{
	m_pLock->Lock();
}


AutoLock::~AutoLock()
{
	m_pLock->Unlock();
}
