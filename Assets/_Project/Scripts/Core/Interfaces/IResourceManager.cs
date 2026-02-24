namespace BFME2.Core
{
    public interface IResourceManager
    {
        int GetResources(int playerId);
        int GetMaxCommandPoints(int playerId);
        int GetUsedCommandPoints(int playerId);
        int GetAvailableCommandPoints(int playerId);
        void AddResources(int playerId, int amount);
        void SpendResources(int playerId, int amount);
        bool CanAfford(int playerId, int amount);
        void AddCommandPointUsage(int playerId, int points);
        void RemoveCommandPointUsage(int playerId, int points);
        bool HasCommandPoints(int playerId, int points);
    }
}
