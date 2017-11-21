#include "pch.h"
#include "GameInfo.h"

namespace ChessEngine
{
	GameInfo::GameInfo()
	{
	}

	void GameInfo::AddTag(std::string tagName, std::string tagValue)
	{
		pgnTags[tagName] = tagValue;
	}

	void GameInfo::AddMove(std::string pgnText)
	{
		pgnMoves.push_back(pgnText);
	}

	std::list<std::string> GameInfo::GetMoves()
	{
		return pgnMoves;
	}

	std::map<std::string, std::string> GameInfo::GetTags()
	{
		return pgnTags;
	}
}
