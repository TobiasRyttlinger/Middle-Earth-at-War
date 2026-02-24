using BFME2.Core;
using BFME2.Powers;
using UnityEngine;
using UnityEngine.UI;

namespace BFME2.UI
{
    public class PowerTreePanel : MonoBehaviour
    {
        [SerializeField] private Transform _tier1Container;
        [SerializeField] private Transform _tier2Container;
        [SerializeField] private Transform _tier3Container;
        [SerializeField] private GameObject _powerButtonPrefab;
        [SerializeField] private Text _powerPointsText;

        private PowerManager _powerManager;
        private PowerTreeDefinition _tree;
        private int _localPlayerId;

        public void Initialize(PowerTreeDefinition tree, int playerId)
        {
            _tree = tree;
            _localPlayerId = playerId;
            _powerManager = ServiceLocator.TryGet<PowerManager>(out var pm) ? pm : null;

            if (_powerManager == null)
                _powerManager = Object.FindAnyObjectByType<PowerManager>();

            PopulateTree();
        }

        private void PopulateTree()
        {
            if (_tree == null) return;

            PopulateTier(_tier1Container, _tree.Tier1Powers);
            PopulateTier(_tier2Container, _tree.Tier2Powers);
            PopulateTier(_tier3Container, _tree.Tier3Powers);
        }

        private void PopulateTier(Transform container, PowerDefinition[] powers)
        {
            if (container == null || powers == null || _powerButtonPrefab == null) return;

            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }

            foreach (var power in powers)
            {
                if (power == null) continue;

                var buttonObj = Instantiate(_powerButtonPrefab, container);
                buttonObj.name = $"Power_{power.DisplayName}";

                var text = buttonObj.GetComponentInChildren<Text>();
                if (text != null)
                {
                    text.text = $"{power.DisplayName}\n({power.PointCost}pp)";
                }

                var image = buttonObj.GetComponentInChildren<Image>();
                if (image != null && power.Icon != null)
                {
                    image.sprite = power.Icon;
                }

                var button = buttonObj.GetComponent<Button>();
                var powerRef = power;
                if (button != null)
                {
                    button.onClick.AddListener(() => OnPowerClicked(powerRef));
                }
            }
        }

        private void OnPowerClicked(PowerDefinition power)
        {
            if (_powerManager == null) return;

            var purchased = _powerManager.GetPurchasedPowers(_localPlayerId);
            if (purchased.Contains(power))
            {
                // Already purchased — activate it
                // Would need to switch to targeting mode for area powers
                _powerManager.ActivatePower(_localPlayerId, power, Vector3.zero);
            }
            else if (_powerManager.CanPurchasePower(_localPlayerId, power))
            {
                _powerManager.PurchasePower(_localPlayerId, power);
            }
        }

        private void Update()
        {
            if (_powerManager != null && _powerPointsText != null)
            {
                _powerPointsText.text = _powerManager.GetPowerPoints(_localPlayerId).ToString();
            }
        }
    }
}
