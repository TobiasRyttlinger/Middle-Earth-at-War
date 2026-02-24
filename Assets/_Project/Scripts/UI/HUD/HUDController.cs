using BFME2.Core;
using UnityEngine;
using UnityEngine.UI;

namespace BFME2.UI
{
    public class HUDController : MonoBehaviour
    {
        [Header("Resource Display")]
        [SerializeField] private Text _resourceText;
        [SerializeField] private Text _commandPointsText;

        [Header("Power Points")]
        [SerializeField] private Text _powerPointsText;

        [Header("Game Time")]
        [SerializeField] private Text _gameTimeText;

        private int _localPlayerId = 0;

        private void OnEnable()
        {
            GameEvents.OnResourcesChanged += HandleResourcesChanged;
            GameEvents.OnCommandPointsChanged += HandleCommandPointsChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnResourcesChanged -= HandleResourcesChanged;
            GameEvents.OnCommandPointsChanged -= HandleCommandPointsChanged;
        }

        private void Update()
        {
            // Update game time display
            if (_gameTimeText != null && GameManager.Instance != null)
            {
                float time = GameManager.Instance.GameTime;
                int minutes = Mathf.FloorToInt(time / 60f);
                int seconds = Mathf.FloorToInt(time % 60f);
                _gameTimeText.text = $"{minutes:00}:{seconds:00}";
            }
        }

        private void HandleResourcesChanged(int playerId, int newAmount)
        {
            if (playerId != _localPlayerId) return;
            if (_resourceText != null)
            {
                _resourceText.text = newAmount.ToString();
            }
        }

        private void HandleCommandPointsChanged(int playerId, int used, int max)
        {
            if (playerId != _localPlayerId) return;
            if (_commandPointsText != null)
            {
                _commandPointsText.text = $"{used}/{max}";
            }
        }

        public void UpdatePowerPoints(int points)
        {
            if (_powerPointsText != null)
            {
                _powerPointsText.text = points.ToString();
            }
        }

        public void SetLocalPlayerId(int playerId)
        {
            _localPlayerId = playerId;
        }
    }
}
