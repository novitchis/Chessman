#pragma once
#include "UCIChessEngine.h"

namespace ChessEngine
{
	class ChessEngineFactory
	{
	public:
		static std::shared_ptr<IChessEngine> Create( ChessEngineType type )
		{
			switch (type)
			{
			case CET_UCI:
				return std::make_shared<UCIChessEngine> ();
			default:
				return NULL;
			}
		}
	};
}