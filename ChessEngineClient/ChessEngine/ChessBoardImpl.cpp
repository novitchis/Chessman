#include "pch.h"
#include "ChessBoardImpl.h"
#include "MoveValidationAlgorithm.h"

#include <sstream>
#include <ctype.h>
#include <iostream>
#include "Utils.h"

using namespace ChessEngine;

#define MAX_FEN_LENGTH		128

ChessBoardImpl::ChessBoardImpl(void)
	: m_bStorePGN ( false )
{

	Initialize();
}

ChessBoardImpl::ChessBoardImpl(const ChessBoardImpl& board)
{
	for (int i = 0; i < 8; i++)
		for (int j = 0; j < 8; j++)
		{
			m_memBoard[i][j] = ChessPieceImpl(board.m_memBoard[i][j]);
		}

	m_coordWhiteKing = board.m_coordWhiteKing;
	m_coordBlackKing = board.m_coordBlackKing;
	m_nCastlingMask = board.m_nCastlingMask;
	m_nLastPawnMoveOrCapture = board.m_nLastPawnMoveOrCapture;
	m_listMoves = board.m_listMoves;
	m_lastPiece = board.m_lastPiece;
	m_currentMoveIndex = board.m_currentMoveIndex;
	m_bStorePGN = board.m_bStorePGN;
}

ChessBoardImpl::~ChessBoardImpl(void)
{
}


std::string ChessBoardImpl::Serialize( SerializationType type )
{
	switch (type)
	{
	case ChessEngine::ST_FEN:
		return Serialize2FEN();
	case ChessEngine::ST_PGN:
		return Serialize2PGN();
	default:
		return "";
	}
}


bool ChessBoardImpl::LoadFrom( const std::string& strData, SerializationType type )
{
	switch (type)
	{
	case ChessEngine::ST_FEN: 
		return LoadFromFEN( strData );
	case ChessEngine::ST_PGN:
		return LoadFromPGN( strData );
	default:
		return "";
	}
}


void ChessBoardImpl::StorePGN()
{
	m_bStorePGN = true;
}


ChessPieceImpl ChessBoardImpl::GetPiece( const CoordinateImpl& coord ) const
{
	if ( !coord ) return ChessPieceImpl();
	return m_memBoard[coord.nRank][coord.nColumn];
}


void ChessBoardImpl::SetPiece( const ChessPieceImpl& piece, const CoordinateImpl& coord )
{
	m_memBoard[coord.nRank][coord.nColumn] = piece;
}

void ChessBoardImpl::SetSideToMove(bool bWhite)
{
	m_lastPiece.bWhite = !bWhite;
}

std::list<MoveDataImpl> ChessBoardImpl::GetMoves()
{
	return m_listMoves;
}

int ChessBoardImpl::GetCurrentMoveIndex()
{
	return m_currentMoveIndex;
}

std::list<MoveDataImpl>	ChessBoardImpl::GetVariationMoveData(std::list<MoveImpl> moves)
{
	std::list<MoveDataImpl> variationMoves;
	ChessBoardImpl copy(*this);
	copy.RemoveMovesAfterCurrent();

	for (auto it = moves.begin(); it != moves.end(); ++it)
	{
		if (!copy.SubmitMove(*it))
			throw std::invalid_argument("received invalid move");

		variationMoves.push_back(copy.m_listMoves.back());
	}

	return variationMoves;
}

std::list<CoordinateImpl> ChessBoardImpl::GetAvailableMoves( const CoordinateImpl& coord ) const
{
	ChessBoardImpl copy( *this );
	MoveValidationAlgorithm MoveAlg( &copy );
	return MoveAlg.GetAvailableMoves( coord );
}


std::string	ChessBoardImpl::GetLastMoveText()
{
	if ( m_currentMoveIndex ) return "";
	else return GetLastMove().strPGNUserFriendly;
}


