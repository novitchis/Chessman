using ChessEngine;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChessEngineClient.ViewModel
{
    public class PromotionViewModel : ViewModelBase
    {
        public List<ChessPieceViewModel> Pieces { get; set; }

        public ICommand PieceSelectedCommand { get; set; }

        public PromotionViewModel(PieceColor pieceColor)
        {
            bool isWhite = pieceColor == PieceColor.White;
            Pieces = new List<ChessPieceViewModel>
            {
                new ChessPieceViewModel(new ChessPiece(PieceType.Queen, isWhite), null),
                new ChessPieceViewModel(new ChessPiece(PieceType.Rook, isWhite),null),
                new ChessPieceViewModel(new ChessPiece(PieceType.Bishop, isWhite), null),
                new ChessPieceViewModel(new ChessPiece(PieceType.Knight, isWhite), null),
                new ChessPieceViewModel(new ChessPiece(PieceType.None, isWhite), null),
            };

            if (!isWhite)
                Pieces.Reverse();
        }
    }
}
