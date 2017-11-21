#include "pch.h"
#include "ChessEngineImpl.h"
#include "ChessEngineFactory.h"

using namespace ChessEngine;

ChessEngineImpl::ChessEngineImpl( const std::wstring& strEnginePath, IChessEngineNotifications* pNotification )
	: m_strEnginePath( strEnginePath )
{
	m_pEngineImpl = ChessEngineFactory::Create( CET_UCI );
	m_pEngineImpl->SetNotification( pNotification );
}


ChessEngineImpl::~ChessEngineImpl(void)
{
}


bool ChessEngineImpl::Start ()
{
	if ( !m_pEngineImpl->ConnectTo( m_strEnginePath) ) return false;
	return m_pEngineImpl->Start();
}


bool ChessEngineImpl::Stop ()
{
	return m_pEngineImpl->Stop();
}


bool ChessEngineImpl::Analyze( ChessBoardImpl& board )
{
	return m_pEngineImpl->Analyze( board );
}


void ChessEngineImpl::SetOptions( const EngineOptionsImpl& options )
{
	m_pEngineImpl->SetOptions( options );
}


void ChessEngineImpl::EnableMoveDelay()
{
	m_pEngineImpl->EnableMoveDelay();
}
