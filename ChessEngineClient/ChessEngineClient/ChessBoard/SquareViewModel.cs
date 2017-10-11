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
        private bool isLastMoveSquare = false;
        private PossibleMoveMark possibleMoveMark = PossibleMoveMark.None;

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

        public PossibleMoveMark PossibleMoveMark
        {
            get { return possibleMoveMark; }
            set
            {
                if (possibleMoveMark != value)
                {
                    possibleMoveMark = value;
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

    public enum PossibleMoveMark
    {
        None,
        Piece,
        EmptySquare
    }
}
