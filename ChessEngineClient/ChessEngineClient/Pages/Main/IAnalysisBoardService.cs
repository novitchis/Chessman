namespace ChessEngineClient
{
    public interface IAnalysisBoardService : IBoardService
    {
        void StartAnalysis();

        void StopAnalysis();
    }
}