#pragma once

#include <functional>
#include "EngineDefines.h"
#include "Move.h"
#include "Coordinate.h"

using namespace std;
using namespace Platform;

namespace ChessEngine
{
	class ManagedConverter
	{
	public:
		static wstring				ManagedString2WString( String^ strManaged );
		static String^				WString2ManagedString( wstring strNative );
		static string				ManagedString2String( String^ strManaged );
		static String^				String2ManagedString( string strNative );		
		static MoveImpl				ConvertManagedMove(Move^ move);
		static Move^				ConvertNativeMove(MoveImpl move);
		static CoordinateImpl		ConvertManagedCoord(Coordinate^ coord);
		static Coordinate^			ConvertNativeCoord(CoordinateImpl coord);
		//static void					ConvertServerInfo( ServerInfo^ managedServerInfo, NativeImpl::ServerInfo& ServerInfo );
		//static ItemInfo^			ConvertItemInfo( const NativeImpl::ItemInfo& NativeItemInfo );
		//static Date^				ConvertDate ( const NativeImpl::Date& Date );
		//static ByteArray			ManagedBuffer2ByteArray( ManagedByteArray^ ManagedBuffer );
		//static ManagedByteArray^	ByteArray2ManagedBuffer( const ByteArray& NativeBuffer );

		template < typename Managed, typename Native, typename Iterator >
		static void ConvertArray( Iterator itStart, Iterator itEnd, Platform::Array<Managed>^ pManagedList, std::function<Managed(Native)> f);		
	};

	template < typename Managed, typename Native, typename Iterator >
	inline void ManagedConverter::ConvertArray( Iterator itStart, Iterator itEnd, Platform::Array<Managed>^ pManagedList, std::function<Managed(Native)> Converter )
	{
		// TODO: Handle index out of bounds //
		int iIndex = -1;
		for ( auto it = itStart; it != itEnd; ++it )
		{
			pManagedList[++iIndex] = Converter(*it);
		}
	}
}
