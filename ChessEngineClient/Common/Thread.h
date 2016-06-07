#pragma once
#include <windows.h>

namespace Core
{
	class Thread
	{
	public:
		Thread(void);
		~Thread(void);
		
		virtual void Run() = 0;
		void Start();
		bool Join();

	private:
		HANDLE m_hThread;
	};
}

