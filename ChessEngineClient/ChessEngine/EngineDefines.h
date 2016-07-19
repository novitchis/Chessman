#pragma once
#define IN_RANGE( val, min, max ) ( ( ( val ) >= ( min ) ) && ( ( val ) <= ( max ) ) )

#include <string>
#include <windows.h>
#include <memory>
#include <algorithm>
#include <tchar.h>

namespace ChessEngine
{
	enum ChessEngineType
	{
		CET_UCI = 0, // only one supported
		CET_CEPC, 
	};

	struct Timer
	{
		DWORD dwInitialTime;
		DWORD dwMoveIncrement;
		DWORD dwTimeIncreaseAfter40Moves;
	};

	 // This stores a chess board field 0..7 //
	struct CoordinateImpl
	{
	public:
		CoordinateImpl()
			: nRank ( -1 )
			, nColumn ( -1 )
		{

		}

		CoordinateImpl( int _nRank, int _nColumn )
			: nRank( _nRank )
			, nColumn ( _nColumn )
		{

		}
		
		operator bool() const
		{
			return IN_RANGE( nRank, 0, 7 ) && IN_RANGE( nColumn, 0, 7 );
		}

		bool operator < ( const CoordinateImpl& rhs ) const
		{
			if ( nRank != rhs.nRank ) return nRank < rhs.nRank;
			return nColumn < rhs.nColumn;
		}

		bool operator== ( const CoordinateImpl& rhs ) const
		{
			return ( nColumn == rhs.nColumn ) && ( nRank == rhs.nRank );
		}
		
		bool operator!= ( const CoordinateImpl& rhs ) const
		{
			return !operator==( rhs );
		}

		
        bool IsWhiteSquare()
            {
            return ((nRank) % 2) == ((nColumn) % 2);
            }

        std::string ToString() const
		{
			if ( !this ) return "";
			std::string strRes;
			strRes.push_back( (char) ( nColumn + 'a' ) );
			strRes.push_back( (char) ( nRank + '1' ) );
			return strRes;
		}

		std::string GetCoordStr() const
		{
			std::string strRes;
			strRes.push_back( (char) ( nColumn + 'a' ) );
			strRes.push_back( ' ' );
			strRes.push_back( (char) ( nRank + '1' ) );
			return strRes;
		}
        //std::wstring ToStringW() const
        //    {
        //    if ( !this ) return L"";
        //    std::wstring strRes;
        //    strRes.push_back( (wchar_t) ( nColumn + 'a' ) );
        //    strRes.push_back( (wchar_t) ( nRank + '1' ) );
        //    return strRes;
        //    }

        std::basic_string<TCHAR> ToStringT() const
            {
            if ( !this ) return _T("");
            if( -1==nRank || -1==nColumn ) return _T("");
            std::wstring strRes;
            strRes.push_back( (TCHAR) ( nColumn + 'a' ) );
            strRes.push_back( (TCHAR) ( nRank + '1' ) );
            return strRes;
            }

        static CoordinateImpl FromString( std::string strCoord )
		{
			std::transform( strCoord.begin(), strCoord.end(), strCoord.begin(), ::tolower);
			return CoordinateImpl( strCoord[1] - '1', strCoord[0] - 'a' );
		}

		int hDistance( CoordinateImpl other ) const
		{
			return nColumn - other.nColumn;
		}

		int vDistance( CoordinateImpl other ) const
		{
			return nRank - other.nRank;
		}

		int nRank; // a-h
		int nColumn; // 1-8
	};


	class ChessPieceImpl
	{
	public:
		ChessPieceImpl()
			: cPiece( None )
			, bWhite ( true )
			, bTransformed( false )
		{

		}

		ChessPieceImpl( char _cPiece, bool _bWhite )
			: cPiece( _cPiece )
			, bWhite( _bWhite )
			, bTransformed( false )
		{

		}

		bool operator ==( const ChessPieceImpl& rhs ) const
		{
			return ( cPiece == rhs.cPiece ) && ( bWhite == rhs.bWhite );
		}
		bool operator <( const ChessPieceImpl& rhs ) const
		{
			if ( bWhite != rhs.bWhite ) return bWhite;
			return ( cPiece < rhs.cPiece );
		}
		
		bool operator !=( const ChessPieceImpl& rhs ) const
		{
			return !operator==( rhs );
		}

		std::string GetPieceStr()
		{
			switch ( cPiece ) 
			{
			case ChessPieceImpl::Pawn:
				return "pawn";
				break;
			case ChessPieceImpl::Knight:
				return "Knight";
				break;
			case ChessPieceImpl::Bishop:
				return "Bishop";
				break;
			case ChessPieceImpl::Rock:
				return "Rock";
				break;
			case ChessPieceImpl::Queen:
				return "Queen";
				break;
			case ChessPieceImpl::King:
				return "King";
				break;
			}

			return "";
		}
		bool AttacksTransversally()
		{
			return ( cPiece == Rock ) || ( cPiece == Queen );
		}
		
		bool AttacksDiagonally()
		{
			return ( cPiece == Bishop ) || ( cPiece == Queen );
		}
		
		bool IsEmpty() const
		{
			return cPiece == None;
		}
		//ChessPieceType	type;
		char			cPiece;
		bool			bWhite;
		bool			bTransformed;

		static const char None		=	' ';
		static const char Pawn		=	'p';
		static const char Knight	=	'n';
		static const char Bishop	=	'b';
		static const char Rock		=	'r';
		static const char Queen		=	'q';
		static const char King		=	'k';
	};

	
	struct MoveImpl
	{
		MoveImpl()
		{

		}

		MoveImpl( const CoordinateImpl& _from, const CoordinateImpl& _to )
			: from( _from )
			, to( _to )
		{

		}

		operator bool () const
		{
			return from && to;
		}

		static MoveImpl FromString( std::string strMove )
		{
			std::transform(strMove.begin(), strMove.end(), strMove.begin(), ::tolower);
			if(strMove.size()<4)
				return MoveImpl(CoordinateImpl(),CoordinateImpl());
			CoordinateImpl from( strMove[1] - '1', strMove[0] - 'a' );
			CoordinateImpl to ( strMove[3] - '1', strMove[2] - 'a' );

			return MoveImpl( from, to );
		}

		std::string Serialize() const
		{
			return from.ToString() + to.ToString();
		}

		CoordinateImpl from;
		CoordinateImpl to;
		ChessPieceImpl promotionPiece;
	};


	enum MoveInfo
	{
		MI_EnPassant = 0,
		MI_Castle,
		MI_Check,
		MI_Promotion,
	};

	struct AdditionalMoveInfo
	{
		MoveInfo	type;
		CoordinateImpl	coordEnPassant; // en passant
		MoveImpl		RockMove; // castle
		ChessPieceImpl  PromotionPiece;
	};

	enum EngineLevel
	{
		EL_Begginer = 0,
		EL_Middle = 5,
        EL_Hard = 7, 
		EL_Advanced = 10, 
		EL_Impossible = 15
	};

	struct ChessEngineOptions
	{
		EngineLevel level;
	};
}