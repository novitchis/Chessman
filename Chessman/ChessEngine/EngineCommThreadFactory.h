#pragma once
#include "IEngineCommThread.h"
#include "InProcEngineCommThread.h"
#include "EngineCommunicationThread.h"

namespace ChessEngine
{
	enum EngineCommType
	{
		ECT_InProc,
		ECT_ExternalProcess,
		ECT_Cloud, // TODO
	};

	class EngineCommThreadFactory
	{
	public:
		static std::shared_ptr<IEngineCommThread> Create( EngineCommType type, IChessEngine* pEngine )
		{
			switch (type)
			{
			case ECT_InProc:
				return std::make_shared<InProcEngineCommThread> (pEngine);
			case ECT_ExternalProcess:
				return std::make_shared<EngineCommunicationThread>(pEngine);
			case ECT_Cloud:
				return NULL; // TODO
			default:
				return NULL;
			}
		}
	};
}