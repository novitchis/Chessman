#pragma once
#include "pch.h"
#include "Thread.h"
#include "IChessEngineNotifications.h"
#include <queue>

namespace ChessEngine
{
	// forward declarations //
	class IChessEngine;

	struct EngineIOData
	{
		EngineIOData();
		~EngineIOData();

		HANDLE hStdInputRd;
		HANDLE hStdInputWr;
		HANDLE hStdOutputRd;
		HANDLE hStdOutputWr;
	};

	class EngineCommunicationThread: public Core::Thread
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
		IChessEngine*				m_pEngine;
		HANDLE						m_hCommandEvent;
		volatile bool				m_bStop;
		bool						m_bErrorOccurred;
	};
}
