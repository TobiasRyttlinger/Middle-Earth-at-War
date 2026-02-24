using UnityEngine;

namespace BFME2.Units
{
    public static class FormationManager
    {
        /// <summary>
        /// Calculates world-space positions for each soldier based on the battalion's
        /// transform and the active formation definition.
        /// </summary>
        public static Vector3[] CalculateWorldPositions(
            FormationDefinition formation,
            int soldierCount,
            Transform battalionTransform,
            float spacing)
        {
            var localPositions = formation.GetPositionsForCount(soldierCount, spacing);
            var worldPositions = new Vector3[localPositions.Length];

            for (int i = 0; i < localPositions.Length; i++)
            {
                worldPositions[i] = battalionTransform.TransformPoint(localPositions[i]);
            }

            return worldPositions;
        }

        /// <summary>
        /// Collapses formation gaps when soldiers die — shifts remaining soldiers
        /// into the first available slots.
        /// </summary>
        public static Vector3[] CollapseFormation(
            FormationDefinition formation,
            int aliveSoldierCount,
            float spacing)
        {
            return formation.GetPositionsForCount(aliveSoldierCount, spacing);
        }

        /// <summary>
        /// Calculates a line formation facing a target direction.
        /// </summary>
        public static Vector3[] GetLineFormation(int count, float spacing, Vector3 forward)
        {
            var right = Vector3.Cross(Vector3.up, forward).normalized;
            var positions = new Vector3[count];
            float halfWidth = (count - 1) * spacing * 0.5f;

            for (int i = 0; i < count; i++)
            {
                positions[i] = right * (i * spacing - halfWidth);
            }

            return positions;
        }

        /// <summary>
        /// Calculates a wedge/V formation.
        /// </summary>
        public static Vector3[] GetWedgeFormation(int count, float spacing, Vector3 forward)
        {
            var right = Vector3.Cross(Vector3.up, forward).normalized;
            var positions = new Vector3[count];

            positions[0] = Vector3.zero; // Leader at front

            for (int i = 1; i < count; i++)
            {
                int side = (i % 2 == 0) ? 1 : -1;
                int depth = (i + 1) / 2;
                positions[i] = -forward * (depth * spacing) + right * (side * depth * spacing * 0.5f);
            }

            return positions;
        }
    }
}
