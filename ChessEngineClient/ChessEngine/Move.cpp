#include "pch.h"
#include "Move.h"
#include "ManagedConverter.h"

using namespace ChessEngine;

Move::Move(MoveImpl moveImpl)
	: m_Move(moveImpl)
{
}

Move::Move()
{
}

Move::Move(Coordinate^ from, Coordinate^ to)
{
	m_Move.from = CoordinateImpl(from->Y, from->X);
	m_Move.to = CoordinateImpl(to->Y, to->X);
}

Coordinate^	Move::GetFrom()
{
	return ref new Coordinate(m_Move.from.nColumn, m_Move.from.nRank);
}


Coordinate^	Move::GetTo()
{
	return ref new Coordinate(m_Move.to.nColumn, m_Move.to.nRank);
}

ChessPiece^	Move::GetPromotionPiece()
{
	return ref new ChessPiece(m_Move.promotionPiece);
}

Platform::String^	Move::ToString()
{
	auto strMove = m_Move.Serialize();
	return ManagedConverter::String2ManagedString(strMove);
}


void Move::FromString(Platform::String^ strMove)
{
	auto strNative = ManagedConverter::ManagedString2String(strMove);
	m_Move = MoveImpl::FromString(strNative);
}

MoveImpl Move::getMoveImpl() const
{
	return m_Move;
}

