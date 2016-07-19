#include "pch.h"
#include "EngineNotificationAdaptor.h"
#include "ManagedConverter.h"

using namespace ChessEngine;

EngineNotificationAdaptor::EngineNotificationAdaptor(IEngineNotification^ pManagedNofitication)
{
}


void EngineNotificationAdaptor::OnEngineMoveFinished(const MoveImpl& move)
{
	m_pManagedNotification->OnEngineMoveFinished(ManagedConverter::ConvertNativeMove(move));
}


void EngineNotificationAdaptor::OnEngineError()
{
	m_pManagedNotification->OnEngineError();
}


void EngineNotificationAdaptor::OnGameEnded(bool bWhiteWins)
{
	m_pManagedNotification->OnGameEnded(bWhiteWins);
}



