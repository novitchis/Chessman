#pragma once
#include "EngineDefines.h"

//wrapper over Coordinate class//
namespace ChessEngine
{
	public ref class Coordinate sealed
	{
	public:
		Coordinate();
		Coordinate(int x, int y);

		property bool IsWhiteSquare
		{
			bool get() { return m_Coordinate.IsWhiteSquare(); }
		}

		property int X
		{
			int get() { return m_Coordinate.nColumn; }
		}

		property int Y
		{
			int get() { return m_Coordinate.nRank; }
		}

		virtual Platform::String^	ToString() override;
		void						FromString(Platform::String^ strCoord);

	internal:
		CoordinateImpl getCoordinateImpl();

	private:
		CoordinateImpl	m_Coordinate;
	};
}
