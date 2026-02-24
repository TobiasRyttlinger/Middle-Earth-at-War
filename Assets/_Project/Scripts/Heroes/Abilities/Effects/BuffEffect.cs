using BFME2.Core;
using UnityEngine;

namespace BFME2.Heroes.Effects
{
    [CreateAssetMenu(menuName = "BFME2/Abilities/Effects/Buff Effect")]
    public class BuffEffect : AbilityEffect
    {
        [Header("Buff")]
        public float DamageMultiplier = 1.5f;
        public float ArmorMultiplier = 1.5f;
        public float SpeedMultiplier = 1.2f;
        public float Radius = 10f;

        public override void Apply(AbilityContext context)
        {
            // Buff all friendly units in radius
            var center = context.Caster.transform.position;
            var colliders = Physics.OverlapSphere(center, Radius, GameConstants.AttackableLayerMask);

            foreach (var col in colliders)
            {
                var damageable = col.GetComponentInParent<IDamageable>();
                if (damageable == null || !damageable.IsAlive) continue;
                if (damageable.OwnerPlayerId != context.CasterPlayerId) continue;

                // Apply speed buff to NavMeshAgent
                var agent = col.GetComponentInParent<UnityEngine.AI.NavMeshAgent>();
                if (agent != null)
                {
                    agent.speed *= SpeedMultiplier;
                }

                // Store original values for removal — in production, use a BuffComponent system
                // This is a placeholder showing the pattern
            }
        }

        public override void Remove(AbilityContext context)
        {
            // Remove buffs — in production, the BuffComponent would handle this
            var center = context.Caster.transform.position;
            var colliders = Physics.OverlapSphere(center, Radius, GameConstants.AttackableLayerMask);

            foreach (var col in colliders)
            {
                var damageable = col.GetComponentInParent<IDamageable>();
                if (damageable == null) continue;
                if (damageable.OwnerPlayerId != context.CasterPlayerId) continue;

                var agent = col.GetComponentInParent<UnityEngine.AI.NavMeshAgent>();
                if (agent != null)
                {
                    agent.speed /= SpeedMultiplier;
                }
            }
        }
    }
}
