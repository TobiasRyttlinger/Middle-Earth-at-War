using UnityEngine;

namespace BFME2.Core
{
    /// <summary>
    /// Base class for modular ability effects. Create concrete subclasses
    /// (DamageEffect, HealEffect, BuffEffect, etc.) as ScriptableObjects
    /// that can be stacked in an AbilityDefinition.
    /// </summary>
    public abstract class AbilityEffect : ScriptableObject
    {
        public abstract void Apply(AbilityContext context);
        public virtual void Remove(AbilityContext context) { }
        public virtual void Tick(AbilityContext context, float deltaTime) { }
    }

    /// <summary>
    /// Runtime context passed to ability effects during execution.
    /// </summary>
    public struct AbilityContext
    {
        public GameObject Caster;
        public GameObject Target;
        public Vector3 TargetPosition;
        public float Duration;
        public int CasterPlayerId;

        public AbilityContext(GameObject caster, int casterPlayerId)
        {
            Caster = caster;
            Target = null;
            TargetPosition = Vector3.zero;
            Duration = 0f;
            CasterPlayerId = casterPlayerId;
        }
    }
}
