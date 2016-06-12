#include "pch.h"
#include "Coordinate.h"
#include "ManagedConverter.h"

using namespace ChessEngine;

Coordinate::Coordinate()
{

}

Coordinate::Coordinate(int x, int y)
	: m_Coordinate(y, x)
{

}

Platform::String^ Coordinate::ToString()
{
	auto strCoord = m_Coordinate.ToString();
	return ManagedConverter::String2ManagedString(strCoord);
}


void Coordinate::FromString(Platform::String^ strCoord)
{
	m_Coordinate = CoordinateImpl::FromString(ManagedConverter::ManagedString2String(strCoord));
}
