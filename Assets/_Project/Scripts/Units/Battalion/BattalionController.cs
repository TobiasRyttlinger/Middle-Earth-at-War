using System.Collections.Generic;
using BFME2.Core;
using UnityEngine;
using UnityEngine.AI;

namespace BFME2.Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class BattalionController : MonoBehaviour, IBattalion
    {
        [SerializeField] private UnitDefinition _definition;

        private NavMeshAgent _agent;
        private StateMachine _stateMachine;
        private BattalionMovement _movement;
        private UnitCombatHandler _combatHandler;
        private FormationManager _formationManager;
        private readonly List<SoldierInstance> _soldiers = new();
        private readonly Queue<ICommand> _commandQueue = new();
        private ICommand _currentCommand;
        private bool _isSelected;

        // IBattalion
        public int AliveSoldiersCount { get; private set; }
        public int MaxSoldiers => _definition != null ? _definition.SoldiersPerBattalion : 0;
        public int Level { get; private set; } = 1;
        public float Experience { get; private set; }
        public UnitDefinition Definition => _definition;
        public StateMachine StateMachine => _stateMachine;
        public BattalionMovement Movement => _movement;
        public UnitCombatHandler CombatHandler => _combatHandler;
        public IReadOnlyList<SoldierInstance> Soldiers => _soldiers;
        public FormationDefinition CurrentFormation { get; private set; }

        // ISelectable
        public SelectableType Type => SelectableType.Unit;
        public FactionId OwnerFaction { get; private set; }
        public int OwnerPlayerId { get; private set; }
        public Transform Transform => transform;
        public Bounds SelectionBounds => new(transform.position, Vector3.one * _definition.NavMeshAgentRadius * 2f);
        public bool IsSelectable => IsAlive;

        // IDamageable
        public float CurrentHealth { get; private set; }
        public float MaxHealth => _definition != null ? _definition.HealthPerSoldier * _definition.SoldiersPerBattalion : 0;
        public bool IsAlive => AliveSoldiersCount > 0;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _movement = GetComponent<BattalionMovement>();
            _combatHandler = GetComponent<UnitCombatHandler>();
            _stateMachine = new StateMachine();
        }

        public void Initialize(UnitDefinition definition, int ownerPlayerId, FactionId faction)
        {
            _definition = definition;
            OwnerPlayerId = ownerPlayerId;
            OwnerFaction = faction;
            Level = 1;
            Experience = 0f;

            // Configure NavMeshAgent
            _agent.speed = definition.MoveSpeed;
            _agent.angularSpeed = definition.RotationSpeed;
            _agent.radius = definition.NavMeshAgentRadius;

            CurrentFormation = definition.DefaultFormation;
            AliveSoldiersCount = definition.SoldiersPerBattalion;
            CurrentHealth = MaxHealth;

            // Initialize state machine with idle state
            var idleState = new UnitIdleState(this);
            _stateMachine.ChangeState(idleState);

            GameEvents.RaiseUnitSpawned(this);
        }

        private void Update()
        {
            if (!IsAlive) return;

            _stateMachine.Tick(Time.deltaTime);

            // Process command queue
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
                // Cancel current command and clear queue
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

            // Distribute damage to soldiers
            float damagePerSoldier = amount / AliveSoldiersCount;
            for (int i = _soldiers.Count - 1; i >= 0; i--)
            {
                if (_soldiers[i].IsAlive)
                {
                    _soldiers[i].TakeDamage(damagePerSoldier);
                    if (!_soldiers[i].IsAlive)
                    {
                        AliveSoldiersCount--;
                    }
                }
            }

            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                AliveSoldiersCount = 0;
                Die();
            }

            GameEvents.RaiseDamageDealt(this, amount, source);
        }

        public void SetFormation(FormationDefinition formation)
        {
            CurrentFormation = formation;
            UpdateSoldierPositions();
        }

        public void GainExperience(float amount)
        {
            Experience += amount;

            if (_definition.ExperienceThresholds != null && Level <= _definition.ExperienceThresholds.Length)
            {
                if (Experience >= _definition.ExperienceThresholds[Level - 1])
                {
                    LevelUp();
                }
            }
        }

        private void LevelUp()
        {
            Level++;
            // Apply level bonuses from definition
            if (_definition.LevelBonusMultipliers != null && Level <= _definition.LevelBonusMultipliers.Length)
            {
                float multiplier = _definition.LevelBonusMultipliers[Level - 1];
                _agent.speed = _definition.MoveSpeed * multiplier;
            }
        }

        public void RegisterSoldier(SoldierInstance soldier)
        {
            _soldiers.Add(soldier);
        }

        public void UpdateSoldierPositions()
        {
            if (CurrentFormation == null || _soldiers.Count == 0) return;

            var positions = CurrentFormation.GetPositionsForCount(AliveSoldiersCount, _definition.SoldierSpacing);
            int posIndex = 0;

            for (int i = 0; i < _soldiers.Count && posIndex < positions.Length; i++)
            {
                if (_soldiers[i].IsAlive)
                {
                    _soldiers[i].UpdateFormationPosition(positions[posIndex]);
                    posIndex++;
                }
            }
        }

        private void Die()
        {
            _currentCommand?.Cancel();
            _commandQueue.Clear();

            var dyingState = new UnitDyingState(this);
            _stateMachine.ChangeState(dyingState);

            GameEvents.RaiseEntityKilled(this);
            GameEvents.RaiseUnitDestroyed(this);
        }

        public void OnSelected()
        {
            _isSelected = true;
        }

        public void OnDeselected()
        {
            _isSelected = false;
        }
    }
}
