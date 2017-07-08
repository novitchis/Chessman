#include "pch.h"
#include "UCIChessEngine.h"
#include "EngineCommThreadFactory.h"
#include "Utils.h"
#include <sstream>
#include <iostream>
#include <time.h>
#include <regex>


using namespace ChessEngine;

UCIChessEngine::UCIChessEngine(void)
	: m_pNotification( NULL )
	, m_hEngineProcess( NULL )
	, m_state( UC_None )
	, m_bDelayResponse( false )
{
	Initialize();
	
	// TODO: add settings to choose the type of engine communication //
	// For now(alpha release) we'll only use the in-proc engine //
	m_pCommThread = EngineCommThreadFactory::Create(ECT_InProc, this);
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
#ifdef BACKEND
	if ( !PathFileExists ( strEnginePath.c_str() ) ) return false;

	// create pipes //
	SECURITY_ATTRIBUTES saAttr; 

	// Set the bInheritHandle flag so pipe handles are inherited. 
	saAttr.nLength = sizeof(SECURITY_ATTRIBUTES); 
	saAttr.bInheritHandle = TRUE; 
	saAttr.lpSecurityDescriptor = NULL; 

	// TODO: log win errors //
	DWORD dwErr;
	if ( ! CreatePipe(&m_IOData.hStdInputRd, &m_IOData.hStdInputWr, &saAttr, 0) ) 
		return false;
	dwErr = GetLastError();
	if ( ! SetHandleInformation(m_IOData.hStdInputWr, HANDLE_FLAG_INHERIT, 0) )
		return false;
	dwErr = GetLastError();
	if (! CreatePipe(&m_IOData.hStdOutputRd, &m_IOData.hStdOutputWr, &saAttr, 0)) 
		return false;
	if ( ! SetHandleInformation(m_IOData.hStdOutputRd, HANDLE_FLAG_INHERIT, 0) )
		return false;
	dwErr = GetLastError();

	PROCESS_INFORMATION piProcInfo = {0, }; 
	STARTUPINFO siStartInfo = {0, };
	BOOL bSuccess = FALSE; 
	
	siStartInfo.cb = sizeof(STARTUPINFO); 
	siStartInfo.hStdError = m_IOData.hStdOutputWr;
	siStartInfo.hStdOutput = m_IOData.hStdOutputWr;
	siStartInfo.hStdInput = m_IOData.hStdInputRd;
    siStartInfo.wShowWindow = SW_HIDE;
    siStartInfo.dwFlags |= STARTF_USESTDHANDLES | STARTF_USESHOWWINDOW;

	CString strPath;
	strPath.Format( L"\"%s\"", strEnginePath.c_str() );
	
	bSuccess = CreateProcess(NULL, strPath.GetBuffer(), NULL, NULL, TRUE, 0, NULL, NULL, &siStartInfo, &piProcInfo);  
	dwErr = GetLastError();	
	if( dwErr ) 
		printf( "CreateProcess Error: %d\n", dwErr );
	
	return !!bSuccess;
#endif
	return true;
}


bool UCIChessEngine::Start()
{
	m_pCommThread->SetIOData( &m_IOData );
	m_pCommThread->Start();

	EnterState( UC_Uci );
	m_pCommThread->QueueCommand( "uci\n" );

	return true;
}


bool UCIChessEngine::Stop()
{
	m_pCommThread->Stop();
	SendKeepAliveMessage();
	if( !m_pCommThread->Join() ) return false;
	return true;
}

bool UCIChessEngine::Analyze( ChessBoardImpl& board, int depth /*= -1*/, int moveTime /*= -1*/)
{
	if (board.IsMate() || board.IsStaleMate())
		return false;

	std::string strCommand = "position fen ";
	strCommand += board.Serialize( ST_FEN );
	strCommand += "\n";
	std::ostringstream commandStringStream;
	commandStringStream << "go";
	if (moveTime == -1 && depth == -1)
	{
		commandStringStream << " infinite";
	}
	else
	{
		if (moveTime != -1)
			commandStringStream << " movetime " << moveTime;

		if (depth != -1)
			commandStringStream << " depth " << depth;
	}
	
	// TODO: what is this?
	Core::AutoLock MLock(&m_lock);
	
	if (m_state == UC_Go)
	{
		EnterState(UC_Stop);
		m_pCommThread->QueueCommand("stop\n");
		m_queueCommands.push(EngineCommand(UC_IsReady, "isready\n"));
		m_queueCommands.push(EngineCommand(UC_Position, strCommand));
		m_queueCommands.push(EngineCommand(UC_Go, commandStringStream.str()));
	}
	else
	{
		if (m_state == UC_None)
			SendKeepAliveMessage();
		else
			m_queueCommands.push(EngineCommand(UC_IsReady, "isready\n"));

		m_queueCommands.push(EngineCommand(UC_Position, strCommand));
		m_queueCommands.push(EngineCommand(UC_Go, commandStringStream.str()));
	}

	return true;
}

bool UCIChessEngine::StopAnalyzing()
{
	if (m_state == UC_Go)
	{
		EnterState(UC_Stop);
		m_pCommThread->QueueCommand("stop\n");
		return true;
	}

	return false;
}

void UCIChessEngine::SetOptions( const EngineOptionsImpl& options )
{
	EnterState( UC_SetOption);
	std::stringstream stm;
	stm << "setoption name Skill Level value " << options.level << "\n";
	stm << "setoption name MultiPV value " << options.multiPV << "\n";

	m_pCommThread->QueueCommand( stm.str() );
	SendKeepAliveMessage();
}


void UCIChessEngine::EnableMoveDelay()
{
	m_bDelayResponse = true;
}


