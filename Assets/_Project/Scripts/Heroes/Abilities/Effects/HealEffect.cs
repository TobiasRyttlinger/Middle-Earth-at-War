using BFME2.Core;
using UnityEngine;

namespace BFME2.Heroes.Effects
{
    [CreateAssetMenu(menuName = "BFME2/Abilities/Effects/Heal Effect")]
    public class HealEffect : AbilityEffect
    {
        [Header("Healing")]
        public float HealAmount = 300f;
        public float Radius = 8f;
        public bool HealSelf = true;

        public override void Apply(AbilityContext context)
        {
            var center = context.TargetPosition != Vector3.zero
                ? context.TargetPosition
                : context.Caster.transform.position;

            var colliders = Physics.OverlapSphere(center, Radius, GameConstants.AttackableLayerMask);

            foreach (var col in colliders)
            {
                var damageable = col.GetComponentInParent<IDamageable>();
                if (damageable == null || !damageable.IsAlive) continue;

                // Only heal friendlies
                if (damageable.OwnerPlayerId != context.CasterPlayerId) continue;

                if (!HealSelf && damageable.Transform.gameObject == context.Caster) continue;

                // "Heal" by dealing negative damage (or we need a Heal method on IDamageable)
                // For now, this is a placeholder — production code would add IHealable interface
                float healNeeded = damageable.MaxHealth - damageable.CurrentHealth;
                float actualHeal = Mathf.Min(HealAmount, healNeeded);

                // Apply heal — would be done via a proper heal interface
                // damageable.Heal(actualHeal);
            }
        }
    }
}
