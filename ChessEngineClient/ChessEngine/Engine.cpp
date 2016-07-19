#include "pch.h"
#include "Engine.h"
#include "ChessEngineFactory.h"

using namespace ChessEngine;


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
