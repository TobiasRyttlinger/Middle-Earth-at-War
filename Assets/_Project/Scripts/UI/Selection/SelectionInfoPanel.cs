using System.Collections.Generic;
using BFME2.Buildings;
using BFME2.Core;
using BFME2.Heroes;
using BFME2.Units;
using UnityEngine;
using UnityEngine.UI;

namespace BFME2.UI
{
    public class SelectionInfoPanel : MonoBehaviour
    {
        [Header("Single Selection")]
        [SerializeField] private GameObject _singleSelectionPanel;
        [SerializeField] private Image _portraitImage;
        [SerializeField] private Text _nameText;
        [SerializeField] private Slider _healthBar;
        [SerializeField] private Text _healthText;
        [SerializeField] private Text _levelText;

        [Header("Multi Selection")]
        [SerializeField] private GameObject _multiSelectionPanel;
        [SerializeField] private Transform _multiPortraitContainer;
        [SerializeField] private GameObject _miniPortraitPrefab;

        [Header("Building Selection")]
        [SerializeField] private GameObject _buildingSelectionPanel;
        [SerializeField] private Text _buildingNameText;
        [SerializeField] private Slider _buildingHealthBar;
        [SerializeField] private Slider _constructionBar;
        [SerializeField] private Transform _productionButtonContainer;

        private void OnEnable()
        {
            GameEvents.OnSelectionChanged += HandleSelectionChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnSelectionChanged -= HandleSelectionChanged;
        }

        private void HandleSelectionChanged(IReadOnlyList<ISelectable> selection)
        {
            HideAll();

            if (selection == null || selection.Count == 0) return;

            if (selection.Count == 1)
            {
                var selected = selection[0];
                switch (selected.Type)
                {
                    case SelectableType.Unit:
                        DisplayUnit(selected as IBattalion);
                        break;
                    case SelectableType.Hero:
                        DisplayHero(selected as IHero);
                        break;
                    case SelectableType.Building:
                        DisplayBuilding(selected as IBuilding);
                        break;
                }
            }
            else
            {
                DisplayMultiSelection(selection);
            }
        }

        public void DisplayUnit(IBattalion battalion)
        {
            if (battalion == null) return;

            _singleSelectionPanel?.SetActive(true);

            if (_nameText != null)
                _nameText.text = $"{battalion.AliveSoldiersCount}/{battalion.MaxSoldiers}";

            UpdateHealthBar(_healthBar, _healthText, battalion.CurrentHealth, battalion.MaxHealth);

            if (_levelText != null)
                _levelText.text = $"Lvl {battalion.Level}";
        }

        public void DisplayHero(IHero hero)
        {
            if (hero == null) return;

            _singleSelectionPanel?.SetActive(true);

            UpdateHealthBar(_healthBar, _healthText, hero.CurrentHealth, hero.MaxHealth);

            if (_levelText != null)
                _levelText.text = $"Lvl {hero.Level}";
        }

        public void DisplayBuilding(IBuilding building)
        {
            if (building == null) return;

            _buildingSelectionPanel?.SetActive(true);

            UpdateHealthBar(_buildingHealthBar, null, building.CurrentHealth, building.MaxHealth);

            if (_constructionBar != null)
            {
                _constructionBar.gameObject.SetActive(!building.IsConstructed);
                _constructionBar.value = building.ConstructionProgress;
            }
        }

        public void DisplayMultiSelection(IReadOnlyList<ISelectable> selection)
        {
            _multiSelectionPanel?.SetActive(true);

            // Clear existing portraits
            if (_multiPortraitContainer != null)
            {
                foreach (Transform child in _multiPortraitContainer)
                {
                    Destroy(child.gameObject);
                }
            }

            // Create mini portraits for each selected unit
            if (_miniPortraitPrefab != null && _multiPortraitContainer != null)
            {
                foreach (var selectable in selection)
                {
                    Instantiate(_miniPortraitPrefab, _multiPortraitContainer);
                }
            }
        }

        public void Clear()
        {
            HideAll();
        }

        private void HideAll()
        {
            _singleSelectionPanel?.SetActive(false);
            _multiSelectionPanel?.SetActive(false);
            _buildingSelectionPanel?.SetActive(false);
        }

        private void UpdateHealthBar(Slider bar, Text text, float current, float max)
        {
            if (bar != null)
            {
                bar.maxValue = max;
                bar.value = current;
            }
            if (text != null)
            {
                text.text = $"{Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";
            }
        }
    }
}
