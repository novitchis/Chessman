using ChessEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient
{
    public delegate void AnalysisEventHandler(object sender, AnalysisEventArgs e);

    public class AnalysisEventArgs: EventArgs
    {
        public AnalysisData Data { get; set; }
    }
}
