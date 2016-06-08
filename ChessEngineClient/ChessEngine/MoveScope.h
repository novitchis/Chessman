#pragma once
#include "EngineDefines.h"

namespace ChessEngine
{
	// forward declarations //
	class ChessBoardImpl;

	class MoveScope
	{
	public:
		MoveScope( ChessBoardImpl* pBoard, const MoveImpl& move );
		~MoveScope(void);

	private:
		ChessBoardImpl* m_pBoard;
		MoveImpl		m_move;
		ChessPieceImpl	m_destPiece;
	};
}
