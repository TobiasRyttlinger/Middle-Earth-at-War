using UnityEngine;

namespace BFME2.Core
{
    [CreateAssetMenu(menuName = "BFME2/Combat/Damage Type")]
    public class DamageTypeDefinition : ScriptableObject
    {
        public string TypeId;
        public string DisplayName;
    }

    [CreateAssetMenu(menuName = "BFME2/Combat/Armor Type")]
    public class ArmorTypeDefinition : ScriptableObject
    {
        public string TypeId;
        public string DisplayName;
    }

    [CreateAssetMenu(menuName = "BFME2/Combat/Damage Table")]
    public class DamageTable : ScriptableObject
    {
        public DamageMultiplierEntry[] Entries;

        /// <summary>
        /// Looks up the damage multiplier for a given damage type vs armor type.
        /// Returns 1.0 if no entry is found.
        /// </summary>
        public float GetMultiplier(DamageTypeDefinition damageType, ArmorTypeDefinition armorType)
        {
            if (Entries == null) return 1f;

            foreach (var entry in Entries)
            {
                if (entry.DamageType == damageType && entry.ArmorType == armorType)
                {
                    return entry.Multiplier;
                }
            }
            return 1f;
        }
    }

    [System.Serializable]
    public struct DamageMultiplierEntry
    {
        public DamageTypeDefinition DamageType;
        public ArmorTypeDefinition ArmorType;
        [Tooltip("0.5 = half damage, 1.0 = normal, 2.0 = double")]
        public float Multiplier;
    }
}
