using BFME2.Core;
using UnityEngine;

namespace BFME2.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject _hudPanel;
        [SerializeField] private GameObject _buildMenuPanel;
        [SerializeField] private GameObject _powerTreePanel;
        [SerializeField] private GameObject _pauseMenuPanel;
        [SerializeField] private GameObject _gameOverPanel;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            // Start with HUD visible, everything else hidden
            ShowPanel(UIPanel.HUD);
            HidePanel(UIPanel.BuildMenu);
            HidePanel(UIPanel.PowerTree);
            HidePanel(UIPanel.PauseMenu);
            HidePanel(UIPanel.GameOver);

            GameEvents.OnGamePaused += () => ShowPanel(UIPanel.PauseMenu);
            GameEvents.OnGameResumed += () => HidePanel(UIPanel.PauseMenu);
            GameEvents.OnGameOver += (winner) => ShowPanel(UIPanel.GameOver);
        }

        public void ShowPanel(UIPanel panel)
        {
            var go = GetPanelObject(panel);
            if (go != null) go.SetActive(true);
        }

        public void HidePanel(UIPanel panel)
        {
            var go = GetPanelObject(panel);
            if (go != null) go.SetActive(false);
        }

        public void TogglePanel(UIPanel panel)
        {
            var go = GetPanelObject(panel);
            if (go != null) go.SetActive(!go.activeSelf);
        }

        private GameObject GetPanelObject(UIPanel panel)
        {
            return panel switch
            {
                UIPanel.HUD => _hudPanel,
                UIPanel.BuildMenu => _buildMenuPanel,
                UIPanel.PowerTree => _powerTreePanel,
                UIPanel.PauseMenu => _pauseMenuPanel,
                UIPanel.GameOver => _gameOverPanel,
                _ => null
            };
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<UIManager>();
        }
    }
}
