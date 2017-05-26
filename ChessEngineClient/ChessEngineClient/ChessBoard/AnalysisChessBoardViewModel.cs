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
            Messenger.Default.Register<GenericMessage<MoveData>>(this, NotificationMessages.GoForwardExecuted, OnGoForwardMessage);
            Messenger.Default.Register<GenericMessage<MoveData>>(this, NotificationMessages.GoBackExecuted, OnCurrentMoveChangedMessage);

            Messenger.Default.Register<GenericMessage<Move>>(this, NotificationMessages.AnalysisBestMoveReceived, OnAnalysisReceived);
        }

        private void OnGoBackMessage(GenericMessage<MoveData> message)
        {
            if (this.analysisBoardService != message.Target)
                return;

            UndoMoveOnBoard(message.Content);
        }

        private void OnGoForwardMessage(GenericMessage<MoveData> message)
        {
            if (this.analysisBoardService != message.Target)
                return;

            ExecuteCurrentMoveOnBoard(true);
        }

        private void OnCurrentMoveChangedMessage(GenericMessage<MoveData> message)
        {
            if (this.analysisBoardService != message.Target)
                return;

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
                ExecuteCurrentMoveOnBoard(useAnimations);

            return result;
        }

        private void UndoMoveOnBoard(MoveData move)
        {
            //TODO: this does not works

            MoveTask moveTask = new MoveTask(move);
            moveTask.ReverseMovedPieceCoordinates();

            ExecuteMoveTask(moveTask);

            moveTask.OnTransitionCompleted = () =>
            {
                //TODO: capture move?
                PlayMoveSound(move);
            };

            Messenger.Default.Send(new GenericMessage<MoveTask>(this, moveTask), NotificationMessages.AnimateMoveTaskCreated);
        }

        private void ExecuteCurrentMoveOnBoard(bool useAnimations)
        {
            MoveData currentMove = analysisBoardService.GetCurrentMove();
            MoveTask moveTask = new MoveTask(currentMove);
            moveTask.OnTransitionCompleted = () =>
            {
                PlayMoveSound(currentMove);
                ExecuteMoveTask(moveTask);
                Messenger.Default.Send(new GenericMessage<MoveData>(this, analysisBoardService, currentMove), NotificationMessages.MoveExecuted);
            };

            if (useAnimations)
                Messenger.Default.Send(new GenericMessage<MoveTask>(this, moveTask), NotificationMessages.AnimateMoveTaskCreated);
            else
                moveTask.OnTransitionCompleted();
        }

        private void ExecuteMoveTask(MoveTask moveTask)
        {
            if (moveTask.CapturedPieceCoordinate != null)
                RemovePiece(GetPieceViewModel(moveTask.CapturedPieceCoordinate));

            foreach (var positionChange in moveTask.MovedPiecesCoordinates)
            {
                ChessPieceViewModel pieceViewModel = GetPieceViewModel(positionChange.Item1);
                pieceViewModel.Coordinate = positionChange.Item2;
            }

            RefreshPositionStateMarkers();
        }

        public override void RefreshSquares()
        {
            base.RefreshSquares();
            RefreshPositionStateMarkers();            
        }

        private void RefreshPositionStateMarkers()
        {
            SelectedSquare = null;
            SuggestedMove = null;

            MoveData lastMove = analysisBoardService.GetCurrentMove();
            
            SetLastMove(lastMove);
            SetKingsHighlight();
        }

        private void SetKingsHighlight()
        {
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

        private void SetLastMove(MoveData lastMove)
        {
            Squares.ForEach(s => s.IsLastMoveSquare = false);
            if (lastMove != null)
            {
                Squares[GetSquareIndex(lastMove.Move.GetFrom())].IsLastMoveSquare = true;
                Squares[GetSquareIndex(lastMove.Move.GetTo())].IsLastMoveSquare = true;
            }
        }

        private void OnAnalysisReceived(GenericMessage<Move> bestMoveMessage)
        {
            if (ShowSuggestedMoveArrow)
                SuggestedMove = bestMoveMessage.Content;
        }
    }
}
