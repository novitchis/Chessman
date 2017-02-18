#include "pch.h"
#include "EngineNotificationAdaptor.h"
#include "ManagedConverter.h"

using namespace ChessEngine;

EngineNotificationAdaptor::EngineNotificationAdaptor(IEngineNotification^ pManagedNofitication)
	:m_pManagedNotification( pManagedNofitication )
{
}


void EngineNotificationAdaptor::OnEngineMoveFinished(const MoveImpl& move, const AnalysisDataImpl& analysis)
{
	Platform::Array<Move^>^ arrayAnalysis = ref new Platform::Array<Move^>(analysis.listAnalysis.size());
	AnalysisData^ analysisData = ref new AnalysisData();
	
	int iCrtIdx = -1;
	for (auto it : analysis.listAnalysis) {
		arrayAnalysis->set(++iCrtIdx, ManagedConverter::ConvertNativeMove(it));
	}
	analysisData->Analysis = arrayAnalysis;
	analysisData->Score = analysis.fScore;
	analysisData->IsBestMove = analysis.isBestMove;
	analysisData->Depth = analysis.depth;
	analysisData->NodesPerSecond = analysis.nodesPerSecond;

	m_pManagedNotification->OnEngineMoveFinished(ManagedConverter::ConvertNativeMove(move), analysisData);
}


void EngineNotificationAdaptor::OnEngineError()
{
	m_pManagedNotification->OnEngineError();
}


void EngineNotificationAdaptor::OnGameEnded(bool bWhiteWins)
{
	m_pManagedNotification->OnGameEnded(bWhiteWins);
}
