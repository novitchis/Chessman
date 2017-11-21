using ChessEngine;
using System;
using System.Collections.Generic;

namespace Chessman.ViewModel
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

        public override string ToString()
        {
            return String.Format("{0}. {1} {2}", MoveNumber, WhiteMove.ToString(), BlackMove?.ToString());
        }
    }
}