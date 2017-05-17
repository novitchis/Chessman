#pragma once
#include "ChessBoardImpl.h"
#include "ManagedConverter.h"
#include "ChessPiece.h"
#include "Coordinate.h"

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

		property bool IsCurrent
		{
			bool get() { return m_isCurrent; }
			void set(bool value) { m_isCurrent = value; }
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

		property bool EnPassantCapture
		{
			bool get()
			{
				return m_moveData.enPassantCapture;
			}
		}

		property ChessEngine::ChessPiece^ CapturedPiece
		{
			ChessEngine::ChessPiece^ get()
			{
				if (!m_moveData.capturedPiece.IsEmpty())
					return ref new ChessEngine::ChessPiece(m_moveData.capturedPiece);

				return nullptr;
			}
		}

		property bool IsCastle
		{
			bool get() 
			{
				return m_moveData.strPGNMove == "O-O" || m_moveData.strPGNMove == "O-O-O";
			}
		}

		static MoveData^ CreateEmptyMove();

	private:
		MoveDataImpl m_moveData;
		ChessEngine::Move^ m_move;
		bool m_isCurrent;
	};
}
