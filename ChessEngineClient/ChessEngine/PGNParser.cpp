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


void PGNParser::Start()
{
	// Parse tags //
	SkipWhiteSpace();

	while (IsValid() && !AtEnd() && m_strPGNData[m_nPos] == '[')
	{
		ParseTag();
		SkipWhiteSpace();
	}

	// remove result
	if (!m_GameInfo.strResult.empty())
	{
		int nResultPos = (int)m_strPGNData.rfind(m_GameInfo.strResult);
		// check if it's at the end of the string
		// possible results: "1-0", "0-1", "1/2-1/2", "*"
		if (nResultPos != std::string::npos && m_strPGNData.size() - nResultPos < 10)
			m_strPGNData.resize(nResultPos - 1);

	}
}


bool PGNParser::GetNext(std::string& strMove)
{
	if (!IsValid()) return false;
	SkipToNextToken();

	if (AtEnd()) return false;

	// parse move number //
	if (m_bWhiteMove)
	{
		m_nPos = (int)m_strPGNData.find('.', m_nPos);
		if (m_nPos == std::string::npos)
		{
			Invalidate();
			return false;
		}
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

	if (nMoveEnd == std::string::npos) {
		Invalidate();
		return false;
	}

	// parse move number //
	// when the pgn contains variation the return to main line can look like: 10...Nf6
	strMove = m_strPGNData.substr(m_nPos, nMoveEnd - m_nPos);

	int lastPointIndex = (int)strMove.rfind('.');
	if (lastPointIndex != -1)
		strMove = strMove.substr(lastPointIndex + 1);

	m_nPos = nMoveEnd;
	m_bWhiteMove = !m_bWhiteMove;

	return true;
}

GameInfo PGNParser::GetGameInfo()
{
	return m_GameInfo;
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
	{
		m_nPos = (int)m_strPGNData.find_first_of("\n\r", m_nPos) + 1;
	}
	if (m_strPGNData[m_nPos] == '{')
	{
		m_nPos = (int)m_strPGNData.find_first_of("}", m_nPos) + 1;
	}
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
void PGNParser::ParseTag()
{
	int nBracketPos = (int)m_strPGNData.find(']', m_nPos);
	if (nBracketPos == std::string::npos) {
		Invalidate();
		return;
	}

	// sample fen tag 
	// [FEN "rnbqkbnr/pppppppp/8/8/8/8/PPPPP1P1/RNBQKBNR w KQkq - 0 1"]
	if (m_strPGNData.substr(m_nPos + 1, 3) == "FEN")
	{
		int fenStartPos = m_nPos + 6;
		m_GameInfo.strFenStart = m_strPGNData.substr(fenStartPos, nBracketPos - 1 - fenStartPos);
	}
	else if (m_strPGNData.substr(m_nPos + 1, 6) == "Result")
	{
		int resultStartPos = m_nPos + 9;
		m_GameInfo.strResult = m_strPGNData.substr(resultStartPos, nBracketPos - 1 - resultStartPos);
	}

	m_nPos = nBracketPos + 1;
}