bool ChessBoardImpl::SubmitMove( const MoveImpl& move, AdditionalMoveInfo& additionalInfo )
{
	if( !ValidateMove( move, additionalInfo ) ) return false;
	for ( auto& it : m_mapStates )
		it.second.OnStateChange();
	m_lastPiece = GetPiece( move.from );
	
	// update last piece or pawn move for FEN serialization //
	int nPrevLastPawnMoveOrCapture = m_nLastPawnMoveOrCapture;
	if ( ( m_lastPiece.cPiece == ChessPieceImpl::Pawn ) ||
		 ( GetPiece( move.to ) != ChessPieceImpl() ) ) m_nLastPawnMoveOrCapture = 0;
	else ++m_nLastPawnMoveOrCapture;

	auto capturedPiece = GetPiece( move.to );
	if (IsEnPassantMove(additionalInfo))
		capturedPiece = GetPiece(additionalInfo.coordEnPassant);

	// handle pawn promotion //
	bool bPromotion = ( m_lastPiece.cPiece == ChessPieceImpl::Pawn ) && ( ( move.to.nRank == 0 ) || ( move.to.nRank == 7 ) );
	if ( bPromotion )
	{
		m_lastPiece.cPiece = move.promotionPiece.cPiece;
		if ( m_lastPiece.cPiece == ChessPieceImpl::None )
			m_lastPiece.cPiece = ChessPieceImpl::Queen;

		m_lastPiece.bTransformed = true;
	}
	
	std::list<CoordinateImpl>	listAttackers;
	if ( m_bStorePGN )
		GetAttackingFields( move.to, m_lastPiece.bWhite, listAttackers );

	SetPiece( m_lastPiece, move.to );
	SetPiece( ChessPieceImpl(), move.from );

	if( additionalInfo.coordEnPassant ) 
		m_memBoard[ additionalInfo.coordEnPassant.nRank ][additionalInfo.coordEnPassant.nColumn] = ChessPieceImpl();
	if ( additionalInfo.RockMove )
	{
		SetPiece( GetPiece( additionalInfo.RockMove.from ), additionalInfo.RockMove.to );
		SetPiece( ChessPieceImpl(), additionalInfo.RockMove.from );
	}

	MoveDataImpl moveData( m_currentMoveIndex + 1, move, capturedPiece );
	moveData.enPassantCapture = IsEnPassantMove(additionalInfo);

	bool bKingCastle = false;
	bool bBQueenCastle = false;
	// update castling info //
	if ( m_lastPiece.bWhite ) 
	{
		if(	m_lastPiece.cPiece == ChessPieceImpl::King ) {
			if ( move.from.hDistance (move.to) == 2)
			{
				SetPiece(ChessPieceImpl(), CoordinateImpl(0,0));
				SetPiece(ChessPieceImpl(ChessPieceImpl::Rock, true), CoordinateImpl(0,3));
				bBQueenCastle = true;
			}
			else if ( move.from.hDistance (move.to) == -2)
			{
				SetPiece(ChessPieceImpl(), CoordinateImpl(0,7));
				SetPiece(ChessPieceImpl(ChessPieceImpl::Rock, true), CoordinateImpl(0,5));
				bKingCastle = true;
			}

			// for undo purposese moveData.nCastlingMask should have 0 bits
			// only for castling that was prevoiusly available and is not anymore after this move
			moveData.nCastlingMask &= ~(m_nCastlingMask & CT_WhiteQueenSide);
			moveData.nCastlingMask &= ~(m_nCastlingMask & CT_WhiteKingSide);
		}
	}
	else
	{
		if(	m_lastPiece.cPiece == ChessPieceImpl::King ) 
		{
			if ( move.from.hDistance (move.to) == 2)
			{
				SetPiece(ChessPieceImpl(), CoordinateImpl(7,0));
				SetPiece(ChessPieceImpl(ChessPieceImpl::Rock, false), CoordinateImpl(7,3));
				bBQueenCastle = true;
			}
			else if ( move.from.hDistance (move.to) == -2)
			{
				SetPiece(ChessPieceImpl(), CoordinateImpl(7,7));
				SetPiece(ChessPieceImpl(ChessPieceImpl::Rock, false), CoordinateImpl(7,5));
				bKingCastle = true;
			}

			// for undo purposese moveData.nCastlingMask should have 0 bits
			// only for castling that was prevoiusly available and is not anymore after this move
			moveData.nCastlingMask &= ~(m_nCastlingMask & CT_BlackKingSide);
			moveData.nCastlingMask &= ~(m_nCastlingMask & CT_BlackQueenSide);
		}
	}
	if ( m_lastPiece.bWhite )
	{
		if( ( m_lastPiece.cPiece == ChessPieceImpl::Rock ) && (move.from == CoordinateImpl( 0, 7 ) ) )
			moveData.nCastlingMask &= ~CT_WhiteKingSide;
		if( ( m_lastPiece.cPiece == ChessPieceImpl::Rock ) && (move.from == CoordinateImpl( 0, 0 ) ) )
			moveData.nCastlingMask &= ~CT_WhiteQueenSide;
	}
	else
	{
		if( ( m_lastPiece.cPiece == ChessPieceImpl::Rock ) && (move.from == CoordinateImpl( 7, 7 ) ) )
			moveData.nCastlingMask &= ~CT_BlackKingSide;
		if( ( m_lastPiece.cPiece == ChessPieceImpl::Rock ) && (move.from == CoordinateImpl( 7, 0 ) ) )
			moveData.nCastlingMask &= ~CT_BlackQueenSide;
	}

	m_nCastlingMask &= moveData.nCastlingMask;
	moveData.nLastPawnMoveOrCapture = nPrevLastPawnMoveOrCapture;
	if ( m_lastPiece.cPiece == ChessPieceImpl::King ) 
	{
		if ( m_lastPiece.bWhite ) m_coordWhiteKing = move.to;
		else m_coordBlackKing = move.to;
	}

	// update pgn str //
	if( m_bStorePGN )
	{
		if ( bKingCastle ) {
			moveData.strPGNMove = "O-O";
			moveData.strPGNUserFriendly = "King side castle";
		}
		else if ( bBQueenCastle ) {
			moveData.strPGNMove = "O-O-O";
			moveData.strPGNUserFriendly = "Queen side castle";
		}
		else {
			bool bAmbigous = false;
			CoordinateImpl coordAmbigous;
			for ( auto it : listAttackers )
			{
				if( GetPiece( it ).cPiece == m_lastPiece.cPiece ) {
					coordAmbigous = it;
					break;
				}
			}

			bool bPawn = (m_lastPiece.cPiece == ChessPieceImpl::Pawn) || bPromotion;
			if ( !bPawn )
			{
				moveData.strPGNMove += toupper( m_lastPiece.cPiece );
				moveData.strPGNUserFriendly = m_lastPiece.GetPieceStr();
			}
			else if ( !capturedPiece.IsEmpty() ) moveData.strPGNUserFriendly += 'a' + move.from.nColumn;

			if ( coordAmbigous && !bPawn ) moveData.strPGNUserFriendly += " from " + coordAmbigous.GetCoordStr();
			if ( ( coordAmbigous && !bPawn) || ( bPawn && !capturedPiece.IsEmpty() ) ) {
				// if column is the same, use column //
				if ( move.from.nColumn == coordAmbigous.nColumn )
					moveData.strPGNMove += move.from.nRank + L'1';
				else
					moveData.strPGNMove += move.from.nColumn + L'a';
			}
			if( !capturedPiece.IsEmpty() ) {
				moveData.strPGNMove += "x";
				moveData.strPGNUserFriendly += " takes ";
			}
			moveData.strPGNMove += move.to.ToString();
			moveData.strPGNUserFriendly += " ";
			moveData.strPGNUserFriendly += move.to.GetCoordStr();

			if( bPromotion ) {
				moveData.strPGNMove += "=";
				moveData.strPGNMove += toupper( m_lastPiece.cPiece );
				moveData.strPGNUserFriendly += " Promotes to " + m_lastPiece.GetPieceStr();
			}
			if ( IsMate() ) {
				moveData.strPGNMove += "#";
				moveData.strPGNUserFriendly += " check mate";
			}
			else if ( InCheck() ) {
				moveData.strPGNMove += "+";
				moveData.strPGNUserFriendly += " check";
			}
		}
	}

	if (m_listMoves.size() > m_currentMoveIndex + 1) {
		auto it = std::next(m_listMoves.begin(), m_currentMoveIndex + 1);
		if (it->strPGNMove != moveData.strPGNMove)
			RemoveMovesAfterCurrent();
	}

	if (m_listMoves.size() == m_currentMoveIndex + 1) {
		m_listMoves.push_back( moveData );
	}

	m_currentMoveIndex++;

	return true;
}

