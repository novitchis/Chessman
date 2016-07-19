#pragma once
#include <windows.h>

namespace Core
{
	class ThreadBase
	{
	public:
		ThreadBase(void);
		~ThreadBase(void);
		
		virtual void Run() = 0;
		void Start();
		bool Join();

	private:
		HANDLE m_hThread;
	};
}

