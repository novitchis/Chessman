using ChessEngine;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient.ViewModel
{
    public class SquareViewModel : ViewModelBase
    {
        public Coordinate Coordinate { get; private set; }

        public ChessPiece Piece { get; set; }

        public SquareViewModel(Coordinate coordinate)
        {
            Coordinate = coordinate;
        }
    }
}
