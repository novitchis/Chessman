#include "pch.h"
#include "MoveData.h"

using namespace ChessEngine;

MoveData::MoveData()
{
}

MoveData::MoveData(MoveDataImpl moveData)
	: m_moveData(moveData)
{
}

MoveData^ MoveData::CreateEmptyMove()
{
	MoveDataImpl emptyMove;
	emptyMove.moveIndex = -1;
	emptyMove.strPGNMove = "...";
	return ref new MoveData(emptyMove);
}