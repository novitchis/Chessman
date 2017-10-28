﻿using ChessEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient.ViewModel
{
    public interface ICoordinatedItem
    {
        Coordinate Coordinate { get; }
    }
}
