using ChessEngine;
using System.Collections.Generic;

namespace ChessEngineClient.ViewModel
{
    public class MoveDataGroup
    {
        public int MoveNumber { get; set; }

        public MoveData WhiteMove { get; set; }

        public MoveData BlackMove { get; set; }

        public MoveDataGroup(int moveNumber)
        {
            MoveNumber = moveNumber;
        }
    }
}