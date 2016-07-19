#pragma once
#include "pch.h"
#include "ThreadBase.h"
#include "IChessEngineNotifications.h"
#include "IEngineCommThread.h"
#include <queue>

namespace ChessEngine
{
	class EngineCommunicationThread: public IEngineCommThread
	{
	public:
		EngineCommunicationThread( IChessEngine* pEngine );
		~EngineCommunicationThread();

		virtual void	Run();
		void			Stop();
		void			SetIOData( EngineIOData* pIOData );
		void			QueueCommand( const std::string& strCommand );
		bool			HasErrors();
	
	private:
		bool			WriteData( const std::string& strData );
		bool			ReadData( std::string& strData );
		bool			GetAvailableBytes ( int& nBytes );

	private:
		EngineIOData*				m_pIOData;// someone else has ownership over this //
		HANDLE						m_hCommandEvent;
		volatile bool				m_bStop;
		bool						m_bErrorOccurred;
	};
}
