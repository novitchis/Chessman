#pragma once
#include "ChessEngineImpl.h"
#include "IEngineNotification.h"
#include "ChessBoard.h"
#include "EngineNotificationAdaptor.h"
#include "IEngine.h"
#include "EngineOptions.h"

namespace ChessEngine
{
	public ref class Engine sealed : IEngine
    {
    public:
        Engine(IEngineNotification^ pNotifications);
    
		bool Connect();

		virtual bool Start();
		virtual bool Stop();
		virtual bool Analyze(ChessBoard^ board, int depth, int moveTime);

		virtual bool StopAnalyzing();
		virtual void SetOptions(EngineOptions^ options);

	private:
		std::shared_ptr<IChessEngine>		m_pEngineImpl;
		EngineNotificationAdaptor			m_NotificationAdaptor;
	};
}
