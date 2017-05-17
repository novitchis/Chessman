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
            RefreshSquares();
        }

        //TODO: this should be moved to a separate class that will monitor messages
        private void PlayMoveSound(MoveData currentMove)
        {
            if (PlaySounds && currentMove != null)
                audioService.PlayMoveExecuted(currentMove.PgnMove);
        }

        protected override void OnSelectionChanged(SquareViewModel oldSquare, SquareViewModel newSquare)
        {
            if (oldSquare == null || newSquare == null)
                return;

            TryExecuteMove(oldSquare.Coordinate, newSquare.Coordinate, true);
        }

        public override void OnPieceDropped(SquareViewModel targetSquare)
        {
            TryExecuteMove(SelectedSquare.Coordinate, targetSquare.Coordinate, false);
            base.OnPieceDropped(targetSquare);
        }

        protected virtual bool TryExecuteMove(Coordinate fromCoordinate, Coordinate toCoordinate, bool useAnimations)
        {
            bool result = analysisBoardService.SubmitMove(fromCoordinate, toCoordinate);
            if (result)
            {
                var currentMove = analysisBoardService.GetCurrentMove();
                if (useAnimations)
                {
                    AnimateMove(currentMove);
                }
                else
                {
                    PlayMoveSound(currentMove);
                    RefreshSquares();
                    Messenger.Default.Send(new GenericMessage<MoveData>(this, analysisBoardService, currentMove), NotificationMessages.MoveExecuted);
                }               
            }

            return result;
        }

        //TODO: refactor this?
        protected void AnimateMove(MoveData moveData)
        {
            MoveAnimationTask moveAnimationTask = new MoveAnimationTask(moveData);
            moveAnimationTask.OnTaskCompleted = () =>
            {
                PlayMoveSound(moveData);
                RefreshSquares();
                Messenger.Default.Send(new GenericMessage<MoveData>(this, analysisBoardService, moveData), NotificationMessages.MoveExecuted);
            };

            Messenger.Default.Send(new GenericMessage<MoveAnimationTask>(this, moveAnimationTask), NotificationMessages.AnimateMoveTaskCreated);
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

        private void OnAnalysisReceived(GenericMessage<Move> bestMoveMessage)
        {
            if (ShowSuggestedMoveArrow)
                SuggestedMove = bestMoveMessage.Content;
        }
    }
}
