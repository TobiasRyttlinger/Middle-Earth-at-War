using BFME2.Core;
using BFME2.Units;
using UnityEngine;

namespace BFME2.Heroes.Effects
{
    [CreateAssetMenu(menuName = "BFME2/Abilities/Effects/Damage Effect")]
    public class DamageEffect : AbilityEffect
    {
        [Header("Damage")]
        public float Damage = 200f;
        public DamageTypeDefinition DamageType;
        public float Radius = 5f;
        public bool AffectsAllies = false;

        public override void Apply(AbilityContext context)
        {
            if (Radius > 0)
            {
                // Area damage
                var colliders = Physics.OverlapSphere(
                    context.TargetPosition != Vector3.zero ? context.TargetPosition : context.Caster.transform.position,
                    Radius,
                    GameConstants.AttackableLayerMask
                );

                foreach (var col in colliders)
                {
                    var damageable = col.GetComponentInParent<IDamageable>();
                    if (damageable == null || !damageable.IsAlive) continue;
                    if (!AffectsAllies && damageable.OwnerPlayerId == context.CasterPlayerId) continue;

                    damageable.TakeDamage(Damage, DamageType, null);
                }
            }
            else if (context.Target != null)
            {
                // Single target damage
                var damageable = context.Target.GetComponentInParent<IDamageable>();
                damageable?.TakeDamage(Damage, DamageType, null);
            }
        }
    }
}
