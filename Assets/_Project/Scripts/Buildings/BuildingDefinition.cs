using BFME2.Core;
using BFME2.Units;
using UnityEngine;

namespace BFME2.Buildings
{
    [CreateAssetMenu(menuName = "BFME2/Buildings/Building Definition")]
    public class BuildingDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string BuildingId;
        public string DisplayName;
        public Sprite Icon;
        public FactionId Faction;

        [Header("Placement")]
        public BuildingPlacementType PlacementType;
        public Vector2Int FootprintSize = new(3, 3);
        public GameObject PlacementGhostPrefab;

        [Header("Stats")]
        public int MaxHealth = 2000;
        public int ResourceCost = 300;
        public float ConstructionTime = 15f;
        public ArmorTypeDefinition ArmorType;

        [Header("Construction")]
        public GameObject ConstructionPrefab;
        public GameObject CompletedPrefab;
        public GameObject DestroyedPrefab;
        public int ConstructionStages = 3;

        [Header("Production")]
        public UnitDefinition[] TrainableUnits;
        public string[] TrainableHeroIds; // Reference by ID since heroes are in a different assembly

        [Header("Economy")]
        public bool IsResourceGenerator;
        public float ResourceGenerationRate = 10f;
        public float ResourceGenerationInterval = 6f;
        public int MaxResourceLevel = 3;

        [Header("Garrison")]
        public bool CanGarrison;
        public int GarrisonCapacity;

        [Header("Audio")]
        public AudioClip ConstructionSound;
        public AudioClip CompletionSound;
        public AudioClip AmbientSound;

        [Header("Build Plot Config")]
        public BuildPlotSize RequiredPlotSize;
    }
}
