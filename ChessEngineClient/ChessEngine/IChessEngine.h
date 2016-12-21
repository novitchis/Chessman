#pragma once
#include "IChessEngineNotifications.h"
#include "ChessBoardImpl.h"
namespace ChessEngine
{
	class IChessEngine
	{
	public:
		virtual void SetNotification( IChessEngineNotifications* pNotification ) = 0;
		virtual bool ConnectTo( const std::wstring& strEnginePath ) = 0;
		virtual bool Start() = 0;
		virtual bool Stop() = 0;
		virtual bool Analyze(ChessBoardImpl& board) = 0;
		virtual bool StopAnalyzing() = 0;
		virtual void SetOptions( const ChessEngineOptions& options ) = 0;
		virtual void EnableMoveDelay() = 0;

		virtual void OnEngineResponse( const std::string& strResponse ) = 0;
	};
}