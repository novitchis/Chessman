#pragma once
#include "ChessEngineImpl.h"
#include "IEngineNotification.h"
#include "ChessBoard.h"
#include "EngineNotificationAdaptor.h"
#include "IEngine.h"

namespace ChessEngine
{
	public ref class Engine sealed : IEngine
    {
    public:
        Engine(IEngineNotification^ pNotifications);
    
		bool Connect();

		virtual bool Start();
		virtual bool Stop();
		virtual bool Analyze(ChessBoard^ board);
		virtual bool StopAnalyzing();

	private:
		std::shared_ptr<IChessEngine>		m_pEngineImpl;
		EngineNotificationAdaptor			m_NotificationAdaptor;
	};
}
