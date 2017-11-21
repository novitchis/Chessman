#pragma once
#include "EngineDefines.h"

namespace ChessEngine
{
	class IChessEngineNotifications
	{
	public:
		virtual void OnEngineMoveFinished(const std::vector<AnalysisDataImpl>& analysis) = 0;
		virtual void OnEngineError() = 0;
		virtual void OnGameEnded( bool bWhiteWins ) = 0;
	};
}