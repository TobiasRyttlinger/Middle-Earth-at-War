using BFME2.Buildings;
using BFME2.Core;
using BFME2.Factions;
using BFME2.Units;
using UnityEngine;

namespace BFME2.AI
{
    public class AIController : MonoBehaviour, IAIController
    {
        [Header("AI Tuning")]
        [SerializeField] private float _decisionInterval = 5f;
        [SerializeField] private float _buildInterval = 15f;
        [SerializeField] private float _trainInterval = 10f;
        [SerializeField] private float _attackInterval = 60f;

        private int _playerId;
        private FactionId _faction;
        private AIDifficulty _difficulty;
        private float _decisionTimer;
        private float _buildTimer;
        private float _trainTimer;
        private float _attackTimer;

        public bool IsActive { get; private set; }

        public void Initialize(int playerId, FactionId faction, AIDifficulty difficulty)
        {
            _playerId = playerId;
            _faction = faction;
            _difficulty = difficulty;
            IsActive = true;

            // Adjust intervals based on difficulty
            float difficultyMultiplier = difficulty switch
            {
                AIDifficulty.Easy => 2f,
                AIDifficulty.Medium => 1f,
                AIDifficulty.Hard => 0.7f,
                AIDifficulty.Brutal => 0.4f,
                _ => 1f
            };

            _decisionInterval *= difficultyMultiplier;
            _buildInterval *= difficultyMultiplier;
            _trainInterval *= difficultyMultiplier;
            _attackInterval *= difficultyMultiplier;
        }

        public void Tick(float deltaTime)
        {
            if (!IsActive) return;

            _decisionTimer += deltaTime;
            _buildTimer += deltaTime;
            _trainTimer += deltaTime;
            _attackTimer += deltaTime;

            if (_decisionTimer >= _decisionInterval)
            {
                _decisionTimer = 0f;
                MakeDecision();
            }
        }

        private void Update()
        {
            Tick(Time.deltaTime);
        }

        private void MakeDecision()
        {
            // Placeholder AI decision-making
            // Priority: Build economy -> Build army -> Attack

            if (_buildTimer >= _buildInterval)
            {
                _buildTimer = 0f;
                TryBuildSomething();
            }

            if (_trainTimer >= _trainInterval)
            {
                _trainTimer = 0f;
                TryTrainUnits();
            }

            if (_attackTimer >= _attackInterval)
            {
                _attackTimer = 0f;
                TryAttack();
            }
        }

        private void TryBuildSomething()
        {
            // Placeholder: Would check available build plots, resources,
            // and faction building list to make a build decision
            Debug.Log($"[AI P{_playerId}] Considering building...");
        }

        private void TryTrainUnits()
        {
            // Placeholder: Would find player's barracks/production buildings
            // and queue unit training
            Debug.Log($"[AI P{_playerId}] Considering training units...");
        }

        private void TryAttack()
        {
            // Placeholder: Would gather army and issue attack-move toward enemy base
            Debug.Log($"[AI P{_playerId}] Considering attack...");
        }
    }
}
