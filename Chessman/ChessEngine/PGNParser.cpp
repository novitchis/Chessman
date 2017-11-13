#include "pch.h"
#include"PGNPArser.h"
#include <regex>
#include <string>
#include <iostream>

using namespace ChessEngine;

char WHITESPACES[] = { ' ', '\t', '\n', '\r', '\0' };
PGNParser::PGNParser(const std::string& strPGNData)
	: m_strPGNData(strPGNData)
	, m_bValid(true)
	, m_nPos(0)
	, m_bWhiteMove(true)
{

}

std::list<GameInfo> PGNParser::ReadAllGames()
{
	std::list<GameInfo> result;
	while (!AtEnd())
	{
		GameInfo game;
		bool foundNewGame = ReadStartGame(game);
		if (!foundNewGame)
			break;

		while (true)
		{
			std::string strToken;
			if (!GetNext(strToken, game.GetTags()["Result"]))
				break;

			game.AddMove(strToken);
		}
		result.push_back(game);
	}
	return result;
}


bool PGNParser::ReadStartGame(GameInfo &gameInfo)
{
	if (!IsValid())
		return false;

	m_bWhiteMove = true;
	// Parse tags //
	SkipWhiteSpace();
	while (IsValid() && !AtEnd() && m_strPGNData[m_nPos] == '[')
	{
		ParseTag(gameInfo);
		SkipWhiteSpace();
	}

	return true;
}


bool PGNParser::GetNext(std::string& strMove, std::string resultTag)
{
	if (!IsValid())
		return false;

	SkipToNextToken();
	if (AtEnd())
		return false;

	// parse move number //
	if (m_bWhiteMove)
	{
		int resultPos = std::string::npos;
		if (!resultTag.empty())
			resultPos = (int)m_strPGNData.find(resultTag, m_nPos);
		
		int dotPos = (int)m_strPGNData.find('.', m_nPos);
		if (resultPos != std::string::npos && dotPos != std::string::npos)
			m_nPos = resultPos > dotPos ? dotPos : resultPos;
		else
			m_nPos = dotPos;

		if (m_nPos == std::string::npos)
		{
			Invalidate();
			return false;
		}

		if (m_nPos == dotPos)
			++m_nPos;
	}

	SkipToNextToken();
	if (!IsValid())
		return false;

	// parse move //
	int nMoveEnd = m_nPos;
	for (int i = m_nPos + 1; nMoveEnd < m_strPGNData.size(); ++i)
	{
		if (strchr(WHITESPACES, m_strPGNData[++nMoveEnd]) != NULL)
			break;
	}

	if (nMoveEnd == std::string::npos) 
	{
		Invalidate();
		return false;
	}

	// parse move number //
	// when the pgn contains variation the return to main line can look like: 10...Nf6
	strMove = m_strPGNData.substr(m_nPos, nMoveEnd - m_nPos);
	if (strMove == "0-0-0")
		strMove = "O-O-O";
	else if (strMove == "0-0")
		strMove = "O-O";

	m_nPos = nMoveEnd;
	m_bWhiteMove = !m_bWhiteMove;
	if (strMove == resultTag)
		return false;

	int lastPointIndex = (int)strMove.rfind('.');
	if (lastPointIndex != -1)
		strMove = strMove.substr(lastPointIndex + 1);

	return true;
}

void PGNParser::SkipToNextToken()
{
	int iterationPos = -1;
	while (iterationPos != m_nPos)
	{
		iterationPos = m_nPos;
		SkipWhiteSpace();
		SkipComments();
		SkipVariations();
	}
}

void PGNParser::SkipWhiteSpace()
{
	while (!AtEnd())
	{
		if (strchr(WHITESPACES, m_strPGNData[m_nPos]) == NULL)
			break;

		++m_nPos;
	}
}

void PGNParser::SkipComments()
{
	if (m_strPGNData[m_nPos] == ';') // comment until the end of the line
		m_nPos = (int)m_strPGNData.find_first_of("\n\r", m_nPos) + 1;
	if (m_strPGNData[m_nPos] == '{')
		m_nPos = (int)m_strPGNData.find_first_of("}", m_nPos) + 1;
}

void PGNParser::SkipVariations()
{
	if (m_strPGNData[m_nPos] == '(')
	{
		int stackCount = 0;
		while (!AtEnd())
		{
			if (m_strPGNData[m_nPos] == '(')
				++stackCount;
			else if (m_strPGNData[m_nPos] == ')')
				--stackCount;

			++m_nPos;

			if (stackCount == 0)
				break;

		}
	}
}

// TODO: Populate GameInfo struct //
// Skip tags for now //
void PGNParser::ParseTag(GameInfo &gameInfo)
{
	int nBracketPos = (int)m_strPGNData.find(']', m_nPos);
	int nFirstSpacePos = (int)m_strPGNData.find(' ', m_nPos);
	if (nBracketPos == std::string::npos || nFirstSpacePos == std::string::npos)
	{
		Invalidate();
		return;
	}

	std::string tagName = m_strPGNData.substr(m_nPos + 1, nFirstSpacePos - 1 - m_nPos);
	std::string tagValue = m_strPGNData.substr(nFirstSpacePos + 2, nBracketPos - 3 - nFirstSpacePos);
	gameInfo.AddTag(tagName, tagValue);

	m_nPos = nBracketPos + 1;
}
