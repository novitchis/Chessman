#pragma once
#include "pch.h"
#include "ThreadBase.h"
#include "IChessEngineNotifications.h"
#include "IEngineCommThread.h"
#include "ChessEngineEntryPoint.h"
#include "MemoryStream.h"
#include "AutoLock.h"
#include <queue>

namespace ChessEngine
{
	class InProcEngineCommThread : public IEngineCommThread
	{
	public:
		InProcEngineCommThread( IChessEngine* pEngine );
		~InProcEngineCommThread();

		virtual void	Run();
		virtual void	Stop();
		virtual void	SetIOData( EngineIOData* pIOData );
		virtual void	QueueCommand( const std::string& strCommand );
		virtual bool	HasErrors();
	
	private:
		bool			WriteData( const std::string& strData );
		bool			ReadData( std::string& strData );
		bool			GetAvailableBytes( int& nBytes );
		bool			PushCommand();

	private:
		ChessEngineEntryPoint			m_StockfishEntryPoint;
		std::shared_ptr<MemoryStream>	m_stmInput;
		std::shared_ptr<MemoryStream>	m_stmOutput;
		HANDLE							m_hCommandEvent;
		volatile bool					m_bStop;
		bool							m_bErrorOccurred;
		Core::CriticalSection			m_csCommands;
		std::queue<std::string>			m_queueCommands;
		
	};
}
