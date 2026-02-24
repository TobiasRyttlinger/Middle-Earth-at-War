using BFME2.Buildings;
using BFME2.Core;
using BFME2.Heroes;
using BFME2.Powers;
using BFME2.Units;
using UnityEngine;

namespace BFME2.Factions
{
    [CreateAssetMenu(menuName = "BFME2/Factions/Faction Definition")]
    public class FactionDefinition : ScriptableObject
    {
        [Header("Identity")]
        public FactionId FactionId;
        public string DisplayName;
        public Sprite FactionIcon;
        public Color FactionColor = Color.white;
        public Color MinimapColor = Color.blue;

        [Header("Buildings")]
        public BuildingDefinition FortressBuilding;
        public BuildingDefinition[] AvailableBuildings;
        public BuildingDefinition[] WallSegments;

        [Header("Units")]
        public UnitDefinition[] AvailableUnits;
        public HeroDefinition[] AvailableHeroes;

        [Header("Economy")]
        public BuildingDefinition ResourceBuilding;
        public int StartingResources = 2000;
        public int MaxCommandPoints = 300;

        [Header("Powers")]
        public PowerTreeDefinition PowerTree;

        [Header("Audio")]
        public AudioClip FactionTheme;
        public AudioClip[] AmbientSounds;
    }
}
