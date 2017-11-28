#pragma once
#include "IChessEngine.h"
#include "IChessEngineNotifications.h"
#include "ChessBoardImpl.h"

namespace ChessEngine
{
	class ChessEngineImpl
	{
	public:
		ChessEngineImpl( const std::wstring& strEnginePath, IChessEngineNotifications* pNotification );
		~ChessEngineImpl();

		bool Start ();
		bool Stop ();
		bool Analyze( ChessBoardImpl& board );
		void SetOptions( const EngineOptionsImpl& options );
		void EnableMoveDelay();

	private:
		std::shared_ptr<IChessEngine>	m_pEngineImpl;
		std::wstring					m_strEnginePath;
	};
}
