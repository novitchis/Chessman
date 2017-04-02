﻿using ChessEngine;
using Framework.MVVM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChessEngineClient
{
    public class PracticeBoardService : BoardService, IPracticeBoardService
    {
        private SynchronizationContext mainSynchronizationContext = null;
        private IEngine engine = null;
        private IAnalysisReceiver analysisReceiver = null;
        private int depth = -1;
        private int moveTime = -1;
        private bool isStarted = false;

        public SideColor UserColor
        {
            get; private set;
        }

        public PracticeBoardService(IEngine engine, IAnalysisReceiver analysisReceiver)
        {
            UserColor = SideColor.White;
            this.engine = engine;
            this.analysisReceiver = analysisReceiver;

            mainSynchronizationContext = SynchronizationContext.Current;
        }

        public override bool SubmitMove(Coordinate from, Coordinate to)
        {
            if (GetIsComputerTurn())
                return false;

            return base.SubmitMove(from, to);
        }

        public void SwitchUserColor()
        {
            bool wasStarted = isStarted;
            if (wasStarted)
                Stop();

            UserColor = UserColor == SideColor.Black ? SideColor.White : SideColor.Black;

            if (wasStarted)
                Start();
        }

        public bool GetIsComputerTurn()
        {
            SideColor sideToMove = IsWhiteTurn ? SideColor.White : SideColor.Black;
            if (sideToMove == UserColor)
                return false;

            return true;
        }

        public void RequestComputerMove()
        {
            if (ChessBoard.IsStalemate() || ChessBoard.IsCheckmate())
                return;

            engine.Analyze(ChessBoard, depth, moveTime);
        }

        public void Start()
        {
            analysisReceiver.AnalysisReceived += OnAnalysisReceived;
            isStarted = true;

            if (!GetIsComputerTurn())
                return;

            RequestComputerMove();
        }

        private void OnAnalysisReceived(object sender, AnalysisEventArgs e)
        {
            if (e.Data.IsBestMove)
            {
                base.SubmitMove(e.Data.Analysis[0].GetFrom(), e.Data.Analysis[0].GetTo());
                mainSynchronizationContext.Post(o =>
                {
                    // TODO: this should be done differently
                    Messenger.Default.Send(new MessageBase(this, this), NotificationMessages.MoveExecuted);
                }, null);
            }
        }

        public void Stop()
        {
            analysisReceiver.AnalysisReceived -= OnAnalysisReceived;
            engine.StopAnalyzing();
            isStarted = false;
        }

        public void SetEngineStrength(int strengthValue)
        {
            // reajust the engine skill level since the level 2 is already too strong
            // strength(1..10) - skill level(0..20)
            int skillLevel = 0;

            switch (strengthValue)
            {
                case 1:
                    skillLevel = 3; depth = 1; moveTime = 50; break;
                case 2:
                    skillLevel = 6; depth = 2; moveTime = 100; break;
                case 3:
                    skillLevel = 9; depth = 3; moveTime = 150; break;
                case 4:
                    skillLevel = 11; depth = 4; moveTime = 200; break;
                case 5:
                    skillLevel = 14; depth = 5; moveTime = 250; break;
                case 6:
                    skillLevel = 16; depth = 7; moveTime = 300; break;
                case 7:
                    skillLevel = 18; depth = 10; moveTime = 350; break;
                case 8:
                    skillLevel = 18; depth = 12; moveTime = 400; break;
                case 9:
                    skillLevel = 20; depth = 14; moveTime = 500; break;
                case 10:
                    skillLevel = 20; depth = 16; moveTime = 1000; break;
                default:
                    break;
            }

            engine.SetOptions(new EngineOptions() { SkillLevel = skillLevel });
        }
    }
}
