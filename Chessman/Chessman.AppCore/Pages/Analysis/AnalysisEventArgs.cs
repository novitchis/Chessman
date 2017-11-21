using ChessEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chessman
{
    public delegate void AnalysisEventHandler(object sender, AnalysisEventArgs e);

    public class AnalysisEventArgs: EventArgs
    {
        public AnalysisData[] AnalysisLines { get; set; }
    }


    public delegate void AnalysisStateEventHandler(object sender, AnalysisStateEventArgs e);
    public class AnalysisStateEventArgs: EventArgs
    {
        public EngineState NewState { get; private set; }

        public AnalysisStateEventArgs(EngineState state)
        {
            NewState = state;
        }

    }
}
