using ChessEngine;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient.ViewModel
{
    public class ChessPieceViewModel : ViewModelBase, ICoordinatedItem
    {
        private bool isHighlighted = false;
        private bool isDragSource = false;
        private ChessPiece piece = null;

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

        public bool IsDragSource
        {
            get { return isDragSource; }
            set
            {
                if (isDragSource != value)
                {
                    isDragSource = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ChessPiece Piece
        {
            get { return piece; }
            set
            {
                if (piece != value)
                {
                    piece = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Coordinate Coordinate
        {
            get; set;
        }

        public bool RemovePending { get; set; }

        public ChessPieceViewModel(ChessPiece piece, Coordinate coordinate)
        {
            Piece = piece;
            Coordinate = coordinate;
        }
    }
}
