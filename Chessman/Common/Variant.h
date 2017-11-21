#pragma once
#include <string>

namespace Core
{
	enum VariantType
	{
		VT_None,
		VT_Bool,
		VT_String,
	};
	
	struct Variant
	{

		Variant( )
			: type ( VT_None )
		{

		}
		
		Variant(  bool _bValue )
			: type ( VT_Bool )
			, bValue( _bValue )
		{

		}
		
		Variant(  const std::string& _strValue )
			: type ( VT_String )
			, strValue( _strValue )
		{

		}
		
		VariantType type;
		bool bValue;
		std::string strValue;
		
		operator bool() const 
		{
			_ASSERTE ( type == VT_Bool );
			return bValue;
		}
		operator std::string() const
		{
			_ASSERTE( type == VT_String );
			return strValue;
		}
	};

}

