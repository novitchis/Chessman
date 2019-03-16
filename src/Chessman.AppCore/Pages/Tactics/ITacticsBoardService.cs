using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chessman
{
    public interface ITacticsBoardService: IBoardService
    {
        event EventHandler StateChanged;

        Task LoadTacticAsync();

        Task ExecuteNextMoveAsync();

        void Restart();

        Task SkipAsync();

        bool IsComputerTurn();

        TacticState GetState();
    }

    public enum TacticState
    {
        NotStarted,
        InProgress,
        Resolved,
        Failed
    }
}
