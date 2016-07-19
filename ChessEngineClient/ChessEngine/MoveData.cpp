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
