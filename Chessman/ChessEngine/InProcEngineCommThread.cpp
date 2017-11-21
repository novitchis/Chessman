#include "pch.h"
#include "InProcEngineCommThread.h"
#include "IChessEngine.h"
#include "Utils.h"

#include <iostream>

#include <Windows.h>
#include <sstream>

using namespace ChessEngine;
using namespace Core;

//#define LOGCOMM 1

InProcEngineCommThread::InProcEngineCommThread( IChessEngine* pEngine )
	: IEngineCommThread(pEngine)
	, m_bStop( false )
	, m_bErrorOccurred( false )
	, m_hCommandEvent( NULL )
{
	m_hCommandEvent = CreateEvent ( NULL, FALSE, FALSE, NULL );
	m_stmInput = std::make_shared<MemoryStream>();
	m_stmOutput = std::make_shared<MemoryStream>();
	m_StockfishEntryPoint.Init(m_stmInput, m_stmOutput);
}


InProcEngineCommThread::~InProcEngineCommThread(void)
{
	SafeCloseHandle( m_hCommandEvent );
}


void InProcEngineCommThread::Run()
{
	m_StockfishEntryPoint.Start();
	while ( !m_bStop && ( WaitForSingleObject( m_hCommandEvent, INFINITE ) == WAIT_OBJECT_0 ) ) 
	{
		std::string strEngineResponse;
		m_stmOutput->ReadToEnd(strEngineResponse);
		//if ( !ReadData( strEngineResponse ) ) 
		//{
		//	m_bErrorOccurred = true;
		//	break;
		//}

#ifdef LOGCOMM
		if (!strEngineResponse.empty())
		{
			std::wostringstream os_;
			os_ << "<-- " << strEngineResponse.c_str() << "\n";
			OutputDebugStringW(os_.str().c_str());
		}
#endif // LOGCOMM

		m_pEngine->OnEngineResponse( strEngineResponse );
		
		// remove the command only after the response has finished //
		//{
		//	AutoLock MLock(&m_csCommands);
		//	m_queueCommands.pop();
		//}
		//PushCommand();
	}
}


void InProcEngineCommThread::Stop()
{
	m_StockfishEntryPoint.Stop();
	m_bStop = true;
}


void InProcEngineCommThread::QueueCommand( const std::string& strCommand )
{
#ifdef LOGCOMM
	if (!strCommand.empty())
	{
		std::wostringstream os_;    
		os_ << "--> ";
		os_ << strCommand.c_str() << "\n";
		OutputDebugStringW( os_.str().c_str() );
	}
#endif

	WriteData( strCommand );
	SetEvent( m_hCommandEvent );
}

bool InProcEngineCommThread::HasErrors()
{
	return m_bErrorOccurred;
}


void InProcEngineCommThread::SetIOData( EngineIOData* pIOData )
{
}


bool InProcEngineCommThread::WriteData( const std::string& strData )
{
	AutoLock MLock(&m_csCommands);
	//if (m_queueCommands.empty())
	m_stmInput->Write(strData);
	//m_queueCommands.push(strData);
	return true;
}


bool InProcEngineCommThread::ReadData( std::string& strData )
{
	int nAvailableBytes = 0;
	bool bRes = true;
	while ((bRes = GetAvailableBytes(nAvailableBytes)) && nAvailableBytes)
	{
		DWORD dwBytesRead = 0;
		std::string strNextChunk;
		bRes = m_stmOutput->ReadToEnd(strNextChunk);
		strData += strNextChunk;
		if (!bRes) break;
		
		Sleep(50);
	}
	return bRes;
}


bool InProcEngineCommThread::GetAvailableBytes ( int& nBytes )
{
	m_stmOutput->Peek(nBytes);
	return true;
}


bool InProcEngineCommThread::PushCommand()
{
	AutoLock MLock(&m_csCommands);
	if (m_queueCommands.empty()) return true;
	auto strCommand = m_queueCommands.front();
	return m_stmInput->Write(strCommand);
}
