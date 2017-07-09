using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngineClient
{
    public interface IAnalysisBoardService : IEngineBoardService
    {
        void SetAnalysisLines(int linesCount);
    }
}
