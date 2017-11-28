#include "pch.h"
#include "DataConverter.h"

#include <codecvt>

using namespace ChessEngine;

std::wstring DataConverter::UTF8ToWString( const char* const pUtf8String )
{
	if( !pUtf8String ) return std::wstring();

	std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;
	
	return converter.from_bytes( pUtf8String );
}


std::string DataConverter::WStringToUTF8( const std::wstring& strUnicode )
{
	std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;

	return converter.to_bytes( strUnicode );
}
