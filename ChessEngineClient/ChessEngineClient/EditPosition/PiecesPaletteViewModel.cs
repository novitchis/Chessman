using ChessEngine;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient.ViewModel
{
    public class PiecesPaletteViewModel : ViewModelBase
    {
        private ChessPiece selectedPiece = null;

        public List<ChessPiece> Pieces { get; set; }

        public ChessPiece SelectedPiece
        {
            get { return selectedPiece; }
            set
            {
                if (selectedPiece != value)
                {
                    selectedPiece = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public PiecesPaletteViewModel()
        {
            bool white = true;
            bool black = false;

            Pieces = new List<ChessPiece>
            {
                new ChessPiece(PieceType.Pawn, white),
                new ChessPiece(PieceType.Knight, white),
                new ChessPiece(PieceType.Bishop, white),
                new ChessPiece(PieceType.Rook, white),
                new ChessPiece(PieceType.Queen, white),
                new ChessPiece(PieceType.King, white),

                new ChessPiece(PieceType.Pawn, black),
                new ChessPiece(PieceType.Knight, black),
                new ChessPiece(PieceType.Bishop, black),
                new ChessPiece(PieceType.Rook, black),
                new ChessPiece(PieceType.Queen, black),
                new ChessPiece(PieceType.King, black),
            };
        }
    }
}
