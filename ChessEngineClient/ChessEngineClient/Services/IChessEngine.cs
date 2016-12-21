﻿using ChessEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient
{
    public interface IChessEngine
    {
        void SetNotificationsProvider(IEngineNotification notifications);

        bool Start();
        bool Stop();
        bool Analyze(ChessBoard board);
        bool StopAnalyzing();
    }
}
