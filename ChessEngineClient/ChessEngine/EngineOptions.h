#pragma once

namespace ChessEngine
{
	public ref class EngineOptions sealed
	{
	public:
		EngineOptions();

		property int SkillLevel
		{
			int get() { return m_skillLevel; }
			void set(int value) { m_skillLevel = value; }
		};
	
	private:
		int m_skillLevel;
	};
}


