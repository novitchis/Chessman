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
		void				SetPiece(Coordinate^ coord, ChessPiece^ piece);
		void				SetSideToMove(bool white);

		bool				SubmitMove(Coordinate^ from, Coordinate^ to);
		bool				UndoMove(bool bWhiteMove);
		bool				GoToMove(int moveIndex);

		IVector<MoveData^>^	GetMoves(bool stopOnCurrent);
		MoveData^			GetCurrentMove();
		IVector<MoveData^>^	GetVariationMoveData(IVector<Move^>^ moves);

		bool				IsWhiteTurn();
		bool				IsValid();
		bool				IsStalemate();
		bool				IsCheckmate();


	private:
		SerializationType GetSerializationType(int type);
	
	private:
		ChessBoardImpl m_ChessBoardImpl;
		friend ref class Engine;
	};
}
