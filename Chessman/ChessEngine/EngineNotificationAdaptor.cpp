#include "pch.h"
#include "EngineNotificationAdaptor.h"
#include "ManagedConverter.h"

using namespace ChessEngine;

EngineNotificationAdaptor::EngineNotificationAdaptor(IEngineNotification^ pManagedNofitication)
	:m_pManagedNotification( pManagedNofitication )
{
}


void EngineNotificationAdaptor::OnEngineMoveFinished(const std::vector<AnalysisDataImpl>& analysis)
{
	Platform::Array<AnalysisData^>^ result = ref new Platform::Array<AnalysisData^>(analysis.size());

	int iCrtLineIdx = -1;
	for (auto line_it : analysis) {
		Platform::Array<Move^>^ arrayAnalysis = ref new Platform::Array<Move^>(line_it.listAnalysis.size());
		AnalysisData^ analysisData = ref new AnalysisData();

		int iCrtIdx = -1;
		for (auto it : line_it.listAnalysis) {
			arrayAnalysis->set(++iCrtIdx, ManagedConverter::ConvertNativeMove(it));
		}
		analysisData->Analysis = arrayAnalysis;
		analysisData->Score = line_it.fScore;
		analysisData->IsBestMove = line_it.isBestMove;
		analysisData->Depth = line_it.depth;
		analysisData->NodesPerSecond = line_it.nodesPerSecond;
		analysisData->MultiPV = line_it.multiPV;

		result->set(++iCrtLineIdx, analysisData);
	}

	m_pManagedNotification->OnEngineMoveFinished(result);
}


void EngineNotificationAdaptor::OnEngineError()
{
	m_pManagedNotification->OnEngineError();
}


void EngineNotificationAdaptor::OnGameEnded(bool bWhiteWins)
{
	m_pManagedNotification->OnGameEnded(bWhiteWins);
}
