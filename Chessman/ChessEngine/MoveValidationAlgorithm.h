#pragma once
#include "EngineDefines.h"

#include <list>

namespace ChessEngine
{
	// forward declarations //
	class ChessBoardImpl;
	
	enum AttackType
	{
		AT_Transversal = 0,
		AT_Diagonal,
	};

	enum AttackResult
	{
		AR_Attacking = 0,
		AR_Blocking,
		AR_EmptyField,
	};

	class MoveValidationAlgorithm
	{
	public:
		MoveValidationAlgorithm( const ChessBoardImpl* pChessBoard );
		bool Run ( const MoveImpl& move, AdditionalMoveInfo& additionalInfo );
		std::list<CoordinateImpl>	GetAvailableMoves( const CoordinateImpl& coord );
		std::list<CoordinateImpl>	GetAttackingFields( const CoordinateImpl& coord, bool bWhiteAttacks );

	private:
		bool ValidatePawnMove	( const MoveImpl& move, const ChessPieceImpl& piece, AdditionalMoveInfo& additionalInfo );
		bool ValidateKnightMove	( const MoveImpl& move, const ChessPieceImpl& piece );
		bool ValidateBishopMove	( const MoveImpl& move, const ChessPieceImpl& piece );
		bool ValidateRockMove	( const MoveImpl& move, const ChessPieceImpl& piece );
		bool ValidateQueenMove	( const MoveImpl& move, const ChessPieceImpl& piece );
		bool ValidateKingMove	( const MoveImpl& move, const ChessPieceImpl& piece, AdditionalMoveInfo& additionalInfo  );
	
		bool IsFieldAvailable( const CoordinateImpl& coord );
		bool IsCaptureMove( const MoveImpl& move, const ChessPieceImpl& piece );
		AttackResult IsPieceAttacking( const CoordinateImpl& coord, bool bWhite, AttackType attackType );
		bool IsFieldAttacked( const CoordinateImpl& coord, bool bWhite, std::list<CoordinateImpl>& listAttackers );

	private:
		const ChessBoardImpl* m_pChessBoard;
	};
}
