#include "pch.h"
#include "MoveValidationAlgorithm.h"
#include "ChessBoardImpl.h"
#include "Utils.h"
#include "MoveScope.h"

#include <algorithm>

using namespace ChessEngine;

#define IS_PIECE_ATTACKING(rank,col,bWhite,listAttackers,atType) \
	auto _coord = CoordinateImpl( rank, col );\
	auto atRes = IsPieceAttacking( _coord, bWhite, atType );\
	if ( atRes == AR_Blocking ) break;\
	if( atRes == AR_Attacking ) {listAttackers.push_back(_coord);break;};

MoveValidationAlgorithm::MoveValidationAlgorithm(const ChessBoardImpl* pChessBoard)
	: m_pChessBoard(pChessBoard)
{
}

int g_nCount = 0;

bool MoveValidationAlgorithm::Run(const MoveImpl& move, AdditionalMoveInfo& additionalInfo)
{
	if (!move) return false;
	if (move.from == move.to) return false;
	ChessPieceImpl piece = m_pChessBoard->GetPiece(move.from);
	if (!IsFieldAvailable(move.to) && !IsCaptureMove(move, piece)) return false;
	// kings cannot be captured! //
	if (m_pChessBoard->GetKingPos(!piece.bWhite) == move.to) return false;

	bool bValidMove = false;
	switch (piece.cPiece)
	{
	case ChessPieceImpl::Pawn:
		bValidMove = ValidatePawnMove(move, piece, additionalInfo);
		break;
	case ChessPieceImpl::Knight:
		bValidMove = ValidateKnightMove(move, piece);
		break;
	case ChessPieceImpl::Bishop:
		bValidMove = ValidateBishopMove(move, piece);
		break;
	case ChessPieceImpl::Rock:
		bValidMove = ValidateRockMove(move, piece);
		break;
	case ChessPieceImpl::Queen:
		bValidMove = ValidateQueenMove(move, piece);
		break;
	case ChessPieceImpl::King:
		bValidMove = ValidateKingMove(move, piece, additionalInfo);
		break;
	}

	if (!bValidMove) return false;

	// verify check possibilities //
	{
		MoveScope moveScope(const_cast<ChessBoardImpl*>(m_pChessBoard), move, additionalInfo);
		CoordinateImpl coordKing;
		if (piece.cPiece == ChessPieceImpl::King)
			coordKing = move.to;
		else
			coordKing = m_pChessBoard->GetKingPos(piece.bWhite);

		auto listAttackers = GetAttackingFields(coordKing, !piece.bWhite);
		if (!listAttackers.empty())
			bValidMove = false;
	}

	return bValidMove;
}

// dummy implementation. May be optimized //
std::list<CoordinateImpl>	MoveValidationAlgorithm::GetAvailableMoves(const CoordinateImpl& coord)
{
	std::list<CoordinateImpl> listResult;

	for (int i = 0; i < 8; ++i)
		for (int j = 0; j < 8; ++j)
			listResult.push_back(CoordinateImpl(i, j));

	auto itNewEnd = std::remove_if(listResult.begin(), listResult.end(),
		[=](const CoordinateImpl& crtCoord)
	{
		AdditionalMoveInfo additionalInfo;
		return !Run(MoveImpl(coord, crtCoord), additionalInfo);
	});

	listResult.erase(itNewEnd, listResult.end());
	return listResult;
}


std::list<CoordinateImpl> MoveValidationAlgorithm::GetAttackingFields(const CoordinateImpl& coord, bool bWhiteAttacks)
{
	std::list<CoordinateImpl>		listAttackers;
	auto piece = m_pChessBoard->GetPiece(coord);
	bool bWhite = !bWhiteAttacks;
	if (piece != ChessPieceImpl()) bWhite = piece.bWhite;
	IsFieldAttacked(coord, bWhite, listAttackers);
	return listAttackers;
}


