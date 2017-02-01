﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine;
using System.Threading;

namespace ChessEngineClient
{
    public class AnalysisBoardService : BoardService, IEngineBoardService
    {
        private IEngine engine = null;
        private IEngineNotification engineNotification = null;
        private bool isStarted = false;

        public AnalysisBoardService(IEngineNotification engineNotification, IEngine engine)
        {
            this.engineNotification = engineNotification;
            this.engine = engine;
        }

        public override void ResetBoard()
        {
            base.ResetBoard();
            AnalyseCurrentPosition();
        }

        public override bool SubmitMove(Coordinate from, Coordinate to)
        {
            bool result = base.SubmitMove(from, to);
            if (result)
                AnalyseCurrentPosition();

            return result;
        }

        public override bool GoToMove(int moveIndex)
        {
            bool result = base.GoToMove(moveIndex);
            if (result)
                AnalyseCurrentPosition();

            return result;
        }

        public void Start()
        {
            // if is not maximum the analyze infinite command has some issues
            engine.SetOptions(new EngineOptions() { SkillLevel = 20 });

            AnalyseCurrentPosition();
            isStarted = true;
        }

        private void AnalyseCurrentPosition()
        {
            if (!ChessBoard.IsStalemate() && !ChessBoard.IsCheckmate())
            {
                engineNotification.OnStateChanged(EngineState.Analyze);
                engine.Analyze(ChessBoard, -1);
            }
            else
            {
                engineNotification.OnStateChanged(EngineState.Stop);
            }
        }

        public void Stop()
        {
            engine.StopAnalyzing();
            isStarted = false;
        }

        public override bool LoadFrom(string serializedValue)
        {
            if (isStarted)
                engine.StopAnalyzing();

            bool result = base.LoadFrom(serializedValue);

            if (isStarted)
                AnalyseCurrentPosition();

            return result;
        }
    }
}
