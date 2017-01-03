using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient
{
    public interface IPracticeBoardService : IEngineBoardService
    {
        SideColor UserPerspective { get; set; }

        void RequestComputerMove();

        bool GetIsComputerTurn();
    }
}
