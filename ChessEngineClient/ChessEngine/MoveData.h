#pragma once
#include "ChessBoardImpl.h"
#include "ManagedConverter.h"

namespace ChessEngine
{
	public ref class MoveData sealed
	{
	internal:
		MoveData(MoveDataImpl moveData);
	public:
		MoveData();

		virtual Platform::String^ ToString()override
		{
			return PgnMove;
		};

		property Platform::String^ PgnMove
		{
			Platform::String^ get() { return ManagedConverter::String2ManagedString(m_moveData.strPGNMove); }
		}

	private:
		MoveDataImpl m_moveData;
	};
}
