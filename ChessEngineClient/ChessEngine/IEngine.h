#pragma once
#include "ChessBoard.h"
#include "IEngineNotification.h"

namespace ChessEngine
{
	public interface class IEngine
	{
	public:
		virtual bool Start() = 0;
		virtual bool Stop() = 0;
		virtual bool Analyze(ChessBoard^ board) = 0;
		virtual bool StopAnalyzing() = 0;
	};

}
