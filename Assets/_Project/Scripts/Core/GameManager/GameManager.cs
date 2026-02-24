using System.Collections.Generic;
using UnityEngine;

namespace BFME2.Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameSettings _defaultSettings;

        public static GameManager Instance { get; private set; }

        public GameState CurrentState { get; private set; } = GameState.Loading;
        public GameSettings CurrentSettings { get; private set; }
        public float GameTime { get; private set; }
        public int LocalPlayerId { get; private set; } = 0;

        private readonly Dictionary<int, PlayerState> _playerStates = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void InitializeGame(GameSettings settings = null)
        {
            CurrentSettings = settings != null ? settings : _defaultSettings;
            if (CurrentSettings == null)
            {
                Debug.LogError("[GameManager] No GameSettings provided.");
                return;
            }

            CurrentState = GameState.Initializing;
            GameTime = 0f;
            _playerStates.Clear();

            for (int i = 0; i < CurrentSettings.PlayerCount; i++)
            {
                var faction = CurrentSettings.PlayerFactions[i];
                var isAI = CurrentSettings.IsAI[i];
                var difficulty = CurrentSettings.AIDifficulties[i];

                var state = new PlayerState(
                    i, faction, isAI, difficulty,
                    CurrentSettings.StartingResources,
                    CurrentSettings.MaxCommandPoints
                );
                _playerStates[i] = state;
            }

            CurrentState = GameState.Playing;
            Time.timeScale = CurrentSettings.GameSpeed;
        }

        private void Update()
        {
            if (CurrentState == GameState.Playing)
            {
                GameTime += Time.deltaTime;
            }
        }

        public PlayerState GetPlayerState(int playerId)
        {
            _playerStates.TryGetValue(playerId, out var state);
            return state;
        }

        public IReadOnlyDictionary<int, PlayerState> GetAllPlayerStates() => _playerStates;

        public void PauseGame()
        {
            if (CurrentState != GameState.Playing) return;
            CurrentState = GameState.Paused;
            Time.timeScale = 0f;
            GameEvents.RaiseGamePaused();
        }

        public void ResumeGame()
        {
            if (CurrentState != GameState.Paused) return;
            CurrentState = GameState.Playing;
            Time.timeScale = CurrentSettings.GameSpeed;
            GameEvents.RaiseGameResumed();
        }

        public void EndGame(FactionId winner)
        {
            CurrentState = GameState.GameOver;
            Time.timeScale = 0f;
            GameEvents.RaiseGameOver(winner);
        }

        public void SetGameSpeed(float speed)
        {
            if (CurrentSettings != null)
            {
                // Modify speed at runtime (won't persist to SO)
                Time.timeScale = Mathf.Clamp(speed, 0.5f, 3f);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                GameEvents.ClearAll();
                ServiceLocator.Clear();
                Instance = null;
            }
        }
    }
}