bool MoveValidationAlgorithm::ValidatePawnMove(const MoveImpl& move, const ChessPieceImpl& piece, AdditionalMoveInfo& additionalInfo)
{
	if (piece.bWhite && (move.from.nRank >= move.to.nRank)) return false;
	if (!piece.bWhite && (move.from.nRank <= move.to.nRank)) return false;

	if (abs(move.to.nRank - move.from.nRank) > 2) return false;
	if (move.to.nColumn == move.from.nColumn)
	{
		if (!IsFieldAvailable(move.to)) return false;
		int nFieldCount = abs(move.to.nRank - move.from.nRank);
		if (nFieldCount == 1) return true;
		CoordinateImpl coordIntermediare((move.from.nRank + move.to.nRank) / 2, move.to.nColumn);
		if (!IsFieldAvailable(coordIntermediare)) return false;
		if (piece.bWhite && (move.from.nRank != 1)) return false;
		if (!piece.bWhite && (move.from.nRank != 6)) return false;
	}
	else
	{
		// capture move //
		if (abs(move.to.nRank - move.from.nRank) != 1) return false;
		if (abs(move.to.nColumn - move.from.nColumn) != 1) return false;

		ChessPieceImpl pieceTo = m_pChessBoard->GetPiece(move.to);
		bool bFieldAvailable = pieceTo.IsEmpty();
		if (!bFieldAvailable && (pieceTo.bWhite != piece.bWhite))
			return true;
		else if (bFieldAvailable)
		{
			if (m_pChessBoard->m_listMoves.empty()) return false;
			// check en passant //
			MoveImpl prevMove = m_pChessBoard->GetLastMove().move;
			ChessPieceImpl prevPiece = m_pChessBoard->GetPiece(prevMove.to);
			if (prevPiece.cPiece != ChessPieceImpl::Pawn) return false;
			if (abs(prevMove.from.nRank - prevMove.to.nRank) != 2) return false;
			if (move.from.nRank != (piece.bWhite ? 4 : 3)) return false;
			if (move.to.nColumn != prevMove.to.nColumn) return false;
			if (move.to.nRank != prevMove.to.nRank + (piece.bWhite ? 1 : (-1))) return false;
			additionalInfo.coordEnPassant = prevMove.to;
			additionalInfo.type = MI_EnPassant;
		}
	}
	return true;
}


bool MoveValidationAlgorithm::ValidateKnightMove(const MoveImpl& move, const ChessPieceImpl& piece)
{
	if (!IsFieldAvailable(move.to) && !IsCaptureMove(move, piece)) return false;
	int nRankDif = abs(move.from.nRank - move.to.nRank);
	int nColDif = abs(move.from.nColumn - move.to.nColumn);

	if (nRankDif + nColDif != 3) return false;
	if (abs(nRankDif - nColDif) != 1) return false;
	return true;
}


bool MoveValidationAlgorithm::ValidateBishopMove(const MoveImpl& move, const ChessPieceImpl& piece)
{
	if (abs(move.from.nRank - move.to.nRank) != abs(move.from.nColumn - move.to.nColumn)) return false;
	int nRankInc = sgn(move.to.nRank - move.from.nRank);
	int nColumnInc = sgn(move.to.nColumn - move.from.nColumn);

	CoordinateImpl crtCoord = move.from;
	crtCoord.nRank += nRankInc;
	crtCoord.nColumn += nColumnInc;

	while (crtCoord != move.to)
	{
		if (!IsFieldAvailable(crtCoord)) return false;
		crtCoord.nRank += nRankInc;
		crtCoord.nColumn += nColumnInc;
	}

	return true;
}


bool MoveValidationAlgorithm::ValidateRockMove(const MoveImpl& move, const ChessPieceImpl& piece)
{
	if ((move.from.nRank != move.to.nRank) && (move.from.nColumn != move.to.nColumn)) return false;

	if (move.from.nRank == move.to.nRank)
	{
		CoordinateImpl crtCoord = move.from;
		int nColumnInc = sgn(move.to.nColumn - move.from.nColumn);
		crtCoord.nColumn += nColumnInc;

		while (crtCoord != move.to)
		{
			if (!IsFieldAvailable(crtCoord)) return false;
			crtCoord.nColumn += nColumnInc;
		}
	}
	else
	{
		CoordinateImpl crtCoord = move.from;
		int nRankInc = sgn(move.to.nRank - move.from.nRank);
		crtCoord.nRank += nRankInc;

		while (crtCoord != move.to)
		{
			if (!IsFieldAvailable(crtCoord)) return false;
			crtCoord.nRank += nRankInc;
		}
	}

	return true;
}