void ChessBoardImpl::RemoveMovesAfterCurrent()
{
	if (m_listMoves.size() > m_currentMoveIndex + 1) {
		auto it = std::next(m_listMoves.begin(), m_currentMoveIndex + 1);
		m_listMoves.erase(it, m_listMoves.end());
	}
}

bool ChessBoardImpl::SubmitMove( const MoveImpl& move )
{
	return SubmitMove( move, AdditionalMoveInfo() );
}


bool ChessBoardImpl::GoToMove( int moveIndex )
{
	if (moveIndex > (int)(m_listMoves.size() - 1))
		return false;

	// first handle go to first move
	if (m_currentMoveIndex < 0) 
	{
		auto it = m_listMoves.begin();
		SubmitMove(it->move);
		if (moveIndex == 0)
			return true;
	}

	if (moveIndex > m_currentMoveIndex)
	{
		auto it = std::next(m_listMoves.begin(), m_currentMoveIndex);
		for (int i = m_currentMoveIndex + 1; i <= moveIndex; ++i)
		{
			++it;
			SubmitMove(it->move);
		}
		return true;
	}
	else if (moveIndex < m_currentMoveIndex)
	{
		bool isWhiteMove = IsWhiteTurn();
		for (int i = m_currentMoveIndex; i > moveIndex; --i)
		{
			isWhiteMove = !isWhiteMove;
			UndoMove(isWhiteMove);
		}
		return true;
	}

	return false;
}


