#pragma once
#include "ChessBoard.h"
#include "IEngineNotification.h"
#include "EngineOptions.h"

namespace ChessEngine
{
	public interface class IEngine
	{
	public:
		virtual bool Start() = 0;
		virtual bool Stop() = 0;
		virtual bool Analyze(ChessBoard^ board, int secondsLeft) = 0;
		virtual bool StopAnalyzing() = 0;
		virtual void SetOptions(EngineOptions^ options) = 0;
	};
}
