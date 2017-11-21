#pragma once
#include "pch.h"
#include <string>
#include <list>
#include <map>


namespace ChessEngine
{
	class GameInfo
	{
	public:
		GameInfo();

		void								AddTag(std::string tagName, std::string tagValue);
		void								AddMove(std::string pgnText);

		std::list<std::string>				GetMoves();
		std::map<std::string, std::string>	GetTags();

	private:
		/*
		The seven tag names of the STR are (in order):
		Event (the name of the tournament or match event)
		Site (the location of the event)
		Date (the starting date of the game)
		Round (the playing round ordinal of the game)
		White (the player of the white pieces)
		Black (the player of the black pieces)
		Result (the result of the game)
		*/
		std::map<std::string, std::string>	pgnTags;
		std::list<std::string>				pgnMoves;
	};
}