bool ChessBoardImpl::UndoMove( bool bWhiteMove )
{
	if ( m_currentMoveIndex < 0 ) return false;
	for ( auto& it : m_mapStates )
		it.second.OnStateChange();

	int nMovesToUndo = 0;
	if ( ( bWhiteMove != m_lastPiece.bWhite ) && m_currentMoveIndex >= 1 ) nMovesToUndo = 2;
	else nMovesToUndo = 1;

	while ( nMovesToUndo-- )
	{
		MoveDataImpl moveData = GetLastMove();
		m_nCastlingMask |= ~moveData.nCastlingMask;
		m_nCastlingMask &= 0xF;

		m_nLastPawnMoveOrCapture = moveData.nLastPawnMoveOrCapture;
		auto piece = GetPiece( moveData.move.to );
		if ( piece.bTransformed )
			piece.cPiece = ChessPieceImpl::Pawn;

		SetPiece( piece, moveData.move.from );
		CoordinateImpl capturedCoordinates = moveData.move.to;
		if (moveData.enPassantCapture)
		{
			capturedCoordinates = CoordinateImpl(moveData.move.from.nRank, moveData.move.to.nColumn);
			SetPiece(ChessPieceImpl(), moveData.move.to);
		}
		SetPiece( moveData.capturedPiece, capturedCoordinates);
		m_lastPiece = piece;
		if ( m_lastPiece.cPiece == ChessPieceImpl::King ) 
		{
			if ( m_lastPiece.bWhite ) m_coordWhiteKing = moveData.move.from;
			else m_coordBlackKing = moveData.move.from;
			if ( abs( moveData.move.to.nColumn - moveData.move.from.nColumn ) == 2 )
			{
				// casling move. move the rock also //
				auto coordRock = CoordinateImpl( moveData.move.from.nRank, (moveData.move.to.nColumn + moveData.move.from.nColumn)/2 );
				auto prospectedRock = GetPiece( coordRock );
				CoordinateImpl coordRockDest;

				coordRockDest.nRank = coordRock.nRank;
				coordRockDest.nColumn = coordRock.nColumn < 4 ? 0 : 7;
				_ASSERTE( prospectedRock.cPiece == ChessPieceImpl::Rock );
				SetPiece( prospectedRock, coordRockDest );
				SetPiece( ChessPieceImpl(), coordRock );
			}
		}
		m_currentMoveIndex--;
	}

	// update last piece //
	if( m_currentMoveIndex < 0 )
	{
		m_lastPiece.bWhite = !m_lastPiece.bWhite;
		m_lastPiece.cPiece = ChessPieceImpl::None;
	}
	else
	{
		MoveDataImpl moveData = GetLastMove();
		m_lastPiece = GetPiece( moveData.move.to );
	}
	return true;
}

bool ChessBoardImpl::IsEnPassantMove(AdditionalMoveInfo& additionalMove) const
{
	return additionalMove.coordEnPassant &&
		additionalMove.coordEnPassant.nColumn != -1 &&
		additionalMove.coordEnPassant.nRank != -1;
}

bool ChessBoardImpl::ValidateMove( const MoveImpl& move, AdditionalMoveInfo& additionalMove ) const
{
	ChessPieceImpl piece = GetPiece( move.from );
	if ( ( piece.IsEmpty() ) || piece.bWhite == m_lastPiece.bWhite ) return false;

	ChessBoardImpl copy(*this);
	MoveValidationAlgorithm MoveAlg( &copy );
	return MoveAlg.Run( move, additionalMove );
}

bool ChessBoardImpl::PromotePawn( CoordinateImpl coord, ChessPieceImpl piece )
{
	if (GetPiece(coord).cPiece != ChessPieceImpl::Pawn)
		return false;

	SetPiece(piece, coord);
	return true;
}


bool ChessBoardImpl::GetAttackingFields( const CoordinateImpl& coord, bool bWhiteAttacks, std::list<CoordinateImpl>& listAttacker ) const
{
	MoveValidationAlgorithm MoveAlg( this );
	listAttacker = MoveAlg.GetAttackingFields( coord, bWhiteAttacks );
	return !listAttacker.empty();
}


CoordinateImpl ChessBoardImpl::GetKingPos( bool bWhite ) const
{
	if ( bWhite ) return m_coordWhiteKing;
	else return m_coordBlackKing;
}


bool ChessBoardImpl::IsWhiteTurn() const
{
	return !m_lastPiece.bWhite;
}


bool ChessBoardImpl::InCheck()
{
	Core::StatePreserver state;
	GetPreservedState( SPT_InCheck, state );
	if ( !state.HasChanged() ) return state.GetState();

	std::list<CoordinateImpl>	listAttackers;
	auto result = GetAttackingFields( GetKingPos( !m_lastPiece.bWhite ), m_lastPiece.bWhite, listAttackers );
	UpdateState( SPT_InCheck, result );
	return result;
}


