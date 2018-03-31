#include "pch.h"
#include "ManagedConverter.h"
#include "DataConverter.h"
#include "Inspectable.h"
#include "Robuffer.h"

//#include <msclr\marshal_cppstd.h>

using namespace ChessEngine;
using namespace Platform;
//using namespace Microsoft::WRL;

wstring ManagedConverter::ManagedString2WString( String^ strManaged )
{
	if ( !strManaged ) return wstring();
	return wstring(strManaged->Data());
}

String^ ManagedConverter::WString2ManagedString( wstring strNative )
{
	return ref new String( strNative.c_str() );
}

string	ManagedConverter::ManagedString2String( String^ strManaged )
{
	wstring wstrNative = ManagedString2WString( strManaged );
	return DataConverter::WStringToUTF8( wstrNative );
}

String^	ManagedConverter::String2ManagedString( string strNative )
{
	wstring wstrNative = DataConverter::UTF8ToWString( strNative.c_str() );
	return ManagedConverter::WString2ManagedString( wstrNative );
}

MoveImpl ManagedConverter::ConvertManagedMove(Move^ move)
{
	return MoveImpl(ConvertManagedCoord(move->GetFrom()), ConvertManagedCoord(move->GetTo()));
}

CoordinateImpl ManagedConverter::ConvertManagedCoord(Coordinate^ coord)
{
	return CoordinateImpl(coord->X, coord->Y);
}

Coordinate^ ManagedConverter::ConvertNativeCoord(CoordinateImpl coord)
{
	return ref new Coordinate(coord.nColumn, coord.nRank);
}

EngineOptionsImpl ManagedConverter::ConvertManagedOptions(EngineOptions^ options)
{
	EngineOptionsImpl result;
	result.level = options->SkillLevel;
	result.multiPV = options->MultiPV;

	return result;
}

//void ManagedConverter::ConvertServerInfo( ServerInfo^ managedServerInfo, NativeImpl::ServerInfo& ServerInfo )
//{
//	if ( !managedServerInfo ) return;
//
//	ServerInfo.m_strUrl = ManagedString2WString( managedServerInfo->GetUrl() );
//	ServerInfo.m_strUsername = ManagedString2WString( managedServerInfo->GetUsername() );
//	ServerInfo.m_strPassword = ManagedString2WString( managedServerInfo->GetPassword() );
//	ServerInfo.m_port = managedServerInfo->GetPort();
//}
//
//ItemInfo^ ManagedConverter::ConvertItemInfo( const NativeImpl::ItemInfo& NativeItemInfo )
//{
//	String^ strMName = WString2ManagedString( NativeItemInfo.GetName() );
//	Date^	strMDate = ConvertDate( NativeItemInfo.GetModifiedDate() );
//
//	return ref new ItemInfo( strMName, strMDate, NativeItemInfo.IsDirectory(), NativeItemInfo.GetFileSize() );
//}
//
//Date^ ManagedConverter::ConvertDate ( const NativeImpl::Date& NativeDate )
//{
//	return ref new Date( NativeDate.m_day, NativeDate.m_month, NativeDate.m_year, NativeDate.m_hour, NativeDate.m_minute );
//}
//
//
//ByteArray ManagedConverter::ManagedBuffer2ByteArray( ManagedByteArray^ ManagedBuffer )
//{
//	ByteArray result(ManagedBuffer->Length);
//
//	memcpy( &result[0], &ManagedBuffer[0], ManagedBuffer->Length );
//	
//	return result;
//}
//
//ManagedByteArray^ ManagedConverter::ByteArray2ManagedBuffer( const ByteArray& NativeBuffer )
//{
//	ManagedByteArray^ mba = ref new ManagedByteArray(NativeBuffer.size());
//
//	ConvertArray<byte, byte, ByteArray::const_iterator>( NativeBuffer.begin(), NativeBuffer.end(), mba, 
//		[](byte data) 
//		{
//			return data;
//		});
//
//	return mba;
//}
