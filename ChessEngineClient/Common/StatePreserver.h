#pragma once
#include "Variant.h"

namespace Core
{
	class StatePreserver
	{
	public:
		StatePreserver()
			: bChanged ( true )
		{

		}

		bool HasChanged() const
		{
			return bChanged;
		}
		
		const Variant& GetState() const
		{
			return vtState;
		}
		
		void SetState( const Variant& _vtState )
		{
			vtState = _vtState;
			bChanged = false;
		}
		
		void OnStateChange()
		{
			bChanged = true;
		}
	
	private:
		Variant vtState;
		bool bChanged;
	};

}

