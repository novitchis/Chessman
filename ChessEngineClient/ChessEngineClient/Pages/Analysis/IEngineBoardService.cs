namespace ChessEngineClient
{
    public interface IEngineBoardService : IBoardService
    {
        void Start();

        void Stop();
    }
}