bool ChessBoardImpl::IsMate()
{
	Core::StatePreserver state;
	GetPreservedState( SPT_IsMate, state );
	if ( !state.HasChanged() ) return state.GetState();

	bool bRes = true;
	if ( !InCheck() ) 
	{
		bRes = false;
		goto END; 
	}
	if ( !GetAvailableMoves( GetKingPos( !m_lastPiece.bWhite) ).empty() )
	{
		bRes = false;
		goto END; 
	}

	for ( int i = 0; i < 8; ++i )
		for ( int j = 0; j < 8; ++j )
		{
			auto crtCoord = CoordinateImpl( i, j ) ;
			auto piece = GetPiece( crtCoord );
			if ( piece.IsEmpty() ) continue;
			if ( piece.bWhite == m_lastPiece.bWhite ) continue;
			if ( !GetAvailableMoves( crtCoord ).empty() ) 
			{
				bRes = false;
				goto END; 
			}
		}

END:
	UpdateState( SPT_IsMate, bRes );
	return bRes;
}


bool ChessBoardImpl::IsStaleMate()
{
	Core::StatePreserver state;
	GetPreservedState( SPT_IsStaleMate, state );
	if ( !state.HasChanged() ) return state.GetState();

	bool bRes = true;

	if ( InCheck() ) 
	{
		bRes = false;
		goto END;
	}

	int nPieceCount = 0;
	for ( int i = 0; i < 8; ++i )
		for ( int j = 0; j < 8; ++j )
		{
			if ( GetPiece( CoordinateImpl(i, j ) ) != ChessPieceImpl() ) ++nPieceCount;
			if( nPieceCount > 2 ) break;
		}
	if ( nPieceCount == 2 ) return true;

	for ( int i = 0; i < 8; ++i )
		for ( int j = 0; j < 8; ++j )
		{
			auto crtCoord = CoordinateImpl( i, j ) ;
			auto piece = GetPiece( crtCoord );
			if ( piece.IsEmpty() ) continue;
			if ( piece.bWhite == m_lastPiece.bWhite ) continue;
			if ( !GetAvailableMoves( crtCoord ).empty() ) 
			{
				bRes = false;
				goto END;
			}
		}
END:
	UpdateState( SPT_IsStaleMate, bRes );
	return bRes;
}


bool ChessBoardImpl::IsValid()
{
	int whiteKingCount = 0;
	int blackKingCount = 0;

	for (int i = 0; i < 8; ++i)
	{
		for (int j = 0; j < 8; ++j)
		{
			ChessPieceImpl piece = GetPiece(CoordinateImpl(i, j));
			if (piece.cPiece == ChessPieceImpl::King)
			{
				if (piece.bWhite)
					whiteKingCount += 1;
				else
					blackKingCount += 1;
			}
		}
	}

	if (whiteKingCount != 1 || blackKingCount != 1)
		return false;

	// check if the side which is NOT currently to move is in check
	std::list<CoordinateImpl> listAttackers;
	return !GetAttackingFields(GetKingPos(!IsWhiteTurn()), IsWhiteTurn(), listAttackers);
}


MoveDataImpl ChessBoardImpl::GetLastMove() const
{
	if ( m_currentMoveIndex < 0 ) return MoveDataImpl();
	auto it = std::next(m_listMoves.begin(), m_currentMoveIndex);
	return *it;
}


std::map<ChessPieceImpl, int> ChessBoardImpl::GetCapturedPieces() const
{
	std::map<ChessPieceImpl, int>  mapResult;
	
	mapResult[ChessPieceImpl(ChessPieceImpl::Pawn,		true) ] = 8;
	mapResult[ChessPieceImpl(ChessPieceImpl::Knight,	true) ] = 2;
	mapResult[ChessPieceImpl(ChessPieceImpl::Bishop,	true) ] = 2;
	mapResult[ChessPieceImpl(ChessPieceImpl::Rock,		true) ] = 2;
	mapResult[ChessPieceImpl(ChessPieceImpl::Queen,		true) ] = 1;

	mapResult[ChessPieceImpl(ChessPieceImpl::Pawn,		false) ] = 8;
	mapResult[ChessPieceImpl(ChessPieceImpl::Knight,	false) ] = 2;
	mapResult[ChessPieceImpl(ChessPieceImpl::Bishop,	false) ] = 2;
	mapResult[ChessPieceImpl(ChessPieceImpl::Rock,		false) ] = 2;
	mapResult[ChessPieceImpl(ChessPieceImpl::Queen,		false) ] = 1;

	for ( int i = 0; i < 8; ++i )
		for ( int j = 0; j < 8; ++j )
		{
			auto piece = GetPiece( CoordinateImpl( i, j ) );
			if ( ( piece.IsEmpty() ) || ( piece.cPiece == ChessPieceImpl::King )) continue;
			if ( piece.bTransformed )
				mapResult[ChessPieceImpl( ChessPieceImpl::Pawn, piece.bWhite) ] --;
			else 
				mapResult[piece] --;
		}

	return mapResult;
}


void ChessBoardImpl::GetPreservedState( StatePreserveType type, Core::StatePreserver& state )
{
	state = m_mapStates[type];
}

void ChessBoardImpl::UpdateState( StatePreserveType type, const Core::Variant& vtState )
{
	m_mapStates[type].SetState( vtState );
}

