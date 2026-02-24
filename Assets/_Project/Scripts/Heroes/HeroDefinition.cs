using BFME2.Core;
using UnityEngine;

namespace BFME2.Heroes
{
    [CreateAssetMenu(menuName = "BFME2/Heroes/Hero Definition")]
    public class HeroDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string HeroId;
        public string DisplayName;
        [TextArea] public string Lore;
        public Sprite Portrait;
        public Sprite FullArt;
        public FactionId Faction;

        [Header("Stats")]
        public int MaxHealth = 2000;
        public float MoveSpeed = 6f;
        public int CommandPointsCost = 50;
        public int ResourceCost = 1500;
        public float ReviveTime = 60f;
        public int ReviveCost = 500;

        [Header("Combat")]
        public DamageTypeDefinition DamageType;
        public float AttackDamage = 80f;
        public float AttackSpeed = 1.2f;
        public float AttackRange = 2.5f;
        public ArmorTypeDefinition ArmorType;

        [Header("Abilities")]
        public AbilityDefinition[] Abilities;
        public int[] AbilityUnlockLevels = { 1, 2, 4, 7, 10 };

        [Header("Experience")]
        public int MaxLevel = 10;
        public int[] ExperienceThresholds = { 50, 150, 300, 500, 750, 1000, 1300, 1700, 2200 };
        public float[] LevelStatMultipliers = { 1f, 1.05f, 1.1f, 1.15f, 1.2f, 1.3f, 1.4f, 1.5f, 1.6f, 1.75f };

        [Header("Visuals")]
        public GameObject HeroPrefab;
        public GameObject MountedPrefab;
        public float ModelScale = 1f;

        [Header("Audio")]
        public AudioClip[] SelectionVoices;
        public AudioClip[] MoveVoices;
        public AudioClip[] AttackVoices;
        public AudioClip[] AbilityVoices;
    }
}
