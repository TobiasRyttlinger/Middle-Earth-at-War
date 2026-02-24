using BFME2.Buildings;
using BFME2.Core;
using UnityEngine;

namespace BFME2.Resources
{
    public class ResourceGenerator : MonoBehaviour
    {
        private BuildingController _building;
        private float _timer;
        private bool _isGenerating;

        public int CurrentLevel => _building != null ? _building.ResourceLevel : 1;

        public float EfficiencyMultiplier
        {
            get
            {
                return CurrentLevel switch
                {
                    1 => 1f,
                    2 => GameConstants.RESOURCE_LEVEL_2_MULTIPLIER,
                    3 => GameConstants.RESOURCE_LEVEL_3_MULTIPLIER,
                    _ => 1f
                };
            }
        }

        public float GenerationRate
        {
            get
            {
                if (_building == null || _building.Definition == null) return 0f;
                return _building.Definition.ResourceGenerationRate * EfficiencyMultiplier;
            }
        }

        private void Awake()
        {
            _building = GetComponent<BuildingController>();
        }

        private void Start()
        {
            // Auto-level timer for farms/furnaces
            GameEvents.OnBuildingCompleted += OnBuildingCompleted;
        }

        private void OnBuildingCompleted(IBuilding building)
        {
            if (building == (IBuilding)_building)
            {
                StartGenerating();
            }
        }

        public void StartGenerating()
        {
            _isGenerating = true;
            _timer = 0f;
        }

        public void StopGenerating()
        {
            _isGenerating = false;
        }

        private void Update()
        {
            if (!_isGenerating || _building == null || !_building.IsConstructed) return;

            var def = _building.Definition;
            if (def == null || !def.IsResourceGenerator) return;

            _timer += Time.deltaTime;

            if (_timer >= def.ResourceGenerationInterval)
            {
                _timer -= def.ResourceGenerationInterval;

                int amount = Mathf.RoundToInt(GenerationRate);

                if (ServiceLocator.TryGet<IResourceManager>(out var resources))
                {
                    resources.AddResources(_building.OwnerPlayerId, amount);
                }
            }

            // Auto-level over time (BFME2 farms/furnaces level up automatically)
            if (_building.ResourceLevel < def.MaxResourceLevel)
            {
                // Simple time-based leveling — level up every 60 seconds
                // You can make this more sophisticated
            }
        }

        private void OnDestroy()
        {
            GameEvents.OnBuildingCompleted -= OnBuildingCompleted;
        }
    }
}
