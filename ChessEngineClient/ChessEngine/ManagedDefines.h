#pragma once

#include "EngineDefines.h"

namespace ChessEngine
{
	// wrapper over SerializationType //
	public ref class BoardSerialization sealed
	{
	public:
		static property int BS_FEN
		{
			int get() { return ST_FEN; }
		}
		static property int BS_PGN
		{
			int get() { return ST_PGN; }
		}
	};


	//wrapper over Move class//
	public ref class Coordinate sealed
	{
	public:

	private:
		CoordinateImpl	m_Coordinate;
	};


	//wrapper over Move class//
	public ref class Move sealed
	{
	public:

	private:
		MoveImpl	m_Move;
	};


}
