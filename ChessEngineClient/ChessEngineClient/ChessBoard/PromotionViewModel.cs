using ChessEngine;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient.ViewModel
{
    public class PromotionViewModel : ViewModelBase
    {
        public List<ChessPieceViewModel> Pieces { get; set; }

        public PromotionViewModel(PieceColor pieceColor)
        {
            bool isWhite = pieceColor == PieceColor.White;
            Pieces = new List<ChessPiece>
            {
                new ChessPiece(PieceType.Queen, isWhite),
                new ChessPiece(PieceType.Rook, isWhite),
                new ChessPiece(PieceType.Bishop, isWhite),
                new ChessPiece(PieceType.Knight, isWhite),
            }.Select(p => new ChessPieceViewModel(p, null)).ToList();
        }
    }
}
