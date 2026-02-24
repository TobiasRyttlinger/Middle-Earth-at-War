using UnityEngine;

namespace BFME2.Units
{
    [CreateAssetMenu(menuName = "BFME2/Units/Formation Definition")]
    public class FormationDefinition : ScriptableObject
    {
        public string FormationId;
        public string DisplayName;
        public Sprite Icon;

        [Tooltip("Relative position offsets from the battalion center for each soldier slot.")]
        public Vector3[] RelativePositions;

        [Header("Modifiers")]
        [Tooltip("Movement speed modifier. 1.0 = normal.")]
        public float SpeedModifier = 1f;
        [Tooltip("Armor modifier. 1.0 = normal.")]
        public float ArmorModifier = 1f;
        [Tooltip("Damage modifier. 1.0 = normal.")]
        public float DamageModifier = 1f;

        /// <summary>
        /// Generates default grid positions for a given soldier count if no custom positions are set.
        /// </summary>
        public Vector3[] GetPositionsForCount(int soldierCount, float spacing)
        {
            if (RelativePositions != null && RelativePositions.Length >= soldierCount)
            {
                return RelativePositions;
            }

            // Generate a default rectangular formation
            int cols = Mathf.CeilToInt(Mathf.Sqrt(soldierCount));
            var positions = new Vector3[soldierCount];
            float halfWidth = (cols - 1) * spacing * 0.5f;

            for (int i = 0; i < soldierCount; i++)
            {
                int row = i / cols;
                int col = i % cols;
                positions[i] = new Vector3(
                    col * spacing - halfWidth,
                    0f,
                    -row * spacing
                );
            }

            return positions;
        }
    }
}
