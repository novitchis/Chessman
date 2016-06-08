#pragma once
#include "ManagedDefines.h"
#include "Move.h"
namespace ChessEngine
{
	public interface class IEngineNotification
	{
		public:
			virtual void OnEngineMoveFinished(Move^ move) = 0;
			virtual void OnEngineError() = 0;
			virtual void OnGameEnded(bool bWhiteWins) = 0;
	};

}
