using BFME2.Core;
using UnityEngine;

namespace BFME2.Heroes
{
    public class AbilityInstance
    {
        public AbilityDefinition Definition { get; }
        public float CurrentCooldown { get; private set; }
        public bool IsReady => CurrentCooldown <= 0f;
        public bool IsUnlocked => _heroController.Level >= _unlockLevel;

        private readonly HeroController _heroController;
        private readonly int _unlockLevel;
        private bool _isActive;
        private float _activeTimer;
        private AbilityContext _activeContext;

        public AbilityInstance(AbilityDefinition definition, HeroController hero, int unlockLevel)
        {
            Definition = definition;
            _heroController = hero;
            _unlockLevel = unlockLevel;
            CurrentCooldown = 0f;
        }

        public void Activate(AbilityContext context)
        {
            if (!IsReady || !IsUnlocked) return;

            context.Duration = Definition.Duration;
            _activeContext = context;
            _isActive = true;
            _activeTimer = 0f;

            // Apply all effects
            if (Definition.Effects != null)
            {
                foreach (var effect in Definition.Effects)
                {
                    if (effect != null)
                    {
                        effect.Apply(context);
                    }
                }
            }

            // Spawn VFX
            if (Definition.VFXPrefab != null)
            {
                var pos = context.TargetPosition != Vector3.zero
                    ? context.TargetPosition
                    : context.Caster.transform.position;

                var fx = Object.Instantiate(Definition.VFXPrefab, pos, Quaternion.identity);
                Object.Destroy(fx, Definition.Duration > 0 ? Definition.Duration : 3f);
            }

            // Start cooldown
            CurrentCooldown = Definition.Cooldown;
        }

        public void TickCooldown(float deltaTime)
        {
            if (CurrentCooldown > 0f)
            {
                CurrentCooldown -= deltaTime;
                if (CurrentCooldown < 0f) CurrentCooldown = 0f;
            }

            // Tick active effects
            if (_isActive)
            {
                _activeTimer += deltaTime;

                if (Definition.Effects != null)
                {
                    foreach (var effect in Definition.Effects)
                    {
                        effect?.Tick(_activeContext, deltaTime);
                    }
                }

                if (Definition.Duration > 0 && _activeTimer >= Definition.Duration)
                {
                    Deactivate();
                }
            }
        }

        private void Deactivate()
        {
            _isActive = false;

            if (Definition.Effects != null)
            {
                foreach (var effect in Definition.Effects)
                {
                    effect?.Remove(_activeContext);
                }
            }
        }
    }
}
