#include "pch.h"
#include "MoveScope.h"
#include "ChessBoardImpl.h"

using namespace ChessEngine;

MoveScope::MoveScope(ChessBoardImpl* pBoard, const MoveImpl& move )
	: m_pBoard( pBoard )
	, m_move( move )
{
	_ASSERTE (move);
	m_destPiece = m_pBoard->GetPiece( move.to );
	m_pBoard->SetPiece( m_pBoard->GetPiece( move.from ), move.to );
	m_pBoard->SetPiece ( ChessPiece(), move.from );
}


MoveScope::~MoveScope(void)
{
	m_pBoard->SetPiece( m_pBoard->GetPiece( m_move.to ), m_move.from );
	m_pBoard->SetPiece ( m_destPiece, m_move.to );
}
