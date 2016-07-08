#include "pch.h"
#include "ChessBoard.h"
#include "ManagedConverter.h"
#include <collection.h>
#include <algorithm>

using namespace ChessEngine;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation::Collections;

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

ChessPiece^ ChessBoard::GetPiece(Coordinate^ coord)
{
	ChessPieceImpl piece = m_ChessBoardImpl.GetPiece(coord->getCoordinateImpl());
	if (piece.IsEmpty())
		return nullptr;

	return ref new ChessPiece(piece);
}

bool ChessBoard::SubmitMove(Coordinate^ from, Coordinate^ to)
{
	return m_ChessBoardImpl.SubmitMove(MoveImpl(from->getCoordinateImpl(), to->getCoordinateImpl()));
}

bool ChessBoard::UndoMove(bool bWhiteMove)
{
	return m_ChessBoardImpl.UndoMove(bWhiteMove);
}

void ChessBoard::StorePGN()
{
	m_ChessBoardImpl.StorePGN();
}

IVector<MoveData^>^ ChessBoard::GetMoves()
{
	Vector<MoveData^>^ result = ref new Vector<MoveData^>();
	std::list<MoveDataImpl> listMoves = m_ChessBoardImpl.GetMoves();

	for (auto it = listMoves.begin(); it != listMoves.end(); ++it)
		result->Append(ref new MoveData(*it));

	return result;
}

SerializationType ChessBoard::GetSerializationType(int type)
{
	if (type == BoardSerialization::BS_FEN) return ST_FEN;
	if (type == BoardSerialization::BS_PGN) return ST_PGN;
	
	return ST_FEN;
}
