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
	try
	{
		auto strData = m_ChessBoardImpl.Serialize(GetSerializationType(type));
		return ManagedConverter::String2ManagedString(strData);
	}
	catch (...)
	{
		throw ref new Exception(3, ref new String(L"Failed to serialize the board."));
	}
}

bool ChessBoard::LoadFrom(Platform::String^ strData)
{
	try
	{
		auto strNativeData = ManagedConverter::ManagedString2String(strData);
		
		ChessBoardImpl parserBoardImpl;
		parserBoardImpl.StorePGN();

		bool isDataValid = parserBoardImpl.LoadFrom(strNativeData);
		if (isDataValid)
			m_ChessBoardImpl.LoadFrom(strNativeData); // TODO: :)  

		return isDataValid;
	}
	catch (...)
	{
		throw ref new Exception(3, ref new String(L"Failed to load the board."));
	}
}

ChessPiece^ ChessBoard::GetPiece(Coordinate^ coord)
{
	try
	{
		ChessPieceImpl piece = m_ChessBoardImpl.GetPiece(coord->getCoordinateImpl());
		if (piece.IsEmpty())
			return nullptr;

		return ref new ChessPiece(piece);
	}
	catch (...)
	{
		throw ref new Exception(3, ref new String(L"Failed to get the chessman."));
	}
}

void ChessBoard::SetPiece(Coordinate^ coord, ChessPiece^ piece)
{
	m_ChessBoardImpl.SetPiece(piece != nullptr ? piece->getPieceImpl() : ChessPieceImpl(), 
		coord->getCoordinateImpl());
}

void ChessBoard::SetSideToMove(bool white)
{
	m_ChessBoardImpl.SetSideToMove(white);
}

bool ChessBoard::SubmitMove(Coordinate^ from, Coordinate^ to)
{
	try
	{
		return m_ChessBoardImpl.SubmitMove(MoveImpl(from->getCoordinateImpl(), to->getCoordinateImpl()));
	}
	catch (...)
	{
		throw ref new Exception(3, ref new String(L"Failed to submit move."));
	}
}

bool ChessBoard::UndoMove(bool bWhiteMove)
{
	try
	{
		return m_ChessBoardImpl.UndoMove(bWhiteMove);
	}
	catch (...)
	{
		throw ref new Exception(3, ref new String(L"Failed to undo move."));
	}
}

bool ChessBoard::GoToMove(int moveIndex)
{
	try
	{
		return m_ChessBoardImpl.GoToMove(moveIndex);
	}
	catch (...)
	{
		throw ref new Exception(3, ref new String(L"Failed to move."));
	}
}

void ChessBoard::StorePGN()
{
	try
	{
		m_ChessBoardImpl.StorePGN();
	}
	catch (...)
	{
		throw ref new Exception(3, ref new String(L"Failed to store PGN."));
	}
}

IVector<MoveData^>^ ChessBoard::GetMoves(bool stopOnCurrent)
{
	try
	{
		Vector<MoveData^>^ result = ref new Vector<MoveData^>();
		std::list<MoveDataImpl> listMoves = m_ChessBoardImpl.GetMoves();

		int count = 0;
		int currentMoveIndex = m_ChessBoardImpl.GetCurrentMoveIndex();

		for (auto it = listMoves.begin(); it != listMoves.end(); ++it, ++count) {
			if (stopOnCurrent && count > currentMoveIndex)
				break;
			if (it->move == NULL) {
				result->Append(nullptr);
			}
			else {
				MoveData^ move = ref new MoveData(*it);
				move->IsCurrent = count == currentMoveIndex;
				result->Append(move);
			}
		}

		return result;
	}
	catch (...)
	{
		throw ref new Exception(3, ref new String(L"Failed to get the list of moves."));
	}
}

MoveData^ ChessBoard::GetCurrentMove()
{
	try
	{
		if (m_ChessBoardImpl.GetCurrentMoveIndex() < 0)
			return nullptr;

		return ref new MoveData(m_ChessBoardImpl.GetLastMove());
	}
	catch (...)
	{
		throw ref new Exception(3, ref new String(L"Failed to get current move."));
	}
}

IVector<MoveData^>^	ChessBoard::GetVariationMoveData(IVector<Move^>^ moves)
{
	try
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
	catch (...)
	{
		throw ref new Exception(3, ref new String(L"Failed to get engine results."));
	}
}

bool ChessBoard::IsWhiteTurn()
{
	try
	{
		return m_ChessBoardImpl.IsWhiteTurn();
	}
	catch (...)
	{
		throw ref new Exception(3, ref new String(L"Failed to get current side."));
	}
}

bool ChessBoard::AcceptEditedPosition()
{
	try
	{
		return m_ChessBoardImpl.AcceptEditedPosition();
	}
	catch (...)
	{
		throw ref new Exception(3, ref new String(L"Failed to check the board."));
	}
}


bool ChessBoard::IsStalemate()
{
	try
	{
		return m_ChessBoardImpl.IsStaleMate();
	}
	catch (...)
	{
		throw ref new Exception(3, ref new String(L"Failed to check for stalemate."));
	}
}


bool ChessBoard::IsCheckmate()
{
	try
	{
		return m_ChessBoardImpl.IsMate();
	}
	catch (...)
	{
		throw ref new Exception(3, ref new String(L"Failed to check for checkmate."));
	}
}

bool ChessBoard::IsCheck()
{
	try
	{
		return m_ChessBoardImpl.InCheck();
	}
	catch (...)
	{
		throw ref new Exception(3, ref new String(L"Failed to get is in check."));
	}
}

SerializationType ChessBoard::GetSerializationType(int type)
{
	if (type == BoardSerialization::BS_FEN) return ST_FEN;
	if (type == BoardSerialization::BS_PGN) return ST_PGN;
	
	return ST_FEN;
}