void ChessBoardImpl::Clear()
{
	m_listMoves.clear();
	m_currentMoveIndex = -1;
	// generate empty chess board //
	for ( int i = 0; i < 8; ++i )
		for ( int j = 0; j < 8; ++j )
			m_memBoard[ i ][ j ] = ChessPieceImpl();
}

void ChessBoardImpl::Initialize()
{
	Clear();

	// generate pawns //
	for ( int i = 0; i < 8; ++i ) 
	{
		m_memBoard[ 1 ][ i ] = ChessPieceImpl( ChessPieceImpl::Pawn, true );
		m_memBoard[ 6 ][ i ] = ChessPieceImpl( ChessPieceImpl::Pawn, false );
	}

	// rocks //
	m_memBoard[ 0 ][ 0 ] = m_memBoard[ 0 ][ 7 ] = ChessPieceImpl( ChessPieceImpl::Rock, true );
	m_memBoard[ 7 ][ 0 ] = m_memBoard[ 7 ][ 7 ] = ChessPieceImpl( ChessPieceImpl::Rock, false );
				 		   	      				   					  
	// knights  //		   	      				   					  
	m_memBoard[ 0 ][ 1 ] = m_memBoard[ 0 ][ 6 ] = ChessPieceImpl( ChessPieceImpl::Knight, true );
	m_memBoard[ 7 ][ 1 ] = m_memBoard[ 7 ][ 6 ] = ChessPieceImpl( ChessPieceImpl::Knight, false );
				 		   	      				   					  
	// Bishops  //		   	      				   					  
	m_memBoard[ 0 ][ 2 ] = m_memBoard[ 0 ][ 5 ] = ChessPieceImpl( ChessPieceImpl::Bishop, true );
	m_memBoard[ 7 ][ 2 ] = m_memBoard[ 7 ][ 5 ] = ChessPieceImpl( ChessPieceImpl::Bishop, false );

	// Queens //
	m_memBoard[ 0 ][3 ] = ChessPieceImpl( ChessPieceImpl::Queen, true );
	m_memBoard[ 7 ][3 ] = ChessPieceImpl( ChessPieceImpl::Queen, false );

	// Kings //
	m_memBoard[ 0 ][ 4 ] = ChessPieceImpl( ChessPieceImpl::King, true );
	m_memBoard[ 7 ][ 4 ] = ChessPieceImpl( ChessPieceImpl::King, false );
	
	m_coordWhiteKing = CoordinateImpl (0, 4);
	m_coordBlackKing = CoordinateImpl (7, 4);
	m_nCastlingMask = FullCastlingMask;
	m_lastPiece.bWhite = false;
	m_nLastPawnMoveOrCapture = 0;
}

 // refer to http://en.wikipedia.org/wiki/Forsyth%E2%80%93Edwards_Notation //
std::string ChessBoardImpl::Serialize2FEN() const
{
	std::string strFEN;
	strFEN.reserve( MAX_FEN_LENGTH );

	for ( int i = 7; i >= 0; --i ) 
	{
		int nOpenFields = 0;
		for ( int j = 0; j < 8; ++j ) 
		{
			auto crtPiece = GetPiece( CoordinateImpl( i, j ) );
			if ( crtPiece != ChessPieceImpl() ) 
			{
				if ( nOpenFields ) 
				{
					strFEN += std::to_string( nOpenFields );
					nOpenFields = 0;
				}
				strFEN += crtPiece.bWhite ? toupper( crtPiece.cPiece ) : crtPiece.cPiece;
			}
			else 
				++nOpenFields;
		}
		
		if ( nOpenFields ) strFEN += std::to_string( nOpenFields );
		if ( i > 0 ) strFEN += '/'; // end rank
	}

	// nextmove //
	strFEN += ' ';
	strFEN += m_lastPiece.bWhite ? 'b' : 'w';
	strFEN += ' ';
	// add castling informations //
	if ( ( m_nCastlingMask & 0xF ) == 0 )
		strFEN += "-";
	else
	{
		if ( m_nCastlingMask & CT_WhiteKingSide ) strFEN += L'K';
		if ( m_nCastlingMask & CT_WhiteQueenSide ) strFEN += L'Q';
		if ( m_nCastlingMask & CT_BlackKingSide ) strFEN += L'k';
		if ( m_nCastlingMask & CT_BlackQueenSide ) strFEN += L'q';
	}
	strFEN += " ";

	// add en-passant possibilities //
	std::string strEnPassant = "-";
	if ( ( m_lastPiece.cPiece == ChessPieceImpl::Pawn ) && m_currentMoveIndex >= 0 )
	{
		auto lastMove = GetLastMove().move;
		if ( abs( lastMove.from.nRank - lastMove.to.nRank ) == 2 )
			strEnPassant = CoordinateImpl( (lastMove.from.nRank + lastMove.to.nRank ) / 2, lastMove.from.nColumn ).ToString();
	}
	strFEN += strEnPassant;
	strFEN += " ";
	// last capture or pawn advance //
	strFEN += std::to_string( m_nLastPawnMoveOrCapture );
	strFEN += " ";
	// move count //
	strFEN += std::to_string( (m_currentMoveIndex + 1) / 2 + 1 );
	
	return strFEN;
}

