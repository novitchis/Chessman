#include "pch.h"
#include "ChessEngineEntryPoint.h"
#include <iostream>

#include "bitboard.h"
#include "evaluate.h"
#include "position.h"
#include "search.h"
#include "thread.h"
#include "tt.h"
#include "uci.h"
#include "syzygy/tbprobe.h"


void StockfishThread::Run()
{
	UCI::loop(1, NULL);
}

ChessEngineEntryPoint::ChessEngineEntryPoint()
{
}


bool ChessEngineEntryPoint::Init(std::shared_ptr<MemoryStream> stmInput, std::shared_ptr<MemoryStream> stmOutput)
{
	UCI::init(Options);
	PSQT::init();
	Bitboards::init();
	Position::init();
	Bitbases::init();
	Search::init();
	Eval::init();
	Pawns::init();
	Threads.init();
	Tablebases::init(Options["SyzygyPath"]);
	TT.resize(Options["Hash"]);
	
	UCI::setInputStream(stmInput);
	UCI::setOutputStream(stmOutput);
	return true;
}


bool ChessEngineEntryPoint::Start()
{
	m_uciThread.Start();
	return true;
}


bool ChessEngineEntryPoint::Stop()
{
	m_uciThread.Join();
	Threads.exit();
	UCI::setInputStream(NULL);
	UCI::setOutputStream(NULL);
	return true;
}


bool ChessEngineEntryPoint::ProcessCommand(const std::string& strCommand)
{

	return true;
}
