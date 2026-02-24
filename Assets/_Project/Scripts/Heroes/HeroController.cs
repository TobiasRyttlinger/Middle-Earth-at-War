using System.Collections.Generic;
using BFME2.Core;
using UnityEngine;
using UnityEngine.AI;

namespace BFME2.Heroes
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class HeroController : MonoBehaviour, IHero
    {
        [SerializeField] private HeroDefinition _definition;

        private NavMeshAgent _agent;
        private StateMachine _stateMachine;
        private readonly Queue<ICommand> _commandQueue = new();
        private ICommand _currentCommand;
        private AbilityInstance[] _abilities;
        private bool _isDead;
        private float _reviveCountdown;

        // IHero
        public HeroDefinition Definition => _definition;
        public int Level { get; private set; } = 1;
        public float Experience { get; private set; }
        public bool CanRevive => _isDead;
        public float ReviveTimer => _reviveCountdown;
        public AbilityInstance[] Abilities => _abilities;

        // ISelectable
        public SelectableType Type => SelectableType.Hero;
        public FactionId OwnerFaction { get; private set; }
        public int OwnerPlayerId { get; private set; }
        public Transform Transform => transform;
        public Bounds SelectionBounds => new(transform.position, Vector3.one * 2f);
        public bool IsSelectable => IsAlive;

        // IDamageable
        public float CurrentHealth { get; private set; }
        public float MaxHealth => _definition != null ? _definition.MaxHealth : 0;
        public bool IsAlive => CurrentHealth > 0 && !_isDead;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _stateMachine = new StateMachine();
        }

        public void Initialize(HeroDefinition definition, int ownerPlayerId, FactionId faction)
        {
            _definition = definition;
            OwnerPlayerId = ownerPlayerId;
            OwnerFaction = faction;
            Level = 1;
            Experience = 0f;
            CurrentHealth = definition.MaxHealth;
            _isDead = false;

            _agent.speed = definition.MoveSpeed;

            gameObject.layer = GameConstants.LAYER_INDEX_HERO;

            if (definition.ModelScale != 1f)
            {
                transform.localScale = Vector3.one * definition.ModelScale;
            }

            // Initialize abilities
            InitializeAbilities();

            GameEvents.RaiseHeroSpawned(this);
        }

        private void InitializeAbilities()
        {
            if (_definition.Abilities == null)
            {
                _abilities = System.Array.Empty<AbilityInstance>();
                return;
            }

            _abilities = new AbilityInstance[_definition.Abilities.Length];
            for (int i = 0; i < _definition.Abilities.Length; i++)
            {
                int unlockLevel = (i < _definition.AbilityUnlockLevels.Length) ? _definition.AbilityUnlockLevels[i] : 1;
                _abilities[i] = new AbilityInstance(_definition.Abilities[i], this, unlockLevel);
            }
        }

        private void Update()
        {
            if (_isDead)
            {
                _reviveCountdown -= Time.deltaTime;
                return;
            }

            if (!IsAlive) return;

            _stateMachine.Tick(Time.deltaTime);

            // Tick ability cooldowns
            if (_abilities != null)
            {
                foreach (var ability in _abilities)
                {
                    ability.TickCooldown(Time.deltaTime);
                }
            }

            // Process commands
            if (_currentCommand != null)
            {
                _currentCommand.Tick(Time.deltaTime);
                if (_currentCommand.IsComplete)
                {
                    _currentCommand = null;
                    ProcessNextCommand();
                }
            }
        }

        public void IssueCommand(ICommand command, bool queue = false)
        {
            if (!IsAlive) return;

            if (queue)
            {
                _commandQueue.Enqueue(command);
            }
            else
            {
                _currentCommand?.Cancel();
                _commandQueue.Clear();
                _currentCommand = command;
                _currentCommand.Execute();
            }
        }

        private void ProcessNextCommand()
        {
            if (_commandQueue.Count > 0)
            {
                _currentCommand = _commandQueue.Dequeue();
                _currentCommand.Execute();
            }
        }

        public void TakeDamage(float amount, DamageTypeDefinition damageType, IDamageable source)
        {
            if (!IsAlive) return;

            CurrentHealth -= amount;

            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                Die();
            }

            GameEvents.RaiseDamageDealt(this, amount, source);
        }

        public void GainExperience(float amount)
        {
            Experience += amount;

            if (_definition.ExperienceThresholds != null && Level < _definition.MaxLevel)
            {
                int thresholdIndex = Level - 1;
                if (thresholdIndex < _definition.ExperienceThresholds.Length
                    && Experience >= _definition.ExperienceThresholds[thresholdIndex])
                {
                    LevelUp();
                }
            }
        }

        private void LevelUp()
        {
            Level++;

            // Apply stat multiplier
            if (_definition.LevelStatMultipliers != null && Level <= _definition.LevelStatMultipliers.Length)
            {
                float mult = _definition.LevelStatMultipliers[Level - 1];
                _agent.speed = _definition.MoveSpeed * mult;
            }

            GameEvents.RaiseHeroLevelUp(this, Level);
        }

        public void UseAbility(int abilityIndex)
        {
            if (_abilities == null || abilityIndex < 0 || abilityIndex >= _abilities.Length) return;

            var ability = _abilities[abilityIndex];
            if (!ability.IsUnlocked || !ability.IsReady) return;

            var context = new AbilityContext(gameObject, OwnerPlayerId);
            ability.Activate(context);
        }

        public void UseAbility(int abilityIndex, Vector3 targetPosition)
        {
            if (_abilities == null || abilityIndex < 0 || abilityIndex >= _abilities.Length) return;

            var ability = _abilities[abilityIndex];
            if (!ability.IsUnlocked || !ability.IsReady) return;

            var context = new AbilityContext(gameObject, OwnerPlayerId) { TargetPosition = targetPosition };
            ability.Activate(context);
        }

        public void UseAbility(int abilityIndex, IDamageable target)
        {
            if (_abilities == null || abilityIndex < 0 || abilityIndex >= _abilities.Length) return;

            var ability = _abilities[abilityIndex];
            if (!ability.IsUnlocked || !ability.IsReady) return;

            var context = new AbilityContext(gameObject, OwnerPlayerId)
            {
                Target = target.Transform.gameObject,
                TargetPosition = target.Transform.position
            };
            ability.Activate(context);
        }

        private void Die()
        {
            _isDead = true;
            _currentCommand?.Cancel();
            _commandQueue.Clear();

            _reviveCountdown = _definition.ReviveTime;

            // Disable visuals and collider
            var collider = GetComponent<Collider>();
            if (collider != null) collider.enabled = false;
            _agent.enabled = false;

            // Hide the model
            foreach (var renderer in GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = false;
            }

            GameEvents.RaiseEntityKilled(this);
            GameEvents.RaiseHeroDied(this);
        }

        public void Revive(Vector3 spawnPosition)
        {
            if (!_isDead) return;

            _isDead = false;
            CurrentHealth = MaxHealth * 0.5f; // Revive at half health
            transform.position = spawnPosition;

            // Re-enable everything
            var collider = GetComponent<Collider>();
            if (collider != null) collider.enabled = true;
            _agent.enabled = true;
            _agent.Warp(spawnPosition);

            foreach (var renderer in GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = true;
            }

            GameEvents.RaiseHeroRevived(this);
        }

        public void OnSelected() { }
        public void OnDeselected() { }
    }
}
