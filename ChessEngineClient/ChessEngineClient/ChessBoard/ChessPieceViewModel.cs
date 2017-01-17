using ChessEngine;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient.ViewModel
{
    public class ChessPieceViewModel : ViewModelBase
    {
        private bool isHighlighted = false;

        public bool IsHighlighted
        {
            get { return isHighlighted; }
            set
            {
                if (isHighlighted != value)
                {
                    if (Piece.Type != PieceType.King)
                        throw new NotSupportedException("Only kings can be highlighted");

                    isHighlighted = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ChessPiece Piece
        {
            get; private set;
        }

        public ChessPieceViewModel(ChessPiece piece)
        {
            Piece = piece;
        }
    }
}
