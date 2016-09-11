#pragma once
#include "IChessEngine.h"
#include "EngineCommunicationThread.h"
#include "ChessBoardImpl.h"
#include <queue>

namespace ChessEngine
{
	enum UCICommand
	{
		UC_None = 0,
		UC_Uci,
		UC_Position,
		UC_Go,
		UC_Stop,
		UC_IsReady, 
		UC_SetOption,
	};

	struct EngineCommand
	{
		EngineCommand(UCICommand _type, const std::string& _strCommand)
			: type(_type)
			, strCommand(_strCommand)
		{}
		UCICommand		type;
		std::string		strCommand;
	};

	class UCIChessEngine : public IChessEngine
	{
	public:
		UCIChessEngine(void);
		~UCIChessEngine(void);
		virtual void SetNotification( IChessEngineNotifications* _pNotification );
		virtual bool ConnectTo( const std::wstring& strEnginePath );
		virtual bool Start();
		virtual bool Stop();
		virtual bool Analyze( ChessBoardImpl& board );
		virtual void SetOptions( const ChessEngineOptions& options );
		virtual void EnableMoveDelay();

		virtual void OnEngineResponse( const std::string& strResponse );

	private:
		void Initialize();
		void SendKeepAliveMessage();
		void EnterState( UCICommand state );
		void ResetState();

		void QueryEngineMove( const std::string& strFEN );
		
		// engine response processing //
		bool ProcessGoResponse( const std::string& strResponse );
		bool ProcessGoResponseBestMove( const std::string& strResponse );
		bool ProcessInitResponse( const std::string& strResponse );
		bool ProcessKeepAliveResponse( const std::string& strResponse );
	
	private:
		IChessEngineNotifications*				m_pNotification;
		std::shared_ptr<IEngineCommThread>		m_pCommThread;
		EngineIOData							m_IOData;
		HANDLE									m_hEngineProcess;
		std::map<UCICommand, std::string>		m_mapCommands;
		UCICommand								m_state; // maybe a std::stack<UCIEgineState> in the future //
		std::queue<EngineCommand>				m_queueCommands;
		bool									m_bDelayResponse;		
		std::string								m_strCumul;
	};
}