bool MoveValidationAlgorithm::ValidateQueenMove(const MoveImpl& move, const ChessPieceImpl& piece)
{
	return ValidateRockMove(move, piece) || ValidateBishopMove(move, piece);
}


// TODO: handle check validation & castle possibilities //
bool MoveValidationAlgorithm::ValidateKingMove(const MoveImpl& move, const ChessPieceImpl& piece, AdditionalMoveInfo& additionalInfo)
{
	auto nColDif = abs(move.from.nColumn - move.to.nColumn);
	auto nRankDif = abs(move.from.nRank - move.to.nRank);
	if ((nRankDif > 1) || (nColDif > 2)) return false;

	if (nColDif <= 1) return true;
	if (nRankDif != 0) return false;

	// handle castle //
	CoordinateImpl currentKingCoord = move.from;
	auto listAttackers = GetAttackingFields(currentKingCoord, !piece.bWhite);
	if (!listAttackers.empty())
		return false;

	bool bColumnIncrease = move.to.nColumn > move.from.nColumn;
	for (auto i = __min(move.to.nColumn, move.from.nColumn); i <= __max(move.to.nColumn, move.from.nColumn); ++i)
	{
		if (i != move.from.nColumn)
		{
			CoordinateImpl squareCoordinate = CoordinateImpl(move.to.nRank, i);
			if (!IsFieldAvailable(squareCoordinate) || !GetAttackingFields(squareCoordinate, !piece.bWhite).empty())
				return false;
		}
	}

	if (piece.bWhite)
	{
		if (bColumnIncrease && !(m_pChessBoard->m_nCastlingMask & CT_WhiteKingSide)) return false;
		if (!bColumnIncrease && !(m_pChessBoard->m_nCastlingMask & CT_WhiteQueenSide)) return false;
	}
	else
	{
		if (bColumnIncrease && !(m_pChessBoard->m_nCastlingMask & CT_BlackKingSide)) return false;
		if (!bColumnIncrease && !(m_pChessBoard->m_nCastlingMask & CT_BlackQueenSide)) return false;
	}

	CoordinateImpl from(piece.bWhite ? 0 : 7, bColumnIncrease ? 7 : 0);
	CoordinateImpl to(piece.bWhite ? 0 : 7, bColumnIncrease ? 5 : 3);
	additionalInfo.RockMove = MoveImpl(from, to);
	additionalInfo.type = MI_Castle;

	return true;
}


bool MoveValidationAlgorithm::IsFieldAvailable(const CoordinateImpl& coord)
{
	return m_pChessBoard->GetPiece(coord).IsEmpty();
}


bool MoveValidationAlgorithm::IsCaptureMove(const MoveImpl& move, const ChessPieceImpl& piece)
{
	ChessPieceImpl pieceFrom = m_pChessBoard->GetPiece(move.from);
	ChessPieceImpl pieceTo = m_pChessBoard->GetPiece(move.to);
	return !IsFieldAvailable(move.to) && (pieceTo.bWhite != pieceFrom.bWhite);
}


AttackResult MoveValidationAlgorithm::IsPieceAttacking(const CoordinateImpl& coord, bool bWhite, AttackType attackType)
{
	auto piece = m_pChessBoard->GetPiece(coord);
	if (piece.IsEmpty()) return AR_EmptyField;
	bool bAttacks = false;
	switch (attackType)
	{
	case AT_Diagonal:
		bAttacks = piece.AttacksDiagonally();
		break;
	case AT_Transversal:
		bAttacks = piece.AttacksTransversally();
		break;
	}
	if (bAttacks && (piece.bWhite != bWhite)) return AR_Attacking;
	return AR_Blocking;
}


