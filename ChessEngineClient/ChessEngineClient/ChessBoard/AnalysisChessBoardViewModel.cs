﻿using ChessEngine;
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
            //TODO: for a promotion move first validate the move
            // then display a selection for the promoted piece
            if (IsPawnPromotionMove(fromCoordinate, toCoordinate))
            {
                // TODO: maybe validate always?
                if (!analysisBoardService.ValidateMove(fromCoordinate, toCoordinate))
                    return false;

                InitiatePromotionMove(new Move(fromCoordinate, toCoordinate), useAnimations);

                // a promotion move is not immediateley executed
                // the user is prompted to 
                return false;
            }

            bool result = analysisBoardService.SubmitMove(fromCoordinate, toCoordinate);
            if (result)
                ExecuteCurrentMoveOnBoard(useAnimations);

            return result;
        }

        private bool IsPawnPromotionMove(Coordinate fromCoordinate, Coordinate toCoordinate)
        {
            ChessPiece movingPiece = analysisBoardService.GetPiece(fromCoordinate);
            // doesn't matter if is invalid move it wouldn't pass validation
            return movingPiece != null && movingPiece.Type == PieceType.Pawn && (toCoordinate.Y == 7 || toCoordinate.Y == 0);
        }

        private void InitiatePromotionMove(Move move, bool isDropAction)
        {
            PromotionMoveTask promotionTask = new PromotionMoveTask(move, isDropAction, pieceType =>
            {
                if (pieceType == PieceType.None)
                    return;

                ChessPiece piece = new ChessPiece(pieceType, analysisBoardService.IsWhiteTurn);
                bool result = analysisBoardService.SubmitPromotionMove(move.GetFrom(), move.GetTo(), piece);
                if (result)
                    ExecuteCurrentMoveOnBoard(false);
            });

            Messenger.Default.Send(new GenericMessage<PromotionMoveTask>(this, promotionTask), NotificationMessages.AnimatePromotionMoveTask);
        }

        private void UndoMoveOnBoard(MoveData move)
        {
            ClearCurrentMoveData();

            MoveTask moveTask = new MoveTask(move);
            UndoMoveTask(moveTask);

            Messenger.Default.Send(new GenericMessage<MoveTask>(this, moveTask), NotificationMessages.AnimateUndoMoveTaskCreated);
            RefreshPositionStateMarkers();
        }

        private void ExecuteCurrentMoveOnBoard(bool useAnimations)
        {
            ClearCurrentMoveData();

            MoveData currentMove = analysisBoardService.GetCurrentMove();
            MoveTask moveTask = new MoveTask(currentMove);
            ExecuteMoveTask(moveTask);

            if (useAnimations)
                Messenger.Default.Send(new GenericMessage<MoveTask>(this, moveTask), NotificationMessages.AnimateMoveTaskCreated);
            else
                moveTask.OnTransitionCompleted();

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
                    mainPieceViewModel.Piece = moveTask.MoveData.Move.GetPromotionPiece();
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
