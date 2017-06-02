#pragma once
#include "EngineDefines.h"
#include "GameInfo.h"

namespace ChessEngine
{
	class PGNParser
	{
	public:
		PGNParser(const std::string& strPGNData);

		std::list<GameInfo>		ReadAllGames();
		bool					IsValid() { return m_bValid; }
		bool					ReadStartGame(GameInfo &gameInfo);
		bool					GetNext(std::string& strMove, std::string resultTag);
		bool					AtEnd() { return m_nPos >= (int)m_strPGNData.size(); }

	private:
		void SkipToNextToken();
		void SkipWhiteSpace();
		void SkipComments();
		void SkipVariations();
		void ParseTag(GameInfo &gameInfo);
		void Invalidate() { m_bValid = false; }

	private:
		std::string		m_strPGNData;
		bool			m_bValid;
		int				m_nPos;
		bool			m_bWhiteMove;
	};
}
