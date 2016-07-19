#pragma once
#include <windows.h>

namespace Core
{
	class Event 
	{
	public:
		Event();
		~Event();

		void raise();
		void wait();
	private:
		HANDLE m_hEvent;
	};
}

