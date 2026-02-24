using BFME2.Core;
using UnityEngine;

namespace BFME2.Units
{
    [CreateAssetMenu(menuName = "BFME2/Units/Unit Definition")]
    public class UnitDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string UnitId;
        public string DisplayName;
        public Sprite Portrait;
        public FactionId Faction;

        [Header("Battalion")]
        public int SoldiersPerBattalion = 10;
        public float SoldierSpacing = 1.5f;
        public FormationDefinition DefaultFormation;

        [Header("Stats")]
        public int HealthPerSoldier = 100;
        public float MoveSpeed = 5f;
        public float RotationSpeed = 120f;
        public float NavMeshAgentRadius = 3f;
        public int CommandPointsCost = 20;
        public int ResourceCost = 200;
        public float BuildTime = 10f;

        [Header("Combat")]
        public DamageTypeDefinition DamageType;
        public float AttackDamage = 15f;
        public float AttackSpeed = 1f;
        public float AttackRange = 2f;
        public ArmorTypeDefinition ArmorType;

        [Header("Visuals")]
        public GameObject SoldierPrefab;
        public GameObject BattalionPrefab;
        public GameObject DeathEffect;

        [Header("Audio")]
        public AudioClip[] SelectionVoices;
        public AudioClip[] MoveVoices;
        public AudioClip[] AttackVoices;
        public AudioClip[] DeathSounds;

        [Header("Experience")]
        public int[] ExperienceThresholds = { 100, 300, 600, 1000 };
        public float[] LevelBonusMultipliers = { 1f, 1.1f, 1.2f, 1.35f, 1.5f };
    }
}
