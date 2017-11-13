#include "pch.h"
#include "MoveScope.h"
#include "ChessBoardImpl.h"

using namespace ChessEngine;

MoveScope::MoveScope(ChessBoardImpl* pBoard, const MoveImpl& move, const AdditionalMoveInfo& moveInfo)
	: m_pBoard(pBoard)
	, m_move(move)
	, m_moveInfo(moveInfo)
{
	_ASSERTE(move);
	m_destPiece = m_pBoard->GetPiece(move.to);
	m_pBoard->SetPiece(m_pBoard->GetPiece(move.from), move.to);
	m_pBoard->SetPiece(ChessPieceImpl(), move.from);
	if (moveInfo.type == MI_EnPassant)
	{
		m_pBoard->SetPiece(ChessPieceImpl(), moveInfo.coordEnPassant);
	}
}


MoveScope::~MoveScope(void)
{
	auto piece = m_pBoard->GetPiece(m_move.to);
	m_pBoard->SetPiece(piece, m_move.from);
	m_pBoard->SetPiece(m_destPiece, m_move.to);

	if (m_moveInfo.type == MI_EnPassant)
	{
		ChessPieceImpl capturedPawn(ChessPieceImpl::Pawn, !piece.bWhite);
		m_pBoard->SetPiece(capturedPawn, m_moveInfo.coordEnPassant);
	}
}
