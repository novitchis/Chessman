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
	// remove result //
	int nResultPos = (int)m_strPGNData.rfind('-');
	if(nResultPos != std::string::npos && m_strPGNData.size() - nResultPos < 10) {
		nResultPos -= 1;
		while (nResultPos > 0 && strchr(WHITESPACES, m_strPGNData[nResultPos]))
			--nResultPos;
		--nResultPos; // skip white score //
		if(nResultPos > 0)
			m_strPGNData.resize(nResultPos - 1);
	}
	else {
		//if unknown result => * is being used //
		nResultPos = (int)m_strPGNData.rfind('*');
		if(nResultPos != std::string::npos && m_strPGNData.size() - nResultPos < 10) // sanity check to avoid resizing to a comment //
			m_strPGNData.resize(nResultPos - 1);
	}
	// Parse tags //
	SkipWhiteSpace();
	
	while (IsValid() && !AtEnd() && m_strPGNData[m_nPos] == '[')
	{
		ParseTag();
		SkipWhiteSpace();
	}

	//std::cout << "pos:" << m_nPos << std::endl;
}


bool PGNParser::GetNext(std::string& strMove)
{
	if (!IsValid()) return false;
	SkipWhiteSpace();
	SkipComments();
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

	SkipWhiteSpace();
	if (!IsValid()) 
		return false;
	// parse move //
	
	int nMoveEnd = m_nPos;
	for (int i = m_nPos + 1; nMoveEnd < m_strPGNData.size(); ++i)
	{
		if (strchr(WHITESPACES, m_strPGNData[++nMoveEnd]) != NULL)
			break;
	}
	
	//m_strPGNData.find_first_of(WHITESPACES, m_nPos);
	//std::cout << "find_first_of:" << nMoveEnd << std::endl;
	if (nMoveEnd == std::string::npos) {
		Invalidate();
		return false;
	}
	strMove = m_strPGNData.substr(m_nPos, nMoveEnd-m_nPos);
	m_nPos = nMoveEnd;
	m_bWhiteMove = !m_bWhiteMove;
	
	//std::cout << "pos:" << m_nPos << std::endl;

	return true;
}

GameInfo PGNParser::GetGameInfo()
{
	return m_GameInfo;
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
		m_nPos = (int)m_strPGNData.find_first_of("\n\r", m_nPos);
	}
	if (m_strPGNData[m_nPos] == '{')
	{
		m_nPos = (int)m_strPGNData.find_first_of("}", m_nPos);
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

	m_nPos = nBracketPos+1;
}
