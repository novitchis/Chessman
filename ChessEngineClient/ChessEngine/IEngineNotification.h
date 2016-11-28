#pragma once
#include "Move.h"
#include "AnalysisData.h"

namespace ChessEngine
{
	public enum class EngineState : int { Start, Analyze, Stop };

	public interface class IEngineNotification
	{
		public:
			virtual void OnEngineMoveFinished(Move^ move, AnalysisData^ analysis ) = 0;
			virtual void OnEngineError() = 0;
			virtual void OnGameEnded(bool bWhiteWins) = 0;
			virtual void OnStateChanged(EngineState state) = 0;
	};

}
