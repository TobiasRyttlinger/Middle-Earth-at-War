namespace BFME2.Core
{
    public interface IAIController
    {
        void Initialize(int playerId, FactionId faction, AIDifficulty difficulty);
        void Tick(float deltaTime);
        bool IsActive { get; }
    }
}
