#pragma once
#include "EngineDefines.h"

namespace ChessEngine
{
	// forward declarations //
	class ChessBoardImpl;

	class MoveScope
	{
	public:
		MoveScope(ChessBoardImpl* pBoard, const MoveImpl& move, const AdditionalMoveInfo& moveInfo);
		~MoveScope(void);

	private:
		ChessBoardImpl* m_pBoard;
		MoveImpl		m_move;
		AdditionalMoveInfo	m_moveInfo;
		ChessPieceImpl	m_destPiece;
	};
}
