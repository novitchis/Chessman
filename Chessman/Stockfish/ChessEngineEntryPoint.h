#pragma once
#include <string>
#include "ThreadBase.h"
#include "MemoryStream.h"

class StockfishThread : public Core::ThreadBase
{
	virtual void Run();
};

class ChessEngineEntryPoint
{
public:
	ChessEngineEntryPoint();

	bool Init(std::shared_ptr<MemoryStream> stmInput, std::shared_ptr<MemoryStream> stmOutput);
	bool Start();
	bool Stop();
	bool ProcessCommand(const std::string& strCommand);

private:
	StockfishThread m_uciThread;
};

