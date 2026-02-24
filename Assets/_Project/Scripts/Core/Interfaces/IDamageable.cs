using UnityEngine;

namespace BFME2.Core
{
    public interface IDamageable
    {
        float CurrentHealth { get; }
        float MaxHealth { get; }
        bool IsAlive { get; }
        Transform Transform { get; }
        int OwnerPlayerId { get; }
        void TakeDamage(float amount, DamageTypeDefinition damageType, IDamageable source);
    }
}
