using ChessEngine;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Chessman.ViewModel
{
    public class AnalysisChessBoardViewModel : ChessBoardViewModel
    {
        private bool showSuggestedMoveArrow = false;
        private IBoardService analysisBoardService = null;
        private IMoveAudioFeedbackService audioService = null;
        private bool showLegalMoves = false;

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

        public bool ShowLegalMoves
        {
            get => showLegalMoves;
            set
            {
                showLegalMoves = value;
                MarkLegalMovesSquares(SelectedSquare);
            }
        }

        public AnalysisChessBoardViewModel(IBoardService analysisBoardService)
            : base(analysisBoardService)
        {
            this.analysisBoardService = analysisBoardService;
            this.audioService = ViewModelLocator.IOCContainer.Resolve<IMoveAudioFeedbackService>();
            InitBoard();

            Messenger.Default.Register<GenericMessage<MoveData>>(this, NotificationMessages.CurrentMoveChanged, OnCurrentMoveChangedMessage);
            Messenger.Default.Register<GenericMessage<MoveData>>(this, NotificationMessages.GoForwardExecuted, OnGoForwardMessage);
            Messenger.Default.Register<GenericMessage<MoveData>>(this, NotificationMessages.GoBackExecuted, OnGoBackMessage);

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

            ExecuteCurrentMoveOnBoard(false);
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
            MarkLegalMovesSquares(newSquare);

            if (oldSquare == null || newSquare == null)
                return;

            TryExecuteMove(oldSquare.Coordinate, newSquare.Coordinate, false);
        }

        private void MarkLegalMovesSquares(SquareViewModel newSquare)
        {
            Squares.ForEach(s => s.PossibleMoveMark = PossibleMoveMark.None);

            if (!ShowLegalMoves || newSquare == null)
                return;

            IList<Coordinate> availableMovesCoordinates = analysisBoardService.GetAvailableMoves(newSquare.Coordinate);
            foreach (Coordinate coordinate in availableMovesCoordinates)
                GetSquare(coordinate).PossibleMoveMark = GetPiece(coordinate) != null ? PossibleMoveMark.Piece : PossibleMoveMark.EmptySquare;
        }

        public override void OnPieceDropped(SquareViewModel targetSquare)
        {
            MarkLegalMovesSquares(targetSquare);

            TryExecuteMove(SelectedSquare.Coordinate, targetSquare.Coordinate, true);
            base.OnPieceDropped(targetSquare);
        }

        protected bool TryExecuteMove(Coordinate fromCoordinate, Coordinate toCoordinate, bool instantMove)
        {
            if (IsPawnPromotionMove(fromCoordinate, toCoordinate))
            {
                // TODO: maybe validate always?
                if (!analysisBoardService.ValidateMove(fromCoordinate, toCoordinate))
                    return false;

                InitiatePromotionMove(new Move(fromCoordinate, toCoordinate), instantMove);

                // a promotion move is not immediateley executed
                // the user is prompted to select the piece type
                return false;
            }

            bool result = analysisBoardService.SubmitMove(fromCoordinate, toCoordinate);
            if (result)
            {
                ExecuteCurrentMoveOnBoard(instantMove);
                OnNewMoveExecuted();
            }

            return result;
        }

        private bool IsPawnPromotionMove(Coordinate fromCoordinate, Coordinate toCoordinate)
        {
            ChessPiece movingPiece = analysisBoardService.GetPiece(fromCoordinate);
            // doesn't matter if is invalid move it wouldn't pass validation
            return movingPiece != null && movingPiece.Type == PieceType.Pawn && (toCoordinate.Y == 7 || toCoordinate.Y == 0);
        }

        private void InitiatePromotionMove(Move move, bool instantMove)
        {
            PromotionMoveTask promotionTask = new PromotionMoveTask(move, instantMove, pieceType =>
            {
                if (pieceType == PieceType.None)
                    return;

                bool result = analysisBoardService.SubmitPromotionMove(move.GetFrom(), move.GetTo(), pieceType);
                if (result)
                {
                    ExecuteCurrentMoveOnBoard(true);
                    OnNewMoveExecuted();
                }
            });

            Messenger.Default.Send(new GenericMessage<PromotionMoveTask>(this, promotionTask), NotificationMessages.AnimatePromotionMoveTask);
        }

        protected virtual void OnNewMoveExecuted()
        {
        }

        private void UndoMoveOnBoard(MoveData move)
        {
            ClearCurrentMoveData();

            MoveTask moveTask = new MoveTask(move);
            UndoMoveTask(moveTask);

            Messenger.Default.Send(new GenericMessage<MoveTask>(this, moveTask), NotificationMessages.AnimateUndoMoveTaskCreated);
            RefreshPositionStateMarkers();
        }

        protected void ExecuteCurrentMoveOnBoard(bool instantMove)
        {
            ClearCurrentMoveData();

            MoveData currentMove = analysisBoardService.GetCurrentMove();
            MoveTask moveTask = new MoveTask(currentMove);
            ExecuteMoveTask(moveTask);

            if (instantMove)
                moveTask.OnTransitionCompleted();
            else
                Messenger.Default.Send(new GenericMessage<MoveTask>(this, moveTask), NotificationMessages.AnimateMoveTaskCreated);

            RefreshPositionStateMarkers();
        }

        private void ExecuteMoveTask(MoveTask moveTask)
        {
            ChessPieceViewModel capturedPieceViewModel = null;
            if (moveTask.CapturedPieceCoordinate != null)
            {
                capturedPieceViewModel = GetPiece(moveTask.CapturedPieceCoordinate);
                capturedPieceViewModel.RemovePending = true;
            }

            ChessPieceViewModel mainPieceViewModel = GetPiece(moveTask.MoveData.Move.GetFrom());

            foreach (var positionChange in moveTask.MovedPiecesCoordinates)
            {
                ChessPieceViewModel pieceViewModel = GetPiece(positionChange.Item1);
                pieceViewModel.Coordinate = positionChange.Item2;
            }

            moveTask.OnTransitionCompleted = () =>
            {
                if (capturedPieceViewModel != null)
                    RemovePiece(capturedPieceViewModel);

                if (moveTask.MoveData.PawnPromoted)
                {
                    mainPieceViewModel.Piece = new ChessPiece(moveTask.MoveData.Move.GetPromotionPieceType(), mainPieceViewModel.Piece.Color == PieceColor.White);
                    //have to re-add the piece in order to update the view
                    ReplacePiece(mainPieceViewModel, mainPieceViewModel);
                }

                PlayMoveSound(moveTask.MoveData);
                Messenger.Default.Send(new GenericMessage<MoveData>(this, analysisBoardService, moveTask.MoveData), NotificationMessages.MoveExecuted);
            };
        }

        private void UndoMoveTask(MoveTask moveTask)
        {
            if (moveTask.CapturedPieceCoordinate != null)
                AddPiece(new ChessPieceViewModel(analysisBoardService.GetPiece(moveTask.CapturedPieceCoordinate), moveTask.CapturedPieceCoordinate));

            foreach (var positionChange in moveTask.MovedPiecesCoordinates)
            {
                ChessPieceViewModel pieceViewModel = GetPiece(positionChange.Item2);
                pieceViewModel.Coordinate = positionChange.Item1;
            }

            if (moveTask.MoveData.PawnPromoted)
            {
                ChessPieceViewModel promotedPieceViewModel = GetPiece(moveTask.MoveData.Move.GetFrom());
                promotedPieceViewModel.Piece = new ChessPiece(PieceType.Pawn, promotedPieceViewModel.Piece.Color == PieceColor.White);
                ReplacePiece(promotedPieceViewModel, promotedPieceViewModel);
            }

            moveTask.OnTransitionCompleted = () =>
            {
                PlayMoveSound(moveTask.MoveData);
                Messenger.Default.Send(new GenericMessage<MoveData>(this, analysisBoardService, analysisBoardService.GetCurrentMove()), NotificationMessages.MoveExecuted);
            };
        }

        public override void RefreshSquares()
        {
            base.RefreshSquares();
            RefreshPositionStateMarkers();            
        }

        private void RefreshPositionStateMarkers()
        {
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
