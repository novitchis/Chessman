#include "pch.h"
#include "ChessBoard.h"
#include "ManagedConverter.h"

using namespace ChessEngine;

ChessBoard::ChessBoard()
{
}


void ChessBoard::Initialize()
{
	m_ChessBoardImpl.Initialize();
}


void ChessBoard::Clear()
{
	m_ChessBoardImpl.Clear();
}


Platform::String^ ChessBoard::Serialize(int type)
{
	auto strData = m_ChessBoardImpl.Serialize(GetSerializationType(type));
	return ManagedConverter::String2ManagedString(strData);
}

bool ChessBoard::LoadFrom(Platform::String^ strData, int type)
{
	auto strNativeData = ManagedConverter::ManagedString2String(strData);
	return m_ChessBoardImpl.LoadFrom(strNativeData, GetSerializationType(type));
}


void ChessBoard::StorePGN()
{
	m_ChessBoardImpl.StorePGN();
}


SerializationType ChessBoard::GetSerializationType(int type)
{
	if (type == BoardSerialization::BS_FEN) return ST_FEN;
	if (type == BoardSerialization::BS_PGN) return ST_PGN;
}