bool MoveValidationAlgorithm::IsFieldAttacked(const CoordinateImpl& coord, bool bWhite, std::list<CoordinateImpl>& listAttackers)
{
	// rank //
	for (int i = coord.nColumn + 1; i < 8; ++i)
	{
		IS_PIECE_ATTACKING(coord.nRank, i, bWhite, listAttackers, AT_Transversal);
	}
	for (int i = coord.nColumn - 1; i >= 0; --i)
	{
		IS_PIECE_ATTACKING(coord.nRank, i, bWhite, listAttackers, AT_Transversal);
	}

	// column //
	for (int i = coord.nRank + 1; i < 8; ++i)
	{
		IS_PIECE_ATTACKING(i, coord.nColumn, bWhite, listAttackers, AT_Transversal);
	}
	for (int i = coord.nRank - 1; i >= 0; --i)
	{
		IS_PIECE_ATTACKING(i, coord.nColumn, bWhite, listAttackers, AT_Transversal);
	}

	// diagonals //
	for (int i = coord.nRank + 1, j = coord.nColumn + 1; IN_RANGE(i, 0, 7) && IN_RANGE(j, 0, 7); ++i, ++j)
	{
		IS_PIECE_ATTACKING(i, j, bWhite, listAttackers, AT_Diagonal);
	}

	for (int i = coord.nRank - 1, j = coord.nColumn - 1; IN_RANGE(i, 0, 7) && IN_RANGE(j, 0, 7); --i, --j)
	{
		IS_PIECE_ATTACKING(i, j, bWhite, listAttackers, AT_Diagonal);
	}

	for (int i = coord.nRank + 1, j = coord.nColumn - 1; IN_RANGE(i, 0, 7) && IN_RANGE(j, 0, 7); ++i, --j)
	{
		IS_PIECE_ATTACKING(i, j, bWhite, listAttackers, AT_Diagonal);
	}
	for (int i = coord.nRank - 1, j = coord.nColumn + 1; IN_RANGE(i, 0, 7) && IN_RANGE(j, 0, 7); --i, ++j)
	{
		IS_PIECE_ATTACKING(i, j, bWhite, listAttackers, AT_Diagonal);
	}

	// knights //
	for (int i = -2; i <= 2; ++i)
	{
		if (i == 0) continue;
		auto nColDif = 3 - abs(i);
		for (int j = -nColDif; j <= nColDif; j += nColDif)
		{
			if (j == 0) continue;
			CoordinateImpl	crtCoord(coord.nRank + i, coord.nColumn + j);
			auto piece = m_pChessBoard->GetPiece(crtCoord);
			if (piece.bWhite == bWhite) continue;
			if (crtCoord && piece.cPiece == ChessPieceImpl::Knight)
				listAttackers.push_back(crtCoord);
		}
	}

	// pawns //
	CoordinateImpl leftPos = CoordinateImpl(coord.nRank + (bWhite ? 1 : -1), coord.nColumn - 1);
	CoordinateImpl rightPos = CoordinateImpl(coord.nRank + (bWhite ? 1 : -1), coord.nColumn + 1);

	if (leftPos)
	{
		ChessPieceImpl piece = m_pChessBoard->GetPiece(leftPos);
		if ((piece.cPiece == ChessPieceImpl::Pawn) && (piece.bWhite != bWhite)) listAttackers.push_back(leftPos);
	}

	if (rightPos)
	{
		ChessPieceImpl piece = m_pChessBoard->GetPiece(rightPos);
		if ((piece.cPiece == ChessPieceImpl::Pawn) && (piece.bWhite != bWhite)) listAttackers.push_back(rightPos);
	}

	// Kings //
	for (int i = coord.nRank - 1; i <= coord.nRank + 1; ++i)
		for (int j = coord.nColumn - 1; j <= coord.nColumn + 1; ++j)
		{
			CoordinateImpl crtCoord = CoordinateImpl(i, j);
			auto piece = m_pChessBoard->GetPiece(crtCoord);
			if (piece.IsEmpty()) continue;
			if (crtCoord == coord) continue;
			if ((piece.cPiece == ChessPieceImpl::King) && piece.bWhite != bWhite)
				listAttackers.push_back(crtCoord);
		}

	return !listAttackers.empty();
}
