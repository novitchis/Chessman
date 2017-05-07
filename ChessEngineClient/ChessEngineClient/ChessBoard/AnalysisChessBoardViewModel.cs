using ChessEngine;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace ChessEngineClient.ViewModel
{
    public class AnalysisChessBoardViewModel : ChessBoardViewModel
    {
        private bool showSuggestedMoveArrow = false;
        private IBoardService analysisBoardService = null;
        private IMoveAudioFeedbackService audioService = null;

        public bool ShowSuggestedMoveArrow
        {
            get { return showSuggestedMoveArrow; }
            set
            {
                showSuggestedMoveArrow = value;
                if (!showSuggestedMoveArrow)
                    SuggestedMove = null;
            }
        }

        public bool PlaySounds { get; set; }

        public AnalysisChessBoardViewModel(IBoardService analysisBoardService)
            : base(analysisBoardService)
        {
            this.analysisBoardService = analysisBoardService;
            this.audioService = ViewModelLocator.IOCContainer.Resolve<IMoveAudioFeedbackService>();
            InitBoard();

            Messenger.Default.Register<GenericMessage<MoveData>>(this, NotificationMessages.CurrentMoveChanged, OnCurrentMoveChangedMessage);
            Messenger.Default.Register<GenericMessage<MoveData>>(this, NotificationMessages.GoForwardExecuted, OnCurrentMoveChangedMessage);
            Messenger.Default.Register<GenericMessage<MoveData>>(this, NotificationMessages.GoBackExecuted, OnCurrentMoveChangedMessage);

            Messenger.Default.Register<GenericMessage<Move>>(this, NotificationMessages.AnalysisBestMoveReceived, OnAnalysisReceived);
        }

        private void OnCurrentMoveChangedMessage(GenericMessage<MoveData> moveMessage)
        {
            OnCurrentMoveChanged(moveMessage.Content);
        }

        private void OnCurrentMoveChanged(MoveData currentMove)
        {
            if (PlaySounds && currentMove != null)
                audioService.PlayMoveExecuted(currentMove.PgnMove);

            RefreshSquares();
        }

        protected override bool OnSelectionChanged(SquareViewModel oldSquare, SquareViewModel newSquare)
        {
            if (oldSquare == null || newSquare == null)
                return false;

            bool result = analysisBoardService.SubmitMove(oldSquare.Coordinate, newSquare.Coordinate);
            if (result)
            {
                var currentMove = analysisBoardService.GetCurrentMove();
                Messenger.Default.Send(new GenericMessage<MoveData>(this, analysisBoardService, currentMove), NotificationMessages.MoveExecuted);
                OnCurrentMoveChanged(currentMove);
            }

            return result;
        }

        public override void RefreshSquares()
        {
            base.RefreshSquares();
            RefreshPositionStateMarkers();            
        }

        private void RefreshPositionStateMarkers()
        {
            MoveData lastMove = analysisBoardService.GetCurrentMove();
            if (lastMove != null)
            {
                Squares[GetSquareIndex(lastMove.Move.GetFrom())].IsLastMoveSquare = true;
                Squares[GetSquareIndex(lastMove.Move.GetTo())].IsLastMoveSquare = true;
            }

            bool shouldHighlightWhiteKing = analysisBoardService.GetIsStalemate() || (analysisBoardService.IsWhiteTurn && analysisBoardService.GetIsInCheck());
            bool shouldHighlightBlackKing = analysisBoardService.GetIsStalemate() || (!analysisBoardService.IsWhiteTurn && analysisBoardService.GetIsInCheck());

            foreach (ChessPieceViewModel pieceVM in Pieces)
            {
                if (pieceVM.Piece.Type == PieceType.King)
                {
                    if (pieceVM.Piece.Color == PieceColor.White)
                        pieceVM.IsHighlighted = shouldHighlightWhiteKing;
                    else
                        pieceVM.IsHighlighted = shouldHighlightBlackKing;
                }
            }
        }

        private int GetSquareIndex(Coordinate coordinate)
        {
            int index = -1;
            if (Perspective == SideColor.White)
                index = (7 - coordinate.Y) * 8 + coordinate.X;
            else
                index = coordinate.Y * 8 + 7 - coordinate.X;

            return index;
        }

        private void OnAnalysisReceived(GenericMessage<Move> bestMoveMessage)
        {
            if (ShowSuggestedMoveArrow)
                SuggestedMove = bestMoveMessage.Content;
        }
    }
}
