#include "pch.h"
#include "Engine.h"
#include "ChessEngineFactory.h"

using namespace ChessEngine;
using namespace Platform;


Engine::Engine(IEngineNotification^ pNotifications)
	:m_NotificationAdaptor( pNotifications )
{
	m_pEngineImpl = ChessEngineFactory::Create(CET_UCI);
	m_pEngineImpl->SetNotification(&m_NotificationAdaptor);

	//MemoryStream stm;
	//stm << "test";
	//std::string str;
	//std::string strToWrite;
	//stm.Write("bla\n");
	//stm.ReadLine(str);
	//strToWrite.resize(1038, 'c');
	//stm.Write(strToWrite);
	//stm.Write("ceva\n");
	//stm.ReadLine(str);
	//stm.Write("Ana are mere paul i le cere \n lkejg\nvewrnvwerng vfvnre2gv\n24t veruvn4ui4");
	//stm.ReadLine(str);
	//stm.ReadToEnd(str);
}


bool Engine::Connect()
{
	return m_pEngineImpl->ConnectTo(L"");
}

bool Engine::Start()
{
	try
	{
		return m_pEngineImpl->Start();
	}
	catch (...)
	{
		throw ref new Exception(2, ref new String(L"Failed to start the engine."));
	}
}

bool Engine::Stop()
{
	try
	{
		return m_pEngineImpl->Stop();
	}
	catch (...)
	{
		throw ref new Exception(2, ref new String(L"Failed to stop the engine."));
	}
}

bool Engine::Analyze(ChessBoard^ board)
{
	try
	{
		return m_pEngineImpl->Analyze(board->m_ChessBoardImpl);
	}
	catch (...)
	{
		throw ref new Exception(2, ref new String(L"Failed to analyze the current board."));
	}
}

bool Engine::StopAnalyzing()
{
	try
	{
		return m_pEngineImpl->StopAnalyzing();
	}
	catch (...)
	{
		throw ref new Exception(2, ref new String(L"Failed to stop analyzing."));
	}
}
