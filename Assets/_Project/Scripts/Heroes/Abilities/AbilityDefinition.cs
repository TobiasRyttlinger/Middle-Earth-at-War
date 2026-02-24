using BFME2.Core;
using UnityEngine;

namespace BFME2.Heroes
{
    [CreateAssetMenu(menuName = "BFME2/Abilities/Ability Definition")]
    public class AbilityDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string AbilityId;
        public string DisplayName;
        [TextArea] public string Description;
        public Sprite Icon;

        [Header("Targeting")]
        public AbilityTargetType TargetType;
        public float Range = 10f;
        public float AreaRadius = 5f;

        [Header("Timing")]
        public float Cooldown = 30f;
        public float Duration = 10f;
        public float CastTime = 0.5f;

        [Header("Effects")]
        public AbilityEffect[] Effects;

        [Header("Visuals & Audio")]
        public GameObject VFXPrefab;
        public AudioClip CastSound;
    }
}
