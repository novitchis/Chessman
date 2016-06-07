#include "pch.h"
#include "UCIChessEngine.h"

#include <sstream>
#include <iostream>
#include <time.h>

using namespace ChessEngine;

UCIChessEngine::UCIChessEngine(void)
	: m_pNotification( NULL )
	, m_hEngineProcess( NULL )
	, m_state( UC_None )
	, m_CommThread( this )
	, m_bDelayResponse( false )
{
	Initialize();
}


UCIChessEngine::~UCIChessEngine(void)
{
}


void UCIChessEngine::SetNotification( IChessEngineNotifications* _pNotification )
{
	m_pNotification = _pNotification;
}


bool UCIChessEngine::ConnectTo( const std::wstring& strEnginePath )
{
	//if ( !PathFileExists ( strEnginePath.c_str() ) ) return false;

	//// create pipes //
	//SECURITY_ATTRIBUTES saAttr; 

	//// Set the bInheritHandle flag so pipe handles are inherited. 
	//saAttr.nLength = sizeof(SECURITY_ATTRIBUTES); 
	//saAttr.bInheritHandle = TRUE; 
	//saAttr.lpSecurityDescriptor = NULL; 

	//// TODO: log win errors //
	//DWORD dwErr;
	//if ( ! CreatePipe(&m_IOData.hStdInputRd, &m_IOData.hStdInputWr, &saAttr, 0) ) 
	//	return false;
	//dwErr = GetLastError();
	//if ( ! SetHandleInformation(m_IOData.hStdInputWr, HANDLE_FLAG_INHERIT, 0) )
	//	return false;
	//dwErr = GetLastError();
	//if (! CreatePipe(&m_IOData.hStdOutputRd, &m_IOData.hStdOutputWr, &saAttr, 0)) 
	//	return false;
	//if ( ! SetHandleInformation(m_IOData.hStdOutputRd, HANDLE_FLAG_INHERIT, 0) )
	//	return false;
	//dwErr = GetLastError();

	//PROCESS_INFORMATION piProcInfo = {0, }; 
	//STARTUPINFO siStartInfo = {0, };
	BOOL bSuccess = FALSE; 
	//
	//siStartInfo.cb = sizeof(STARTUPINFO); 
	//siStartInfo.hStdError = m_IOData.hStdOutputWr;
	//siStartInfo.hStdOutput = m_IOData.hStdOutputWr;
	//siStartInfo.hStdInput = m_IOData.hStdInputRd;
 //   siStartInfo.wShowWindow = SW_HIDE;
 //   siStartInfo.dwFlags |= STARTF_USESTDHANDLES | STARTF_USESHOWWINDOW;

	//CString strPath;
	//strPath.Format( L"\"%s\"", strEnginePath.c_str() );
	//
	//bSuccess = CreateProcess(NULL, strPath.GetBuffer(), NULL, NULL, TRUE, 0, NULL, NULL, &siStartInfo, &piProcInfo);  
	//dwErr = GetLastError();	
	//if( dwErr ) 
	//	printf( "CreateProcess Error: %d\n", dwErr );
	
	return !!bSuccess;
}


bool UCIChessEngine::Start()
{
	m_CommThread.SetIOData( &m_IOData );
	m_CommThread.Start();

	EnterState( UC_Uci );
	m_CommThread.QueueCommand( "uci\n" );
	
	srand (time(NULL));
	return true;
}


bool UCIChessEngine::Stop()
{
	m_CommThread.Stop();
	SendKeepAliveMessage();
	if( !m_CommThread.Join() ) return false;
	return true;
}


bool UCIChessEngine::Analyze( ChessBoardImpl& board )
{
	std::string strCommand = "position fen ";
	strCommand += board.Serialize( ST_FEN );
	strCommand += "\n";
	EnterState( UC_Position );
	m_CommThread.QueueCommand( strCommand );
	return true;
}

void UCIChessEngine::SetOptions( const ChessEngineOptions& options )
{
	// set level //
	int nLevel = options.level;
	//switch ( options.level )
	//{
	//case EL_Begginer:
	//	nLevel = 0;
	//	break;
	//case EL_Middle:
	//	nLevel = 5;
	//	break;
	//case EL_Advanced:
	//	nLevel = 10;
	//	break;
	//case EL_Impossible:
	//	nLevel = 15;
	//	break;
	//default:
	//	break;
	//}
	EnterState( UC_SetOption );
	std::stringstream stm;
	stm << "setoption name Skill Level value " << nLevel << "\n";
	m_CommThread.QueueCommand( stm.str() );
}


void UCIChessEngine::EnableMoveDelay()
{
	m_bDelayResponse = true;
}

void UCIChessEngine::OnEngineResponse( const std::string& strResponse )
{
	UCICommand newState = UC_None;
	switch (m_state)
	{
	case ChessEngine::UC_Uci:
		ProcessInitResponse( strResponse );
		break;
	case ChessEngine::UC_IsReady:
		ProcessKeepAliveResponse( strResponse );
		break;
	case ChessEngine::UC_Position:
		newState = UC_Go;
		m_CommThread.QueueCommand( "go\n" );
		break;
	case ChessEngine::UC_Go:
		ProcessGoResponse( strResponse );
		break;
	case ChessEngine::UC_None:
	default:
		break;
	}

	EnterState( newState );
}


void UCIChessEngine::Initialize()
{

}


void UCIChessEngine::SendKeepAliveMessage()
{
	EnterState( UC_IsReady );
	m_CommThread.QueueCommand( "isready" );
}


void UCIChessEngine::EnterState( UCICommand state )
{
	m_state = state;
}


void UCIChessEngine::ResetState()
{
	m_state = UC_None;
}


bool UCIChessEngine::ProcessGoResponse( const std::string& strResponse )
{	
	// parse bestmove //
	std::string strBestMove = "bestmove";
	auto nPos = strResponse.find( strBestMove );
	if ( nPos == -1 ) 
	{
		m_pNotification->OnEngineError();
		return false;
	}

	auto strMove = strResponse.substr( nPos + strBestMove.size() + 1 );
	auto nSpacePos = strMove.find( ' ' );
	if ( nSpacePos != std::string::npos ) 
		strMove = strMove.substr( 0, nSpacePos );

	MoveImpl move = MoveImpl::FromString( strMove );

	if ( m_bDelayResponse )
	{
		auto nSleepValue = 1000 + rand() % 1000;
		Sleep( nSleepValue );
	}

	m_pNotification->OnEngineMoveFinished( move );
	return true;
}

bool UCIChessEngine::ProcessInitResponse( const std::string& strResponse )
{
	// TODO: settings parse //
	return true;
}


bool UCIChessEngine::ProcessKeepAliveResponse( const std::string& strResponse )
{
	return ( strResponse == "readyok" );
}
