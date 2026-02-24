using BFME2.Buildings;
using UnityEngine;
using UnityEngine.UI;

namespace BFME2.UI
{
    public class ProductionQueueUI : MonoBehaviour
    {
        [SerializeField] private Transform _queueContainer;
        [SerializeField] private GameObject _queueItemPrefab;
        [SerializeField] private Slider _productionProgressBar;
        [SerializeField] private Text _productionNameText;
        [SerializeField] private Button _cancelButton;

        private BuildingController _building;

        public void SetBuilding(BuildingController building)
        {
            _building = building;
        }

        private void Update()
        {
            if (_building == null || !_building.IsConstructed)
            {
                ClearDisplay();
                return;
            }

            // Update current production progress
            if (_building.CurrentProduction != null)
            {
                if (_productionProgressBar != null)
                {
                    _productionProgressBar.gameObject.SetActive(true);
                    _productionProgressBar.value = _building.ProductionProgress;
                }

                if (_productionNameText != null)
                {
                    _productionNameText.text = _building.CurrentProduction.DisplayName;
                }
            }
            else
            {
                if (_productionProgressBar != null)
                    _productionProgressBar.gameObject.SetActive(false);

                if (_productionNameText != null)
                    _productionNameText.text = "";
            }
        }

        public void OnCancelClicked()
        {
            _building?.CancelProduction();
        }

        private void ClearDisplay()
        {
            if (_productionProgressBar != null)
                _productionProgressBar.gameObject.SetActive(false);

            if (_productionNameText != null)
                _productionNameText.text = "";

            if (_queueContainer != null)
            {
                foreach (Transform child in _queueContainer)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        private void OnEnable()
        {
            if (_cancelButton != null)
            {
                _cancelButton.onClick.AddListener(OnCancelClicked);
            }
        }

        private void OnDisable()
        {
            if (_cancelButton != null)
            {
                _cancelButton.onClick.RemoveListener(OnCancelClicked);
            }
        }
    }
}
