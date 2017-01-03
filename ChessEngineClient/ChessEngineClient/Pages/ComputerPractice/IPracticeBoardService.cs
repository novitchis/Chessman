using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient
{
    public interface IPracticeBoardService : IEngineBoardService
    {
        SideColor UserColor { get; }

        void SwitchUserColor();

        void RequestComputerMove();

        bool GetIsComputerTurn();
    }
}
