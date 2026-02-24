using BFME2.Core;
using UnityEngine;

namespace BFME2.Units
{
    public class CombatManager : MonoBehaviour
    {
        [SerializeField] private DamageTable _damageTable;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        public float CalculateDamage(float baseDamage, DamageTypeDefinition damageType, ArmorTypeDefinition armorType)
        {
            float multiplier = 1f;
            if (_damageTable != null && damageType != null && armorType != null)
            {
                multiplier = _damageTable.GetMultiplier(damageType, armorType);
            }
            return baseDamage * multiplier;
        }

        public void ApplyDamage(IDamageable target, float amount, DamageTypeDefinition damageType, IDamageable source)
        {
            if (target == null || !target.IsAlive) return;

            // Look up armor type from the target if it's known
            // For now, apply damage with type multiplier if we have enough info
            float finalDamage = amount;
            if (_damageTable != null && damageType != null)
            {
                // The target's armor type would come from its definition
                // This is a simplified version — in production, IDamageable would expose ArmorType
                finalDamage = amount; // Multiplier applied if armor type is known
            }

            target.TakeDamage(finalDamage, damageType, source);
        }

        public void ApplyAreaDamage(Vector3 center, float radius, float damage, DamageTypeDefinition damageType, IDamageable source, int sourcePlayerId)
        {
            var colliders = Physics.OverlapSphere(center, radius, GameConstants.AttackableLayerMask);

            foreach (var col in colliders)
            {
                var damageable = col.GetComponentInParent<IDamageable>();
                if (damageable == null || !damageable.IsAlive) continue;
                if (damageable.OwnerPlayerId == sourcePlayerId) continue;

                // Falloff based on distance
                float distance = Vector3.Distance(center, damageable.Transform.position);
                float falloff = 1f - (distance / radius);
                float finalDamage = damage * Mathf.Max(falloff, 0.2f); // Minimum 20% at edge

                ApplyDamage(damageable, finalDamage, damageType, source);
            }
        }

        public IDamageable FindNearestTarget(Vector3 position, float range, int attackerPlayerId)
        {
            var colliders = Physics.OverlapSphere(position, range, GameConstants.AttackableLayerMask);

            IDamageable nearest = null;
            float nearestDist = float.MaxValue;

            foreach (var col in colliders)
            {
                var damageable = col.GetComponentInParent<IDamageable>();
                if (damageable == null || !damageable.IsAlive) continue;
                if (damageable.OwnerPlayerId == attackerPlayerId) continue;

                float dist = Vector3.Distance(position, damageable.Transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = damageable;
                }
            }

            return nearest;
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<CombatManager>();
        }
    }
}