std::list<CoordinateImpl> ChessBoardImpl::getPiecesMovableAs(MoveImpl move, char pieceType, bool pWhite) const
{
	std::list<CoordinateImpl> result;

	if (pieceType == ChessPieceImpl::None)
		return result;

	ChessBoardImpl mockBoard(*this);

	MoveValidationAlgorithm moveChecker(&mockBoard);
	CoordinateImpl coord = move.to;

	for (int r = 0; r < 8; ++r)
		for (int c = 0; c < 8; ++c)
		{
			CoordinateImpl candidate(r,c);
			if (move.from == candidate)
				continue;

			ChessPieceImpl candidatePiece = mockBoard.m_memBoard[r][c];

			if (candidatePiece.cPiece == pieceType && candidatePiece.bWhite == pWhite) 
			{
				std::list<CoordinateImpl> possibleMoves = moveChecker.GetAvailableMoves(candidate);
				for (auto it = possibleMoves.begin(); it != possibleMoves.end(); ++it)
				{
					// TODO check this -> seems == no work
					if (*it == coord)
						result.push_back(candidate);
				}
			}
		}

//	std::cout << std::endl << "possible moves to " << coord.ToString() << " are:" << std::endl;
	// TODO remove this after I'm sure this works properly
//	for (auto it = result.begin(); it != result.end(); ++it)
//		std::cout << it->ToString() << std::endl;

	return result;
}

std::string ChessBoardImpl::Serialize2PGN()
{
	Core::StatePreserver state;
	GetPreservedState( SPT_PGNSerialization, state );
	if ( !state.HasChanged() ) return state.GetState();
	
	std::string result = "";
	int moveNo = 0;

	int nWhiteMaxMoveLen = 0;
	int nBlackMaxMoveLen = 0;
	for ( auto it = m_listMoves.begin(); it != m_listMoves.end(); ++it )
	{
		if (moveNo > m_currentMoveIndex) 
			break;

		if( moveNo % 2 )
		{
			if ( nWhiteMaxMoveLen < it->strPGNMove.size() ) 
				nWhiteMaxMoveLen = (int)it->strPGNMove.size();
		}
		else
		{
			if ( nBlackMaxMoveLen < it->strPGNMove.size() ) 
				nBlackMaxMoveLen = (int)it->strPGNMove.size();
		}
		++moveNo;
	}

	nWhiteMaxMoveLen ++;
	nBlackMaxMoveLen ++;

	moveNo = 0;
	for ( auto it = m_listMoves.begin(); it != m_listMoves.end(); ++it )
	{
		if (moveNo > m_currentMoveIndex)
			break;

		if ( moveNo % 2 == 0 )
		{
			result += "\n";
			result += std::to_string(moveNo / 2 + 1) + ". ";
		}
		
		std::string movestr = it->strPGNMove;

		// max move length: 7 e.g. : dxe8=Q+ ( pawn from d7 takes at e8, promoting to queen and giving check ) //
		int nMaxMoveSize = (moveNo%2) ? nWhiteMaxMoveLen : nBlackMaxMoveLen;
		while ( movestr.size() < nMaxMoveSize ) 
			movestr += " ";
		result += movestr;
		
		++ moveNo;
	}

	UpdateState( SPT_PGNSerialization, result );
	return result;
}


bool ChessBoardImpl::LoadFromFEN( const std::string& strData )
{
	std::string strToken;
	std::istringstream stm( strData );
	std::getline( stm, strToken, ' ' );

	Clear();

	// parse pieces
	int nCrtRank = 7;
	int nCrtColumn = 0;
	for ( auto i = 0; i < strToken.size(); ++i )
	{
		auto crtChar = strToken[i];
		if ( IN_RANGE( crtChar, '1', '9' ) ) nCrtColumn += crtChar - '0'; // empty squares //
		if ( IN_RANGE( crtChar, 'a', 'z' ) ) 
		{
			CoordinateImpl coord(nCrtRank, nCrtColumn);
			SetPiece( ChessPieceImpl( tolower( crtChar ), false ), coord );
			if (crtChar == 'k')
				m_coordBlackKing = coord;

			nCrtColumn++;
		}
		if ( IN_RANGE( crtChar, 'A', 'Z' ) ) 
		{
			CoordinateImpl coord(nCrtRank, nCrtColumn);
			SetPiece( ChessPieceImpl( tolower( crtChar ), true ), coord );
			if (crtChar == 'K')
				m_coordWhiteKing = coord;

			nCrtColumn++;
		}
		if ( crtChar == '/' ) 
		{
			--nCrtRank;
			nCrtColumn = 0;
		}
	}

	// current color//
	std::getline( stm, strToken, ' ' );
	m_lastPiece.bWhite = ( strToken[0] == 'b' );

	// castling info //
	std::getline( stm, strToken, ' ' );
	m_nCastlingMask = 0;
	if ( strToken != "-" )
	{
		if ( strToken.find( 'K' ) != -1 ) m_nCastlingMask |= CT_WhiteKingSide; 
		if ( strToken.find( 'Q' ) != -1 ) m_nCastlingMask |= CT_WhiteQueenSide; 
		if ( strToken.find( 'k' ) != -1 ) m_nCastlingMask |= CT_BlackKingSide; 
		if ( strToken.find( 'q' ) != -1 ) m_nCastlingMask |= CT_BlackQueenSide; 
	}

	// TODO: load the rest //
	return false;
}

