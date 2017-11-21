#include "pch.h"
#include "Event.h"

using namespace Core;

Event::Event()
{
	m_hEvent = CreateEvent(NULL, FALSE, FALSE, NULL);
}


Event::~Event()
{
	if (m_hEvent) {
		CloseHandle(m_hEvent);
		m_hEvent = NULL;
	}
}


void Event::raise()
{
	SetEvent(m_hEvent);
}



void Event::wait()
{
	WaitForSingleObject(m_hEvent, INFINITE);
}



