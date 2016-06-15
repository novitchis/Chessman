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
        private SquareViewModel selectedSquare = null;

        public List<SquareViewModel> Squares { get; private set; }

        public SquareViewModel SelectedSquare
        {
            get { return selectedSquare; }
            set
            {
                if (selectedSquare != value)
                {
                    if (selectedSquare != null)
                        selectedSquare.IsSelected = false;

                    selectedSquare = value;

                    if (selectedSquare != null)
                        selectedSquare.IsSelected = true;

                    NotifyPropertyChanged();
                }
            }
        }

        public ChessBoardViewModel()
        {
            Squares = new List<SquareViewModel>();

            ChessBoard chessBoard = new ChessBoard();
            chessBoard.Initialize();
            //TODO: just for test
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    SquareViewModel squareViewModel = new SquareViewModel(new Coordinate(x, y));
                    squareViewModel.Piece = chessBoard.GetPiece(squareViewModel.Coordinate);
                    Squares.Add(squareViewModel);
                }
            }
        }
    }
}
