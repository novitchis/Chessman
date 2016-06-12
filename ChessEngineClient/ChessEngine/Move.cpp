#include "pch.h"
#include "Move.h"
#include "ManagedConverter.h"

using namespace ChessEngine;

Move::Move()
{

}


Move::Move(Coordinate^ from, Coordinate^ to)
{
	m_Move.from = CoordinateImpl(from->X, to->Y);
	m_Move.to = CoordinateImpl(to->X, to->Y);
}



Coordinate^	Move::GetFrom()
{
	return ref new Coordinate(m_Move.from.nColumn, m_Move.from.nRank);
}


Coordinate^	Move::GetTo()
{
	return ref new Coordinate(m_Move.to.nColumn, m_Move.to.nRank);
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