void UCIChessEngine::OnEngineResponse( const std::string& strResponse )
{
	Core::AutoLock MLock(&m_lock);
	UCICommand newState = m_state;
	switch (m_state)
	{
	case ChessEngine::UC_Uci:
		if (ProcessInitResponse(strResponse))
			newState = UC_None;
		break;
	case ChessEngine::UC_IsReady:
		// wait for is ready response
		if (ProcessKeepAliveResponse( strResponse ))
			newState = UC_None;

		m_pCommThread->QueueCommand("");
		break;
	case ChessEngine::UC_Position:
		newState = UC_None;
		break;
	case ChessEngine::UC_Go:
		if (ProcessGoResponse(strResponse))
		{
			newState = UC_Go; // remain in this state //
			m_pCommThread->QueueCommand("");
		}
		else
		{
			// go finished
			newState = UC_None;
		}
		break;
	case ChessEngine::UC_Stop:

		if (ProcessGoResponseBestMove(strResponse))
			newState = UC_None;
		else
			m_pCommThread->QueueCommand("");
		break;
	case ChessEngine::UC_None:
	default:
		break;
	}

	if (newState == UC_None && !m_queueCommands.empty())
	{
		auto nextCommand = m_queueCommands.front();
		EnterState(nextCommand.type);
		m_pCommThread->QueueCommand(nextCommand.strCommand);
		m_queueCommands.pop();
	}
	else
	{
		EnterState(newState);
	}
}


void UCIChessEngine::Initialize()
{

}


void UCIChessEngine::SendKeepAliveMessage()
{
	EnterState( UC_IsReady );
	m_pCommThread->QueueCommand( "isready\n" );
}


void UCIChessEngine::EnterState( UCICommand state )
{
	m_state = state;
}


void UCIChessEngine::ResetState()
{
	m_state = UC_None;
}


bool UCIChessEngine::ProcessGoResponseBestMove(const std::string& strResponse)
{
	// parse bestmove //
	// best move can come as the las move with other info lines before it
	auto vecLines = split<std::string>(strResponse, "\n\r");
	auto lastLienTokens = split<std::string>(vecLines[vecLines.size() - 1], " ");
	bool isBestMove = lastLienTokens[0] == "bestmove";

	if (isBestMove)
	{
		AnalysisDataImpl analysisData;
		analysisData.isBestMove = true;
		analysisData.listAnalysis.push_back(MoveImpl::FromString(lastLienTokens[1]));
		std::vector<AnalysisDataImpl> analysis;
		analysis.push_back(analysisData);
		m_pNotification->OnEngineMoveFinished(analysis);
	}

	return isBestMove;
}

bool UCIChessEngine::ProcessGoResponse( const std::string& strResponse)
{	
	//// parse bestmove //
	std::string strBestMove = "bestmove";
	auto nPos = strResponse.find( strBestMove );
	if (nPos != -1)
	{
		EnterState(UC_Stop);
		return !ProcessGoResponseBestMove(strResponse);
	}

	//m_strCumul += strResponse;
	auto vecLines = split<std::string>(strResponse, "\n");

	std::vector<std::string>	vecProspectedLines;
	int currentDepth = 1;
	for (auto it : vecLines)
	{
		// select only the lines with max depth
		if (it.find("seldepth") != -1) {
			int depthPos = it.find("depth") + 6;
			int nNextSpace = it.find(" ", depthPos);
			auto strDepth = it.substr(depthPos, nNextSpace - depthPos);
			int depth = atoi(strDepth.c_str());

			if (depth == currentDepth)
				vecProspectedLines.push_back(it);
			else if (depth > currentDepth)
			{
				currentDepth = depth;
				vecProspectedLines.clear();
				vecProspectedLines.push_back(it);
			}
		}
	}
	
	if (vecProspectedLines.empty()) return true;

	std::vector<AnalysisDataImpl> analysis;
	for (auto iter : vecProspectedLines)
	{
		int nCpPos = iter.find("cp") + 3;
		int nPvPos = iter.rfind("pv") + 3;
		int depthPos = iter.find("depth") + 6;
		int npsPos = iter.find("nps") + 4;
		int multipvPos = iter.find("multipv") + 8;

		int nNextSpace = iter.find(" ", nCpPos);
		auto strCp = iter.substr(nCpPos, nNextSpace - nCpPos);
		nNextSpace = iter.find(" ", depthPos);
		auto strDepth = iter.substr(depthPos, nNextSpace - depthPos);
		nNextSpace = iter.find(" ", npsPos);
		auto strNPS = iter.substr(npsPos, nNextSpace - npsPos);
		nNextSpace = iter.find(" ", multipvPos);
		auto strMultiPV = iter.substr(multipvPos, nNextSpace - multipvPos);
		auto strMoves = iter.substr(nPvPos, iter.size() - nPvPos);
		auto vecMoves = split<std::string>(strMoves, " ");

		if (vecMoves.empty()) continue;

		AnalysisDataImpl crtAnalysisData;
		crtAnalysisData.fScore = atof(strCp.c_str()) / 100.;
		crtAnalysisData.depth = atoi(strDepth.c_str());
		crtAnalysisData.nodesPerSecond = atoi(strNPS.c_str());
		crtAnalysisData.multiPV = atoi(strMultiPV.c_str());
		for (auto it : vecMoves)
		{
			crtAnalysisData.listAnalysis.push_back(MoveImpl::FromString(it));
		}

		analysis.push_back(crtAnalysisData);
	}
	
	if (analysis.size() == 0)
		return true; // ignore it!

	m_pNotification->OnEngineMoveFinished(analysis);

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
