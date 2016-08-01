#include "pch.h"
#include "EngineCommunicationThread.h"
#include "IChessEngine.h"
#include "Utils.h"

#include <iostream>

using namespace ChessEngine;


EngineCommunicationThread::EngineCommunicationThread( IChessEngine* pEngine )
	: IEngineCommThread(pEngine)
	, m_pIOData( NULL )
	, m_bStop( false )
	, m_bErrorOccurred( false )
	, m_hCommandEvent( NULL )
{
	m_hCommandEvent = CreateEvent ( NULL, FALSE, FALSE, NULL );
}


EngineCommunicationThread::~EngineCommunicationThread(void)
{
	SafeCloseHandle( m_hCommandEvent );
}


void EngineCommunicationThread::Run()
{
	while ( !m_bStop && ( WaitForSingleObject( m_hCommandEvent, INFINITE ) == WAIT_OBJECT_0 ) ) 
	{
		std::string strEngineResponse;
		if ( !ReadData( strEngineResponse ) ) 
		{
			m_bErrorOccurred = true;
			break;
		}

		m_pEngine->OnEngineResponse( strEngineResponse );
	}
}


void EngineCommunicationThread::Stop()
{
	m_bStop = true;
}


void EngineCommunicationThread::QueueCommand( const std::string& strCommand )
{
	if (!strCommand.empty())
		WriteData( strCommand );
	SetEvent( m_hCommandEvent );
}


bool EngineCommunicationThread::HasErrors()
{
	return m_bErrorOccurred;
}


void EngineCommunicationThread::SetIOData( EngineIOData* pIOData )
{
	m_pIOData = pIOData;
}


bool EngineCommunicationThread::WriteData( const std::string& strData )
{
	DWORD dwBytesWritten = 0;
	BOOL bRes = WriteFile( m_pIOData->hStdInputWr, strData.c_str(), (DWORD)strData.size(), &dwBytesWritten, NULL );
	
	// TODO: use Logger //
	if ( !bRes ) {
		printf( "[EngineCommunicationThread::WriteData] WriteFile failed with %d.\n", GetLastError() );
		return FALSE;
	}
	FlushFileBuffers( m_pIOData->hStdInputWr );
	Sleep( 50 );
	return TRUE;
}


bool EngineCommunicationThread::ReadData( std::string& strData )
{
	int nAvailableBytes = 0;
	bool bRes = true;
	int nIndex = 0;
	while ( ( bRes = GetAvailableBytes( nAvailableBytes ) ) && nAvailableBytes )
	{
		DWORD dwBytesRead = 0;
		strData.resize( nAvailableBytes + nIndex );
		bRes = !!ReadFile( m_pIOData->hStdOutputRd, &strData[nIndex], nAvailableBytes, &dwBytesRead, NULL );
		if( !bRes ) break;
		_ASSERTE( dwBytesRead == nAvailableBytes );
		nIndex += dwBytesRead;
		Sleep( 50 );
	}

	return bRes;
}


bool EngineCommunicationThread::GetAvailableBytes ( int& nBytes )
{
#ifdef BACKEND
	DWORD dwBytesAvailable = 0;
	if(!m_pIOData )
        {
        printf( __FUNCTION__ " NULL found: %d",GetLastError() );
        return false;
        }
    try
        {
        if ( !PeekNamedPipe( m_pIOData->hStdOutputRd, NULL, 0, NULL, &dwBytesAvailable, NULL ) )
            {
            printf( "[EngineCommunicationThread::ReadData] PeekNamedPipe failed with error %d", GetLastError() );
            return false;
            }
        }
    catch (...)
        {
        printf( __FUNCTION__ " Exception found: %d",GetLastError() );
        return false;
        }
	
	nBytes = dwBytesAvailable;
#endif
	return true;
}
