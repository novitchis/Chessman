using ChessEngine;
using System.Collections.Generic;

namespace ChessEngineClient.ViewModel
{
    public class MoveDataGroup
    {
        public int MoveNumber { get; set; }

        public MoveData WhiteMove { get; set; }

        public MoveData BlackMove { get; set; }

        public List<MoveData> Moves
        {
            get
            {
                var result = new List<MoveData> { WhiteMove };
                if (BlackMove != null)
                    result.Add(BlackMove);

                return result;
            }
        }

        public MoveDataGroup(int moveNumber)
        {
            MoveNumber = moveNumber;
        }
    }
}