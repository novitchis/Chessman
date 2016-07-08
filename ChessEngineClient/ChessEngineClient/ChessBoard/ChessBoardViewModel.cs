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
                    OnSelectionChanged(selectedSquare, value);
                    selectedSquare = value;

                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        public ChessBoardViewModel(IChessBoardService chessBoardService)
        {
            this.chessBoardService = chessBoardService;
            this.chessBoardService.GoToExecuted += OnGoToExecuted;
            Squares = new List<SquareViewModel>();

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                    Squares.Add(new SquareViewModel(new Coordinate(x, y)));
            }
            RefreshPieces();
        }

        private void OnGoToExecuted(object sender, ChessEventArgs e)
        {
            RefreshPieces();
        }

        private void OnSelectionChanged(SquareViewModel oldSquare, SquareViewModel newSquare)
        {
            if (oldSquare != null)
                oldSquare.IsSelected = false;

            if (newSquare != null)
                newSquare.IsSelected = true;

            if (oldSquare == null || newSquare == null)
                return;

            if (chessBoardService.SubmitMove(oldSquare.Coordinate, newSquare.Coordinate))
                RefreshPieces();

        }

        private void RefreshPieces()
        {
            foreach (SquareViewModel square in Squares)
                square.Piece = chessBoardService.GetPiece(square.Coordinate);
        }
    }
}
