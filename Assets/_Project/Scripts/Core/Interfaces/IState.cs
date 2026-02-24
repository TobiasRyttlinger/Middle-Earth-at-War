namespace BFME2.Core
{
    public interface IState
    {
        void Enter();
        void Tick(float deltaTime);
        void Exit();
    }
}
