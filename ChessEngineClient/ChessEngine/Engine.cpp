#include "pch.h"
#include "Engine.h"
#include "ChessEngineFactory.h"

using namespace ChessEngine;
Engine::Engine(IEngineNotification^ pNotifications)
	:m_pNotifications( pNotifications )
{
	m_pEngineImpl = ChessEngineFactory::Create(CET_UCI);
}


bool Engine::Connect()
{
	return m_pEngineImpl->ConnectTo(L"");
}

bool Engine::Start()
{
	return m_pEngineImpl->Start();
}

bool Engine::Stop()
{
	return m_pEngineImpl->Stop();
}

bool Engine::Analyze(ChessBoard^ board)
{
	return m_pEngineImpl->Analyze(board->m_ChessBoardImpl);
}
