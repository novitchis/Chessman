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

		property int MultiPV
		{
			int get() { return m_multiPV; }
			void set(int value) { m_multiPV = value; }
		};
	
	private:
		int m_skillLevel;
		int m_multiPV;
	};
}