bool ChessBoardImpl::LoadFromPGN( const std::string& strData )
{
	std::string strToken;
	std::istringstream stm( strData );
	std::string delims = " \n\t";

	auto tokens = split(strData, delims);

	for (auto it = tokens.begin(); it != tokens.end(); ++it)
	{
		MoveImpl move;

		std::string strToken = *it;
		if (strToken.find('.') != std::string::npos)
			continue;

		if (strToken.find("O-O-O") != std::string::npos)
		{
			// queen-side castling
			if (IsWhiteTurn())
				SubmitMove(MoveImpl(CoordinateImpl(0,4), CoordinateImpl(0,2)));
			else
				SubmitMove(MoveImpl(CoordinateImpl(7,4), CoordinateImpl(7,2)));
			continue;
		} 
		else if (strToken.find("O-O") != std::string::npos)
		{
			// king-side castling
			if (IsWhiteTurn())
				SubmitMove(MoveImpl(CoordinateImpl(0,4), CoordinateImpl(0,6)));
			else
				SubmitMove(MoveImpl(CoordinateImpl(7,4), CoordinateImpl(7,6)));
			continue;
		}
		
		std::string destString;

		if (strToken.find('=') != std::string::npos)
		{
			auto index = strToken.find('=');
			char destPiece = strToken[index+1];
			destString = strToken.substr(index-2, index);

			move.promotionPiece = ChessPieceImpl(tolower(destPiece), IsWhiteTurn());
		}
		else if (strToken[strToken.length() - 1] == '+' || strToken[strToken.length() - 1] == '#')
		{
			destString = strToken.substr(strToken.length() - 3, strToken.length() - 2);
		}
		else 
		{
			destString = strToken.substr(strToken.length() - 2);
		}

		CoordinateImpl destination = CoordinateImpl::FromString(destString);
		char pieceType = ChessPieceImpl::None;
		int moveIndex = 0;

		if (std::string("KQRBNP").find(strToken[0]) == std::string::npos)
		{
			// pawn move
			pieceType = ChessPieceImpl::Pawn;
		}
		else 
		{
			pieceType = tolower(strToken[0]);
			moveIndex = 1;
		}
		
		move.to = destination;
		std::list<CoordinateImpl> possibleMoves = getPiecesMovableAs(move, pieceType, IsWhiteTurn());

		if (possibleMoves.empty())
		{
			std::cout << "error:no possible moves to " <<
				destination.ToString() << " as " << pieceType << std::endl;

			return false;
		}

		if (possibleMoves.size() > 1) 
		{
			//disambiguation
			if (strToken.length() < 3)
			{
				std::cout << "disambiguation error, move string too short" << std::endl;
				return false;
			}

			if (IN_RANGE(strToken[moveIndex], 'a', 'h'))
			{
				// disambiguation by file/column
				auto itNewEnd = std::remove_if(possibleMoves.begin(), possibleMoves.end(),
					[=] (const CoordinateImpl& coord) {
						return coord.nColumn != strToken[moveIndex] - 'a';
					});
				possibleMoves.erase( itNewEnd, possibleMoves.end() );
				moveIndex ++;
			}
			
			if (possibleMoves.size() > 1 || (IN_RANGE(strToken[moveIndex], '1', '8')))
			{
				// disambiguation by rank
				// if column disambiguation was needed and failed => complete source tile needed
				auto itNewEnd = std::remove_if(possibleMoves.begin(), possibleMoves.end(),
					[=] (const CoordinateImpl& coord) {
						return coord.nRank == strToken[moveIndex] - '1';
					});
				possibleMoves.erase( itNewEnd, possibleMoves.end() );
			}
		}
		
		if (possibleMoves.size() != 1)
		{
			// goto hell, bug!
			std::cout << "something buggy at disambiguation, please check." << std::endl;
			return false;
		}

		move.from = possibleMoves.front();

		// don't really care what the move does, just do it
		SubmitMove(move);
	}

	return true;
}

