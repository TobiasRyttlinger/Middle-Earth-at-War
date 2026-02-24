namespace BFME2.Core
{
    public interface ICommand
    {
        void Execute();
        void Cancel();
        bool IsComplete { get; }
        void Tick(float deltaTime);
    }
}
