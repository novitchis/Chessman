#include "pch.h"
#include "MoveData.h"

using namespace ChessEngine;

MoveData::MoveData()
	: m_isCurrent(false)
{
}

MoveData::MoveData(MoveDataImpl moveData)
	: m_moveData(moveData), m_isCurrent(false)
{
}

MoveData^ MoveData::CreateEmptyMove()
{
	MoveDataImpl emptyMove;
	emptyMove.moveIndex = -1;
	emptyMove.strPGNMove = "...";
	return ref new MoveData(emptyMove);
}