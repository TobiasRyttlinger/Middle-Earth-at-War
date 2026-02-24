using BFME2.Core;
using UnityEngine;

namespace BFME2.Buildings
{
    public class BuildingPlacementManager : MonoBehaviour
    {
        [SerializeField] private BuildPlotPlacementStrategy _plotStrategy;
        [SerializeField] private FreePlacementStrategy _freeStrategy;
        [SerializeField] private UnityEngine.Camera _mainCamera;

        private IInputManager _input;
        private IBuildingPlacementStrategy _activeStrategy;
        private BuildingDefinition _currentBuilding;
        private int _placingPlayerId;
        private Quaternion _buildingRotation = Quaternion.identity;

        public bool IsPlacing => _currentBuilding != null;
        public BuildingDefinition CurrentBuilding => _currentBuilding;

        private void Start()
        {
            _input = ServiceLocator.Get<IInputManager>();

            if (_mainCamera == null)
                _mainCamera = UnityEngine.Camera.main;

            ServiceLocator.Register(this);
        }

        public void StartPlacement(BuildingDefinition building, int ownerPlayerId)
        {
            if (building == null) return;

            _currentBuilding = building;
            _placingPlayerId = ownerPlayerId;
            _buildingRotation = Quaternion.identity;

            // Select strategy based on placement type
            _activeStrategy = building.PlacementType switch
            {
                BuildingPlacementType.BuildPlot => _plotStrategy,
                BuildingPlacementType.FreePlacement => _freeStrategy,
                _ => _freeStrategy
            };

            // Subscribe to build mode input
            if (_input != null)
            {
                _input.OnBuildPlaceInput += HandlePlaceInput;
                _input.OnBuildRotateInput += HandleRotateInput;
                _input.OnBuildCancelInput += CancelPlacement;
            }
        }

        private void Update()
        {
            if (!IsPlacing || _activeStrategy == null || _mainCamera == null) return;

            // Raycast to find placement position
            var ray = _mainCamera.ScreenPointToRay(_input.MouseScreenPosition);
            if (Physics.Raycast(ray, out RaycastHit hit, GameConstants.GROUND_RAYCAST_DISTANCE,
                GameConstants.TerrainLayerMask | (1 << GameConstants.LAYER_INDEX_BUILD_PLOT)))
            {
                var snappedPos = _activeStrategy.SnapPosition(hit.point);
                bool canPlace = _activeStrategy.CanPlace(_currentBuilding, snappedPos, _buildingRotation);
                _activeStrategy.ShowGhost(_currentBuilding, snappedPos, canPlace);
            }
        }

        private void HandlePlaceInput(Vector2 screenPos)
        {
            if (!IsPlacing || _activeStrategy == null) return;

            var ray = _mainCamera.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out RaycastHit hit, GameConstants.GROUND_RAYCAST_DISTANCE,
                GameConstants.TerrainLayerMask | (1 << GameConstants.LAYER_INDEX_BUILD_PLOT)))
            {
                var snappedPos = _activeStrategy.SnapPosition(hit.point);

                if (!_activeStrategy.CanPlace(_currentBuilding, snappedPos, _buildingRotation))
                    return;

                // Check affordability
                if (ServiceLocator.TryGet<IResourceManager>(out var resources))
                {
                    if (!resources.CanAfford(_placingPlayerId, _currentBuilding.ResourceCost))
                        return;

                    resources.SpendResources(_placingPlayerId, _currentBuilding.ResourceCost);
                }

                // Place the building
                PlaceBuilding(snappedPos);
            }
        }

        private void PlaceBuilding(Vector3 position)
        {
            // Instantiate the building
            GameObject buildingObj;
            if (_currentBuilding.ConstructionPrefab != null)
            {
                buildingObj = Instantiate(_currentBuilding.ConstructionPrefab, position, _buildingRotation);
            }
            else if (_currentBuilding.CompletedPrefab != null)
            {
                buildingObj = Instantiate(_currentBuilding.CompletedPrefab, position, _buildingRotation);
            }
            else
            {
                buildingObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                buildingObj.transform.position = position;
                buildingObj.transform.rotation = _buildingRotation;
                buildingObj.transform.localScale = new Vector3(
                    _currentBuilding.FootprintSize.x,
                    3f,
                    _currentBuilding.FootprintSize.y
                );
            }

            buildingObj.name = _currentBuilding.DisplayName;

            var controller = buildingObj.GetComponent<BuildingController>();
            if (controller == null)
                controller = buildingObj.AddComponent<BuildingController>();

            controller.Initialize(_currentBuilding, _placingPlayerId, (FactionId)_placingPlayerId);

            // If it's a build plot placement, occupy the plot
            if (_currentBuilding.PlacementType == BuildingPlacementType.BuildPlot && _plotStrategy != null)
            {
                var plot = _plotStrategy.GetPlotAtPosition(position);
                plot?.Occupy(controller);
            }

            // Clean up placement mode
            _activeStrategy.HideGhost();
            CancelPlacement();
        }

        private void HandleRotateInput(float angle)
        {
            _buildingRotation *= Quaternion.Euler(0f, angle, 0f);
        }

        public void CancelPlacement()
        {
            _activeStrategy?.HideGhost();
            _currentBuilding = null;
            _activeStrategy = null;

            if (_input != null)
            {
                _input.OnBuildPlaceInput -= HandlePlaceInput;
                _input.OnBuildRotateInput -= HandleRotateInput;
                _input.OnBuildCancelInput -= CancelPlacement;
            }
        }

        private void OnDestroy()
        {
            CancelPlacement();
            ServiceLocator.Unregister<BuildingPlacementManager>();
        }
    }
}
