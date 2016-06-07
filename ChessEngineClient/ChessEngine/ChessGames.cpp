#include "pch.h"
#include "ChessGames.h"

#include <windows.h>
#include <ShlObj.h>
#include <Shlwapi.h>
#include <fstream>
#include <streambuf>
#include <algorithm>
#include "json.h"

using namespace ChessEngine;

std::set<std::wstring> ChessGames::List()
{
	if ( !m_vecGames.empty() ) return m_vecGames;

	//std::wstring strFolderPath = GetConfigFolder();
	//if ( strFolderPath.empty() ) return m_vecGames;

	//strFolderPath += L"\\*.mcf";
	//// enumerate content //
	//WIN32_FIND_DATA sFindData;
	//HANDLE hFind = FindFirstFile( strFolderPath.c_str(), &sFindData);
	//if (hFind!=INVALID_HANDLE_VALUE) {
	//	do {
	//		// ignore . and .. //
	//		std::wstring strFileName = sFindData.cFileName;
	//		if ((strFileName==L".") || (strFileName==L"..")) continue;
	//		auto nDotPos = strFileName.find_last_of('.');
	//		strFileName = strFileName.substr(0, nDotPos);
	//		m_vecGames.insert( strFileName );
	//		// next file/folder //
	//	} while (FindNextFile(hFind, &sFindData));

	//	// cleanup //
	//	if ( hFind ) FindClose(hFind);
	//}

	return m_vecGames;
}


bool ChessGames::Load( const std::wstring& strName, ChessGameInfo& GameInfo )
{
	auto strTarget = GetFilePath( strName );
	std::ifstream stm(strTarget);
	std::string strData;

	stm.seekg(0, std::ios::end);   
	strData.reserve(stm.tellg());
	stm.seekg(0, std::ios::beg);

	strData.assign((std::istreambuf_iterator<char>(stm)), std::istreambuf_iterator<char>());
	
	try
	{
		auto jGame = json::Deserialize( strData );
		auto jWhite = jGame["white"];
		auto jBlack = jGame["black"];

		GameInfo.WhiteSide.bHuman		= jWhite["Human"];
		GameInfo.WhiteSide.nLevel		= jWhite["Level"];
		GameInfo.WhiteSide.nTimeLeft	= jWhite["Time"];

		GameInfo.BlackSide.bHuman		= jBlack["Human"];
		GameInfo.BlackSide.nLevel		= jBlack["Level"];
		GameInfo.BlackSide.nTimeLeft	= jBlack["Time"];

		GameInfo.strPGN = jGame["PGN"];
	}
	catch( std::runtime_error& )
	{

	}
	
	return true;
}


bool ChessGames::Save( const std::wstring& strName, const ChessGameInfo& GameInfo )
{
	auto strTarget = GetFilePath( strName );
	json::Object jGame;
	json::Object jWhite;
	json::Object jBlack;
	
	jWhite["Human"] = GameInfo.WhiteSide.bHuman;
	jWhite["Level"] = GameInfo.WhiteSide.nLevel;
	jWhite["Time"] = GameInfo.WhiteSide.nTimeLeft;
	
	jBlack["Human"] = GameInfo.BlackSide.bHuman;
	jBlack["Level"] = GameInfo.BlackSide.nLevel;
	jBlack["Time"] = GameInfo.BlackSide.nTimeLeft;

	jGame["white"] = jWhite;
	jGame["black"] = jBlack;
	jGame["PGN"] = GameInfo.strPGN;

	auto strGame = json::Serialize( jGame );

	std::ofstream out( strTarget );
	out << strGame;
	out.close();

	m_vecGames.insert( strName );
	return true;
}


bool ChessGames::HasItem( const std::wstring& strName )
{
	List();
	return m_vecGames.find( strName ) != m_vecGames.end();
}

bool ChessGames::Remove( const std::wstring& strName )
{
	auto strTarget = GetFilePath( strName );
	m_vecGames.erase( strName );
	return !!DeleteFile( strTarget.c_str() );
}


bool ChessGames::RemoveAll()
{
	auto vecSavedGames = List();

	for ( auto it : vecSavedGames )
		if ( !Remove( it ) ) return false;

	return true;
}


std::wstring ChessGames::GetConfigFolder()
{
	std::wstring strConfigFolder;
	//wchar_t strAppData[MAX_PATH+1] = {0, };
	//if ( !SHGetSpecialFolderPath( NULL, strAppData, CSIDL_APPDATA, false ) ) return L"";
	//strConfigFolder = strAppData;
	//strConfigFolder += L"\\Motion Chess";
	//CreateDirectory( strConfigFolder.c_str(), NULL );
	//
	//strConfigFolder += L"\\Saved Games";
	//CreateDirectory( strConfigFolder.c_str(), NULL );

	return strConfigFolder;
}


std::wstring ChessGames::GetFilePath( const std::wstring& strName )
{
	std::wstring strResult = GetConfigFolder();
	if ( strResult.empty() ) return false;
	strResult += L"\\";
	strResult += strName;
	strResult += L".mcf";
	return strResult;
}
