#include "pch.h"
#include "ChessPiece.h"

using namespace ChessEngine;

ChessPiece::ChessPiece()
{
}

ChessPiece::ChessPiece(PieceType type, bool bWhite)
	: m_Piece((char)type, bWhite)
{
}

ChessPiece::ChessPiece(ChessPieceImpl pieceImpl)
	: m_Piece(pieceImpl)
{
}

ChessPieceImpl ChessPiece::getPieceImpl()
{
	return m_Piece;
}
