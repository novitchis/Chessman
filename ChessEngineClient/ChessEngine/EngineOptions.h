#pragma once

namespace ChessEngine
{
	public enum class EngineLevel
	{ 
		Begginer = 0,
		Middle = 5,
		Hard = 7,
		Advanced = 10,
		Impossible = 15
	};

	public ref class EngineOptions sealed
	{
	public:
		EngineOptions();

		property EngineLevel Level
		{
			EngineLevel get() { return m_engineLevel; }
			void set(EngineLevel value) { m_engineLevel = value; }
		};
	
	private:
		EngineLevel m_engineLevel;
	};
}


