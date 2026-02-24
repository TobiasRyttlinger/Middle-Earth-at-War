using UnityEngine;

namespace BFME2.Core
{
    [CreateAssetMenu(menuName = "BFME2/Game Settings")]
    public class GameSettings : ScriptableObject
    {
        [Header("Players")]
        public int PlayerCount = 2;
        public FactionId[] PlayerFactions = { FactionId.Gondor, FactionId.Mordor };
        public bool[] IsAI = { false, true };
        public AIDifficulty[] AIDifficulties = { AIDifficulty.Medium, AIDifficulty.Medium };

        [Header("Economy")]
        public int StartingResources = GameConstants.DEFAULT_STARTING_RESOURCES;
        public int MaxCommandPoints = GameConstants.DEFAULT_MAX_COMMAND_POINTS;

        [Header("Game Speed")]
        [Range(0.5f, 3f)]
        public float GameSpeed = 1f;

        [Header("Map")]
        public string MapSceneName;
    }
}
