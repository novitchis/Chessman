using ChessEngine;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient.ViewModel
{
    public class SquareViewModel : ViewModelBase, ICoordinatedItem
    {
        private ChessPieceViewModel pieceViewModel = null;
        private bool isLastMoveSquare = false;

        #region "Properties"

        public Coordinate Coordinate { get; private set; }


        public bool IsLastMoveSquare
        {
            get { return isLastMoveSquare; }
            set
            {
                if (isLastMoveSquare != value)
                {
                    isLastMoveSquare = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        public SquareViewModel(Coordinate coordinate)
        {
            Coordinate = coordinate;
        }
    }
}
