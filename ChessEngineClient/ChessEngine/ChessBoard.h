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
		bool				GoToMove(int moveIndex);

		IVector<MoveData^>^	GetMoves(bool stopOnCurrent);
		MoveData^			GetCurrentMove();


	private:
		SerializationType GetSerializationType(int type);
	
	private:
		ChessBoardImpl m_ChessBoardImpl;
		friend ref class Engine;
	};
}
