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

		int							GetX();
		int							GetY();
		virtual Platform::String^	ToString() override;
		void						FromString(Platform::String^ strCoord);
	private:
		CoordinateImpl	m_Coordinate;
	};
}
