#pragma once

namespace ChessEngine
{
	class DataConverter
	{
	public:
		static std::wstring UTF8ToWString( const char* const pUtf8String );
		static std::string WStringToUTF8( const std::wstring& strUnicode );
	};
}

