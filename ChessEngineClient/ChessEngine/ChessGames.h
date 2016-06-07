#pragma once
#include <set>
#include <string>

namespace ChessEngine
{
	enum ChessGameResult
	{
		CGR_Won = 0,
		CGR_Lost,
		CGR_Draw,
		CGR_InProgress,
	};
	
	struct SideInfo
	{
		SideInfo()
			: bHuman( false )
			, nLevel( 0 )
			, nTimeLeft( -1 )
		{

		}

		bool bHuman;
		int  nLevel;
		int  nTimeLeft;
	};
	
	struct ChessGameInfo
	{
		std::string		strPGN;
		SideInfo		WhiteSide;
		SideInfo		BlackSide;
	};

	class ChessGames
	{
	public:
		std::set<std::wstring>			List();
		bool							Load( const std::wstring& strName, ChessGameInfo& jGame );
		bool							Save( const std::wstring& strName, const ChessGameInfo& jGame );
		bool							HasItem( const std::wstring& strName );
		bool							Remove ( const std::wstring& strName );
		bool							RemoveAll();

	private:
		std::wstring					GetConfigFolder();
		std::wstring					GetFilePath( const std::wstring& strName );

	private:
		std::set<std::wstring>			m_vecGames;
	};
}
