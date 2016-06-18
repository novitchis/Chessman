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
        private IChessBoardService chessBoardService = null;
        private SquareViewModel selectedSquare = null;

        #region "Properties"

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

        #endregion

        public ChessBoardViewModel(IChessBoardService chessBoardService)
        {
            this.chessBoardService = chessBoardService;
            Squares = new List<SquareViewModel>();

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    SquareViewModel squareViewModel = new SquareViewModel(new Coordinate(x, y));
                    squareViewModel.Piece = chessBoardService.GetPiece(squareViewModel.Coordinate);
                    Squares.Add(squareViewModel);
                }
            }
        }
    }
}
