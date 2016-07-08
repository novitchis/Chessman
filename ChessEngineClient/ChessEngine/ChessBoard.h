#pragma once
#include "ChessBoardImpl.h"
#include "ManagedDefines.h"
#include "Move.h"
#include "ChessPiece.h"
#include "MoveData.h"

using namespace Windows::Foundation::Collections;

namespace ChessEngine
{

	public ref class ChessBoard sealed
	{
	public:
		ChessBoard();
	
		void				Initialize(); //Initial Position
		void				Clear();
		Platform::String^	Serialize(int type);
		bool				LoadFrom(Platform::String^ strData, int type);
		void				StorePGN();
		ChessPiece^			GetPiece(Coordinate^ coord);
		bool				SubmitMove(Coordinate^ from, Coordinate^ to);
		bool				UndoMove(bool bWhiteMove);

		Windows::Foundation::Collections::IVector<MoveData^>^	GetMoves();


	private:
		SerializationType GetSerializationType(int type);
	
	private:
		ChessBoardImpl m_ChessBoardImpl;
		friend ref class Engine;
	};
}
