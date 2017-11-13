namespace Chessman
{
    public interface IEngineBoardService : IBoardService
    {
        void Start();

        void Stop();
    }
}