#pragma once
#include "EngineDefines.h"
#include "Coordinate.h"

namespace ChessEngine
{
	//wrapper over Move class//
	public ref class Move sealed
	{
	public:
		Move();
		Move(Coordinate^ from, Coordinate^ to);

		Coordinate^					GetFrom();
		Coordinate^					GetTo();
		virtual Platform::String^	ToString() override;
		void						FromString(Platform::String^ strMove);
	
	private:
		MoveImpl	m_Move;
	};
}