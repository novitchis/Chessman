#pragma once
#include "EngineDefines.h"

namespace ChessEngine
{
	struct GameInfo
	{
		std::string strEvent;
		std::string  strSite;
		std::string  strDate;
		std::string  strRound;
		int			 nwhiteELO;
		int			 nBlackELO;
		bool		 bWhiteWon;
	};

	class PGNParser
	{
	public:
		PGNParser(const std::string& strPGNData);

		bool	IsValid() { return m_bValid; }
		void	Start();
		bool	GetNext(std::string& strMove);
		GameInfo GetGameInfo();

	private:
		void SkipWhiteSpace();
		void SkipComments();
		void ParseTag();
		bool AtEnd() { return m_nPos >= (int)m_strPGNData.size(); }
		void Invalidate() { m_bValid = false;  }
	private:
		std::string		m_strPGNData;
		GameInfo		m_GameInfo;
		bool			m_bValid;

		int				m_nPos;
		bool			m_bWhiteMove;
	};
}