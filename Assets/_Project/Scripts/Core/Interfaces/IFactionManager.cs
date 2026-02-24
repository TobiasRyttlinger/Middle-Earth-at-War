namespace BFME2.Core
{
    public interface IFactionManager
    {
        void AssignFaction(int playerId, FactionId factionId);
        FactionId GetPlayerFactionId(int playerId);
    }
}
