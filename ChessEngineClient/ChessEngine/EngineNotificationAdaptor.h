#pragma once
#include "IChessEngineNotifications.h"
#include "IEngineNotification.h"
#include "EngineDefines.h"

namespace ChessEngine
{
	class EngineNotificationAdaptor: public IChessEngineNotifications
	{
	public:
		EngineNotificationAdaptor(IEngineNotification^ pManagedNofitication);
		virtual void OnEngineMoveFinished(const std::vector<AnalysisDataImpl>& analysis);
		virtual void OnEngineError();
		virtual void OnGameEnded(bool bWhiteWins);

	private:
		IEngineNotification^	m_pManagedNotification;
	};

}
