#pragma once
#include "ChessEngineImpl.h"
#include "IEngineNotification.h"
#include "ChessBoard.h"
#include "EngineNotificationAdaptor.h"

namespace ChessEngine
{
    public ref class Engine sealed
    {
    public:
        Engine(IEngineNotification^ pNotifications);
    
		bool Connect();
		bool Start();
		bool Stop();
		bool Analyze(ChessBoard^ board);

	private:
		std::shared_ptr<IChessEngine>		m_pEngineImpl;
		EngineNotificationAdaptor			m_NotificationAdaptor;
	};
}
