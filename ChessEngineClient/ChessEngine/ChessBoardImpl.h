#pragma once
#include "pch.h"
#include "EngineDefines.h"
#include "StatePreserver.h"

#include <map>
#include <list>

namespace ChessEngine
{
	enum SerializationType
	{
		ST_FEN,
		ST_PGN,
	};

	enum CastlingType
	{
		CT_WhiteKingSide = 1 << 0,
		CT_WhiteQueenSide = 1 << 1,
		CT_BlackKingSide = 1 << 2,
		CT_BlackQueenSide = 1 << 3,
	};

	const int FullCastlingMask = CT_WhiteKingSide | CT_WhiteQueenSide | CT_BlackKingSide | CT_BlackQueenSide;

	struct MoveData
	{
		MoveData ()
			: nCastlingMask( FullCastlingMask )
		{

		}
		MoveData( const MoveImpl& _move, const ChessPiece& _capturedPiece )
			: move( _move )
			, capturedPiece( _capturedPiece )
			, nCastlingMask( FullCastlingMask )
		{

		}
		MoveImpl			move;
		ChessPiece		capturedPiece;
		int				nCastlingMask;
		int				nLastPawnMoveOrCapture;
		std::string		strPGNMove;
		std::string		strPGNUserFriendly;
	};

	enum StatePreserveType
	{
		SPT_InCheck = 0,
		SPT_IsMate,
		SPT_IsStaleMate,
		SPT_PGNSerialization,
		SPT_PGNDisplay
	};

	class ChessBoardImpl
	{
	public:
		ChessBoardImpl();
		ChessBoardImpl(const ChessBoardImpl& board);

		~ChessBoardImpl();
		
		// TODO //
        void						Initialize(); //Initial Position
		void						Clear();
		std::string					Serialize( SerializationType type ); 
		bool						LoadFrom( const std::string& strData, SerializationType type );
		void						StorePGN();

		ChessPiece					GetPiece( const CoordinateImpl& coord ) const;
		void						SetPiece( const ChessPiece& piece, const CoordinateImpl& coord );

		std::list<CoordinateImpl>	GetAvailableMoves( const CoordinateImpl& coord ) const;
		bool						SubmitMove( const MoveImpl& move, AdditionalMoveInfo& additionalInfo);
		bool						SubmitMove( const MoveImpl& move );
		std::string					GetLastMoveText();

		bool						UndoMove( bool bWhiteMove ); // TODO
		bool						ValidateMove( const MoveImpl& move, AdditionalMoveInfo& coordEnPassant ) const;
		bool						PromotePawn( CoordinateImpl coord, ChessPiece piece );

		std::list<CoordinateImpl>	getPiecesMovableAs(MoveImpl move, char pieceType, bool pWhite) const;
		bool						GetAttackingFields( const CoordinateImpl& coord, bool bWhiteAttacks, std::list<CoordinateImpl>& listAttacker ) const;
		CoordinateImpl				GetKingPos( bool bWhite ) const;
		bool						IsWhiteTurn() const;
		bool						InCheck();
		bool						IsMate();
		bool						IsStaleMate();
		MoveImpl						GetLastMove() const;
		std::map<ChessPiece, int>	GetCapturedPieces() const;	
		void						GetPreservedState( StatePreserveType type, Core::StatePreserver& state );
		void						UpdateState( StatePreserveType type, const Core::Variant& vtState );

	private:
		std::string					Serialize2FEN() const;
		std::string					Serialize2PGN();
		bool						LoadFromFEN( const std::string& strData );
		bool						LoadFromPGN( const std::string& strData );

	private:
		ChessPiece							m_memBoard[8][8];
		CoordinateImpl							m_coordWhiteKing;
		CoordinateImpl							m_coordBlackKing;
		std::list<MoveData>					m_listMoves;
		ChessPiece							m_lastPiece;
		int									m_nCastlingMask;
		int									m_nLastPawnMoveOrCapture;
		bool								m_bStorePGN;
		std::map< StatePreserveType, Core::StatePreserver > m_mapStates;
		
		friend class MoveValidationAlgorithm;
		friend class MoveScope;
	};
}
