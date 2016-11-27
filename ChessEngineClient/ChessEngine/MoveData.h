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

		property int Index
		{
			int get() { return m_moveData.moveIndex; }
		}

		property ChessEngine::Move^ Move
		{
			ChessEngine::Move^ get()
			{ 
				if (m_move == nullptr)
					m_move = ref new ChessEngine::Move(m_moveData.move);

				return m_move;
			}
		}

		static MoveData^ CreateEmptyMove();

	private:
		MoveDataImpl m_moveData;
		ChessEngine::Move^ m_move;
	};
}
