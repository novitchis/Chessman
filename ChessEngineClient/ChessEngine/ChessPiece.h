#pragma once
#include "EngineDefines.h"

namespace ChessEngine
{
	public enum class PieceType : int
	{
		None   = ' ',
		Pawn   = 'p',
		Knight = 'n',
		Bishop = 'b',
		Rook   = 'r',
		Queen  = 'q',
		King   = 'k'
	};

	public enum class PieceColor
	{
		Black,
		White
	};

	public ref class ChessPiece sealed
	{
	public:
		ChessPiece();
		ChessPiece(PieceType type, bool bWhite);

	internal:
		ChessPiece(ChessPieceImpl pieceImpl);

	public:
		property PieceType Type
		{
			PieceType get() { return (PieceType)m_Piece.cPiece; }
		}

		property PieceColor Color
		{
			PieceColor get() { return m_Piece.bWhite ? PieceColor::White : PieceColor::Black; }
		}
	internal:
		ChessPieceImpl getPieceImpl();

	private:
		ChessPieceImpl	m_Piece;
	};	
}
