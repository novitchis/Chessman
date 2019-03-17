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
		ST_FEN = 0,
		ST_PGN,
		ST_DETECT,
	};

	enum CastlingType
	{
		CT_WhiteKingSide = 1 << 0,
		CT_WhiteQueenSide = 1 << 1,
		CT_BlackKingSide = 1 << 2,
		CT_BlackQueenSide = 1 << 3,
	};

	const int FullCastlingMask = CT_WhiteKingSide | CT_WhiteQueenSide | CT_BlackKingSide | CT_BlackQueenSide;

	struct MoveDataImpl
	{
		MoveDataImpl()
			: nCastlingMask(FullCastlingMask)
		{
			pawnTransform = false;
		}

		MoveDataImpl(int _moveIndex, const MoveImpl& _move, const ChessPieceImpl& _capturedPiece)
			: move(_move)
			, capturedPiece(_capturedPiece)
			, nCastlingMask(FullCastlingMask)
			, moveIndex(_moveIndex)
		{
			pawnTransform = false;
		}

		MoveImpl			move;
		ChessPieceImpl		capturedPiece;
		int					nCastlingMask;
		bool				enPassantCapture;
		int					nLastPawnMoveOrCapture;
		int					moveIndex;
		bool				pawnTransform;
		std::string			strPGNMove;
		std::string			strPGNUserFriendly;
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
		std::string					Serialize(SerializationType type, bool stopOnCurrent = true);
		bool						LoadFrom(const std::string& strData);
		void						StorePGN();

		ChessPieceImpl				GetPiece(const CoordinateImpl& coord) const;

		// edit board
		void						SetPiece(const ChessPieceImpl& piece, const CoordinateImpl& coord);
		void						SetSideToMove(bool bWhite);
		bool						AcceptEditedPosition();

		std::list<MoveDataImpl>		GetMoves();
		int							GetCurrentMoveIndex();
		std::list<MoveDataImpl>		GetVariationMoveData(std::list<MoveImpl> moves);

		std::list<CoordinateImpl>	GetAvailableMoves(const CoordinateImpl& coord) const;
		bool						SubmitMove(const MoveImpl& move, AdditionalMoveInfo& additionalInfo);
		bool						SubmitMove(const MoveImpl& move);
		std::string					GetLastMoveText();

		bool						UndoMove(bool bWhiteMove);
		bool						GoToMove(int moveIndex);
		bool						ValidateMove(const MoveImpl& move) const;
		bool						ValidateMove(const MoveImpl& move, AdditionalMoveInfo& coordEnPassant) const;
		bool						PromotePawn(CoordinateImpl coord, ChessPieceImpl piece);

		std::list<CoordinateImpl>		getPiecesMovableAs(MoveImpl move, char pieceType, bool pWhite) const;
		bool							GetAttackingFields(const CoordinateImpl& coord, bool bWhiteAttacks, std::list<CoordinateImpl>& listAttacker) const;
		CoordinateImpl					GetKingPos(bool bWhite) const;
		bool							IsWhiteTurn() const;
		bool							InCheck();
		bool							IsMate();
		bool							IsStaleMate();
		MoveDataImpl					GetLastMove() const;
		std::map<ChessPieceImpl, int>	GetCapturedPieces() const;
		void							GetPreservedState(StatePreserveType type, Core::StatePreserver& state);
		void							UpdateState(StatePreserveType type, const Core::Variant& vtState);
		void							ClearPreservedStates();

		MoveImpl					GetMoveFromPGN(const std::string& pgnMove);

	private:
		std::string					Serialize2FEN() const;
		std::string					Serialize2PGN(bool stopOnCurrent = true);
		bool						LoadFromFEN(const std::string& strData);
		bool						LoadFromPGN(const std::string& strData);

		SerializationType			DetectFormat(const std::string& strData);
		bool						IsEnPassantMove(AdditionalMoveInfo& coordEnPassant) const;
		void						RemoveMovesAfterCurrent();
		bool						IsValidFen(const std::string& strData);

	private:
		ChessPieceImpl				m_memBoard[8][8];
		CoordinateImpl				m_coordWhiteKing;
		CoordinateImpl				m_coordBlackKing;
		std::list<MoveDataImpl>		m_listMoves;
		ChessPieceImpl				m_lastPiece;
		int							m_nCastlingMask;
		int							m_nLastPawnMoveOrCapture;
		bool						m_bStorePGN;
		int							m_currentMoveIndex;
		std::map< StatePreserveType, Core::StatePreserver > m_mapStates;
		std::string					m_loadedFen;

		friend class MoveValidationAlgorithm;
		friend class MoveScope;
	};
}
