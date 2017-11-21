#pragma once
#include "ThreadBase.h"
#include "IChessEngine.h"

namespace ChessEngine
{
	// forward declarations //
	class IChessEngine;

	struct EngineIOData
	{
		EngineIOData() : hStdInputRd(NULL)
			, hStdInputWr(NULL)
			, hStdOutputRd(NULL)
			, hStdOutputWr(NULL)
		{
		}

		~EngineIOData() {
			SafeCloseHandle(hStdInputRd);
			SafeCloseHandle(hStdInputWr);
			SafeCloseHandle(hStdOutputRd);
			SafeCloseHandle(hStdOutputWr);
		}


		HANDLE hStdInputRd;
		HANDLE hStdInputWr;
		HANDLE hStdOutputRd;
		HANDLE hStdOutputWr;
	};


	class IEngineCommThread: public Core::ThreadBase
	{
	public:
		IEngineCommThread(IChessEngine* pEngine)
			: m_pEngine(pEngine)
		{}
		virtual ~IEngineCommThread() {}

		virtual void	Run() = 0;
		virtual void	Stop() = 0;
		virtual void	SetIOData(EngineIOData* pIOData) = 0;
		virtual void	QueueCommand(const std::string& strCommand) = 0;
		virtual bool	HasErrors() = 0;

	protected:
		IChessEngine*				m_pEngine;
	};
}
