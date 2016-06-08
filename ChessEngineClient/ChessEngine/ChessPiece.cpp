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


PieceType ChessPiece::GetType()
{
	return (PieceType)m_Piece.cPiece;
}
