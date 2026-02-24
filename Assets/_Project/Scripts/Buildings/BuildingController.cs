using System.Collections.Generic;
using BFME2.Core;
using BFME2.Units;
using UnityEngine;

namespace BFME2.Buildings
{
    public class BuildingController : MonoBehaviour, IBuilding
    {
        [SerializeField] private BuildingDefinition _definition;

        private StateMachine _stateMachine;
        private readonly Queue<UnitDefinition> _productionQueue = new();
        private float _productionTimer;
        private UnitDefinition _currentProduction;
        private Vector3 _rallyPoint;

        // IBuilding
        public BuildingDefinition Definition => _definition;
        public float ConstructionProgress { get; private set; }
        public bool IsConstructed => ConstructionProgress >= 1f;
        public BuildingState CurrentBuildingState { get; private set; } = BuildingState.Constructing;

        // ISelectable
        public SelectableType Type => SelectableType.Building;
        public FactionId OwnerFaction { get; private set; }
        public int OwnerPlayerId { get; private set; }
        public Transform Transform => transform;
        public Bounds SelectionBounds => _definition != null
            ? new Bounds(transform.position, new Vector3(_definition.FootprintSize.x, 3f, _definition.FootprintSize.y))
            : new Bounds(transform.position, Vector3.one * 3f);
        public bool IsSelectable => CurrentBuildingState != BuildingState.Destroyed;

        // IDamageable
        public float CurrentHealth { get; private set; }
        public float MaxHealth => _definition != null ? _definition.MaxHealth : 0;
        public bool IsAlive => CurrentHealth > 0;

        // Production
        public IReadOnlyCollection<UnitDefinition> ProductionQueue => _productionQueue;
        public UnitDefinition CurrentProduction => _currentProduction;
        public float ProductionProgress => _currentProduction != null ? _productionTimer / _currentProduction.BuildTime : 0f;
        public Vector3 RallyPoint => _rallyPoint;

        // Resource generation
        public int ResourceLevel { get; private set; } = 1;

        private void Awake()
        {
            _stateMachine = new StateMachine();
        }

        public void Initialize(BuildingDefinition definition, int ownerPlayerId, FactionId faction)
        {
            _definition = definition;
            OwnerPlayerId = ownerPlayerId;
            OwnerFaction = faction;
            CurrentHealth = 0f; // Starts at 0, increases during construction
            ConstructionProgress = 0f;
            CurrentBuildingState = BuildingState.Constructing;

            _rallyPoint = transform.position + transform.forward * 5f;

            gameObject.layer = GameConstants.LAYER_INDEX_BUILDING;

            GameEvents.RaiseBuildingPlaced(this);
        }

        private void Update()
        {
            _stateMachine?.Tick(Time.deltaTime);

            if (CurrentBuildingState == BuildingState.Constructing)
            {
                AdvanceConstruction(Time.deltaTime);
            }
            else if (CurrentBuildingState == BuildingState.Active)
            {
                ProcessProduction(Time.deltaTime);
            }
        }

        public void AdvanceConstruction(float deltaTime)
        {
            if (IsConstructed || _definition == null) return;

            float rate = 1f / _definition.ConstructionTime;
            ConstructionProgress += rate * deltaTime;
            CurrentHealth = MaxHealth * ConstructionProgress;

            if (ConstructionProgress >= 1f)
            {
                CompleteConstruction();
            }
        }

        public void CompleteConstruction()
        {
            ConstructionProgress = 1f;
            CurrentHealth = MaxHealth;
            CurrentBuildingState = BuildingState.Active;

            GameEvents.RaiseBuildingCompleted(this);
        }

        public void TakeDamage(float amount, DamageTypeDefinition damageType, IDamageable source)
        {
            if (!IsAlive) return;

            CurrentHealth -= amount;

            if (CurrentHealth <= MaxHealth * 0.3f && CurrentBuildingState == BuildingState.Active)
            {
                CurrentBuildingState = BuildingState.Damaged;
            }

            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                DestroyBuilding();
            }

            GameEvents.RaiseDamageDealt(this, amount, source);
        }

        public void QueueUnit(UnitDefinition unit)
        {
            if (!IsConstructed || unit == null) return;

            if (ServiceLocator.TryGet<IResourceManager>(out var resources))
            {
                if (!resources.CanAfford(OwnerPlayerId, unit.ResourceCost)) return;
                if (!resources.HasCommandPoints(OwnerPlayerId, unit.CommandPointsCost)) return;

                resources.SpendResources(OwnerPlayerId, unit.ResourceCost);
            }

            _productionQueue.Enqueue(unit);

            if (_currentProduction == null)
            {
                StartNextProduction();
            }
        }

        public void CancelProduction()
        {
            if (_currentProduction == null) return;

            // Refund resources
            if (ServiceLocator.TryGet<IResourceManager>(out var resources))
            {
                resources.AddResources(OwnerPlayerId, _currentProduction.ResourceCost);
            }

            _currentProduction = null;
            _productionTimer = 0f;

            if (_productionQueue.Count > 0)
            {
                StartNextProduction();
            }
        }

        private void StartNextProduction()
        {
            if (_productionQueue.Count == 0)
            {
                _currentProduction = null;
                return;
            }

            _currentProduction = _productionQueue.Dequeue();
            _productionTimer = 0f;
        }

        private void ProcessProduction(float deltaTime)
        {
            if (_currentProduction == null) return;

            _productionTimer += deltaTime;

            if (_productionTimer >= _currentProduction.BuildTime)
            {
                // Spawn the unit
                if (BattalionFactory.Instance != null)
                {
                    BattalionFactory.Instance.CreateBattalion(
                        _currentProduction,
                        _rallyPoint,
                        OwnerPlayerId,
                        OwnerFaction
                    );
                }

                // Reserve command points
                if (ServiceLocator.TryGet<IResourceManager>(out var resources))
                {
                    resources.AddCommandPointUsage(OwnerPlayerId, _currentProduction.CommandPointsCost);
                }

                _currentProduction = null;
                _productionTimer = 0f;

                StartNextProduction();
            }
        }

        public void SetRallyPoint(Vector3 position)
        {
            _rallyPoint = position;
        }

        public void Demolish()
        {
            // Refund partial resources
            if (ServiceLocator.TryGet<IResourceManager>(out var resources))
            {
                int refund = Mathf.RoundToInt(_definition.ResourceCost * GameConstants.BUILDING_SELL_REFUND_PERCENT);
                resources.AddResources(OwnerPlayerId, refund);
            }

            DestroyBuilding();
        }

        private void DestroyBuilding()
        {
            CurrentBuildingState = BuildingState.Destroyed;

            // Cancel any production
            _currentProduction = null;
            _productionQueue.Clear();

            // Swap to destroyed visual if available
            if (_definition.DestroyedPrefab != null)
            {
                Instantiate(_definition.DestroyedPrefab, transform.position, transform.rotation);
            }

            GameEvents.RaiseBuildingDestroyed(this);

            Destroy(gameObject, 0.1f);
        }

        public void LevelUpResource()
        {
            if (_definition == null || !_definition.IsResourceGenerator) return;
            if (ResourceLevel >= _definition.MaxResourceLevel) return;
            ResourceLevel++;
        }

        public void OnSelected() { }
        public void OnDeselected() { }
    }
}
