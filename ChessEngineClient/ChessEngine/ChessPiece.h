#pragma once
#include "EngineDefines.h"

namespace ChessEngine
{
	public enum class PieceType : int
	{
		None	= ' ',
		Pawn	= 'p',
		Knight	= 'n',
		Bishop	= 'b',
		Rock	= 'r',
		Queen	= 'q',
		King	= 'k'
	};

	ref class ChessPiece sealed
	{
	public:
		ChessPiece();
		ChessPiece(PieceType type, bool bWhite);
	
		PieceType		GetType();

	private:
		ChessPieceImpl	m_Piece;
	};

}
