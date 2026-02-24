using System.Collections.Generic;

namespace BFME2.Core
{
    public class PlayerState
    {
        public int PlayerId { get; }
        public FactionId Faction { get; }
        public bool IsAI { get; }
        public AIDifficulty Difficulty { get; }

        public int CurrentResources { get; set; }
        public int UsedCommandPoints { get; set; }
        public int MaxCommandPoints { get; set; }
        public int AvailableCommandPoints => MaxCommandPoints - UsedCommandPoints;

        public int PowerPoints { get; set; }

        public List<IBattalion> OwnedUnits { get; } = new();
        public List<IBuilding> OwnedBuildings { get; } = new();
        public List<IHero> OwnedHeroes { get; } = new();

        public PlayerState(int playerId, FactionId faction, bool isAI, AIDifficulty difficulty,
            int startingResources, int maxCommandPoints)
        {
            PlayerId = playerId;
            Faction = faction;
            IsAI = isAI;
            Difficulty = difficulty;
            CurrentResources = startingResources;
            MaxCommandPoints = maxCommandPoints;
            UsedCommandPoints = 0;
            PowerPoints = 0;
        }
    }
}
