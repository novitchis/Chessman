using ChessEngine;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient.ViewModel
{
    public class ChessBoardViewModel : ViewModelBase
    {
        public List<SquareViewModel> Squares { get; private set; }

        public ChessBoardViewModel()
        {
            Squares = new List<SquareViewModel>();

            //TODO: just for test
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Squares.Add(new SquareViewModel(new Coordinate(x,y)));
                }
            }
        }
    }
}
