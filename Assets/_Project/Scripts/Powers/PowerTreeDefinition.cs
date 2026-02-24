using BFME2.Core;
using UnityEngine;

namespace BFME2.Powers
{
    [CreateAssetMenu(menuName = "BFME2/Powers/Power Tree Definition")]
    public class PowerTreeDefinition : ScriptableObject
    {
        public string TreeId;
        public PowerDefinition[] Tier1Powers;
        public PowerDefinition[] Tier2Powers;
        public PowerDefinition[] Tier3Powers;

        [Tooltip("Power points needed to unlock each tier")]
        public int[] TierPointRequirements = { 0, 5, 15 };
    }

    [CreateAssetMenu(menuName = "BFME2/Powers/Power Definition")]
    public class PowerDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string PowerId;
        public string DisplayName;
        [TextArea] public string Description;
        public Sprite Icon;

        [Header("Cost & Cooldown")]
        public int PointCost = 1;
        public float Cooldown = 120f;

        [Header("Targeting")]
        public PowerTargetType TargetType;
        public float AreaRadius = 15f;

        [Header("Effects")]
        public AbilityEffect[] Effects;

        [Header("Visuals & Audio")]
        public GameObject VFXPrefab;
        public AudioClip ActivationSound;
    }
}
