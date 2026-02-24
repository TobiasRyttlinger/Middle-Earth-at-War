using BFME2.Buildings;
using BFME2.Core;
using BFME2.Factions;
using UnityEngine;
using UnityEngine.UI;

namespace BFME2.UI
{
    public class BuildMenuController : MonoBehaviour
    {
        [SerializeField] private Transform _buttonContainer;
        [SerializeField] private GameObject _buildButtonPrefab;

        private BuildingDefinition[] _buildings;
        private int _localPlayerId;

        public void PopulateMenu(BuildingDefinition[] buildings, int playerId)
        {
            _buildings = buildings;
            _localPlayerId = playerId;

            // Clear existing buttons
            if (_buttonContainer != null)
            {
                foreach (Transform child in _buttonContainer)
                {
                    Destroy(child.gameObject);
                }
            }

            if (buildings == null || _buildButtonPrefab == null || _buttonContainer == null) return;

            foreach (var building in buildings)
            {
                if (building == null) continue;

                var buttonObj = Instantiate(_buildButtonPrefab, _buttonContainer);
                buttonObj.name = $"Build_{building.DisplayName}";

                // Set button text/icon
                var text = buttonObj.GetComponentInChildren<Text>();
                if (text != null)
                {
                    text.text = $"{building.DisplayName}\n({building.ResourceCost})";
                }

                var image = buttonObj.GetComponentInChildren<Image>();
                if (image != null && building.Icon != null)
                {
                    image.sprite = building.Icon;
                }

                // Set button click handler
                var button = buttonObj.GetComponent<Button>();
                var buildingRef = building; // Capture for closure
                if (button != null)
                {
                    button.onClick.AddListener(() => OnBuildingSelected(buildingRef));
                }
            }
        }

        private void OnBuildingSelected(BuildingDefinition building)
        {
            if (ServiceLocator.TryGet<IResourceManager>(out var resources))
            {
                if (!resources.CanAfford(_localPlayerId, building.ResourceCost))
                {
                    Debug.Log($"[BuildMenu] Cannot afford {building.DisplayName}");
                    return;
                }
            }

            // Start placement mode
            var placementManager = ServiceLocator.TryGet<BuildingPlacementManager>(out var pm) ? pm : null;
            if (placementManager == null)
            {
                placementManager = Object.FindAnyObjectByType<BuildingPlacementManager>();
            }

            placementManager?.StartPlacement(building, _localPlayerId);
        }

        public void UpdateAffordability()
        {
            if (_buildings == null || _buttonContainer == null) return;

            var buttons = _buttonContainer.GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length && i < _buildings.Length; i++)
            {
                bool canAfford = true;
                if (ServiceLocator.TryGet<IResourceManager>(out var resources))
                {
                    canAfford = resources.CanAfford(_localPlayerId, _buildings[i].ResourceCost);
                }
                buttons[i].interactable = canAfford;
            }
        }

        private void Update()
        {
            // Periodically update affordability
            UpdateAffordability();
        }
    }
}
