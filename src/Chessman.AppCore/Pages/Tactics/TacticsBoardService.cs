using ChessEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chessman
{
    public class TacticsBoardService : BoardService, ITacticsBoardService
    {
        private ITacticsService tacticsService = null;
        private Tactic currentTactic = null;
        private TacticState state = TacticState.NotStarted;
        public event EventHandler StateChanged;

        public TacticsBoardService(ITacticsService tacticsService)
        {
            this.tacticsService = tacticsService;
        }

        public async Task<Tactic> LoadTacticAsync()
        {
            currentTactic = await tacticsService.GetAsync();
            SetState(TacticState.InProgress);
            LoadFrom(currentTactic.fenBefore);

            return currentTactic;
        }

        public async Task ExecuteNextMoveAsync()
        {
            if (state != TacticState.InProgress)
                throw new Exception("Can't execute a move. Tactic is finished.");

            await Task.Delay(200);
            MoveData currentMove = GetCurrentMove();
            if (currentMove == null)
                SubmitMove(currentTactic.blunderMove);
            else
                SubmitMove(currentTactic.forcedLine[currentMove.Index]); // forced lines don't include blunder move
        }

        public void Restart()
        {
            SetState(TacticState.InProgress);
            LoadFrom(currentTactic.fenBefore);
        }

        public async Task SkipAsync()
        {
            await tacticsService.Skip();
        }

        public bool IsComputerTurn()
        {
            SideColor sideToMove = IsWhiteTurn ? SideColor.White : SideColor.Black;
            SideColor userColor = WasBlackFirstToMove() ? SideColor.White : SideColor.Black;
            if (sideToMove == userColor)
                return false;

            return true;
        }

        public TacticState GetState()
        {
            return state;
        }

        private void SetState(TacticState state)
        {
            this.state = state;
            NotifyStateChanged();
        }

        protected virtual void NotifyStateChanged()
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        //TODO: do we even need the ones from board vm?
        public override bool SubmitMove(string pgnMove)
        {
            if (state != TacticState.InProgress)
                return false;

            var result = base.SubmitMove(pgnMove);
            if (result)
                OnMoveExecuted();

            return result;
        }

        public override bool SubmitMove(Coordinate from, Coordinate to)
        {
            if (state != TacticState.InProgress)
                return false;

            var result = base.SubmitMove(from, to);
            if (result)
                OnMoveExecuted();

            return result;
        }

        private void OnMoveExecuted()
        {
            if (!IsComputerTurn())
                return;

            var currentMove = GetCurrentMove();

            if (currentMove.PgnMove != currentTactic.forcedLine[currentMove.Index - 1])
                SetState(TacticState.Failed);
            else if (currentTactic.forcedLine.Length == currentMove.Index)
                SetState(TacticState.Resolved);
        }
    }
}
