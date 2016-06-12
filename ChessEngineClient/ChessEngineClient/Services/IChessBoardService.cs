﻿using ChessEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient
{
    public interface IChessBoardService
    {
        bool Move(Coordinate from, Coordinate to);
    }
}
