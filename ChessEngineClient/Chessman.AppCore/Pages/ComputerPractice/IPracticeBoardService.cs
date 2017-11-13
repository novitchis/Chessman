using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chessman
{
    public interface IPracticeBoardService : IEngineBoardService
    {
        event AnalysisEventHandler AnalysisReceived;

        SideColor UserColor { get; }

        void SwitchUserColor();

        void RequestComputerMove();

        bool IsComputerTurn();

        void SetEngineStrength(int strengthValue);
    }
}
