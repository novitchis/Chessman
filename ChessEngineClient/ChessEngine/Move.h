#pragma once
#include "EngineDefines.h"
#include "Coordinate.h"
#include "ChessPiece.h"

namespace ChessEngine
{
	//wrapper over Move class//
	public ref class Move sealed
	{

	internal:
		Move(MoveImpl moveImpl);

	public:
		Move();
		Move(Coordinate^ from, Coordinate^ to);

		Coordinate^					GetFrom();
		Coordinate^					GetTo();
		ChessPiece^					GetPromotionPiece();

		virtual Platform::String^	ToString() override;
		void						FromString(Platform::String^ strMove);

	internal:
		MoveImpl getMoveImpl() const;
	private:
		MoveImpl	m_Move;
	};
}