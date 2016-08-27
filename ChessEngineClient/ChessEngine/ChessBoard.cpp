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

bool ChessBoard::GoToMove(int moveIndex)
{
	return m_ChessBoardImpl.GoToMove(moveIndex);
}

void ChessBoard::StorePGN()
{
	m_ChessBoardImpl.StorePGN();
}

IVector<MoveData^>^ ChessBoard::GetMoves(bool stopOnCurrent)
{
	Vector<MoveData^>^ result = ref new Vector<MoveData^>();
	std::list<MoveDataImpl> listMoves = m_ChessBoardImpl.GetMoves();

	int count = 0;
	for (auto it = listMoves.begin(); it != listMoves.end(); ++it, ++count) {
		if (stopOnCurrent && count > m_ChessBoardImpl.GetCurrentMoveIndex())
			break;
		if (it->move == NULL) {
			result->Append(nullptr);
		}
		else {
			result->Append(ref new MoveData(*it));
		}
	}

	return result;
}

MoveData^ ChessBoard::GetCurrentMove()
{
	if (m_ChessBoardImpl.GetCurrentMoveIndex() < 0)
		return nullptr;

	return ref new MoveData(m_ChessBoardImpl.GetLastMove());
}

IVector<MoveData^>^	ChessBoard::GetVariationMoveData(IVector<Move^>^ moves)
{
	std::list<MoveImpl> movesImpl;
	for (auto&& elem : moves)
		movesImpl.push_back(elem->getMoveImpl());

	std::list<MoveDataImpl> variationMoves = m_ChessBoardImpl.GetVariationMoveData(movesImpl);

	Vector<MoveData^>^ result = ref new Vector<MoveData^>();
	for (auto it = variationMoves.begin(); it != variationMoves.end(); ++it)
		result->Append(ref new MoveData(*it));

	return result;
}

bool ChessBoard::IsWhiteTurn()
{
	return m_ChessBoardImpl.IsWhiteTurn();
}

SerializationType ChessBoard::GetSerializationType(int type)
{
	if (type == BoardSerialization::BS_FEN) return ST_FEN;
	if (type == BoardSerialization::BS_PGN) return ST_PGN;
	
	return ST_FEN;
}
