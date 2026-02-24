using System.Collections.Generic;
using BFME2.Core;
using UnityEngine;

namespace BFME2.Powers
{
    public class PowerManager : MonoBehaviour
    {
        [SerializeField] private PowerTreeDefinition[] _factionPowerTrees;

        private readonly Dictionary<int, int> _powerPoints = new();
        private readonly Dictionary<int, List<PowerDefinition>> _purchasedPowers = new();
        private readonly Dictionary<string, float> _cooldowns = new(); // key: "{playerId}_{powerId}"

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        public void InitializePlayer(int playerId)
        {
            _powerPoints[playerId] = 0;
            _purchasedPowers[playerId] = new List<PowerDefinition>();
        }

        public int GetPowerPoints(int playerId)
        {
            return _powerPoints.TryGetValue(playerId, out int points) ? points : 0;
        }

        public void AddPowerPoints(int playerId, int points)
        {
            if (!_powerPoints.ContainsKey(playerId))
                _powerPoints[playerId] = 0;

            _powerPoints[playerId] += points;
        }

        public bool CanPurchasePower(int playerId, PowerDefinition power)
        {
            if (power == null) return false;

            int points = GetPowerPoints(playerId);
            if (points < power.PointCost) return false;

            // Check if already purchased
            if (_purchasedPowers.TryGetValue(playerId, out var purchased))
            {
                if (purchased.Contains(power)) return false;
            }

            return true;
        }

        public void PurchasePower(int playerId, PowerDefinition power)
        {
            if (!CanPurchasePower(playerId, power)) return;

            _powerPoints[playerId] -= power.PointCost;

            if (!_purchasedPowers.ContainsKey(playerId))
                _purchasedPowers[playerId] = new List<PowerDefinition>();

            _purchasedPowers[playerId].Add(power);
        }

        public void ActivatePower(int playerId, PowerDefinition power, Vector3 targetPosition)
        {
            if (power == null) return;

            string cooldownKey = $"{playerId}_{power.PowerId}";
            if (_cooldowns.TryGetValue(cooldownKey, out float remaining) && remaining > 0)
                return;

            // Apply effects
            var context = new AbilityContext
            {
                TargetPosition = targetPosition,
                CasterPlayerId = playerId,
                Duration = 0f
            };

            if (power.Effects != null)
            {
                foreach (var effect in power.Effects)
                {
                    effect?.Apply(context);
                }
            }

            // Spawn VFX
            if (power.VFXPrefab != null)
            {
                var fx = Instantiate(power.VFXPrefab, targetPosition, Quaternion.identity);
                Destroy(fx, 10f);
            }

            // Start cooldown
            _cooldowns[cooldownKey] = power.Cooldown;
        }

        public float GetPowerCooldown(int playerId, PowerDefinition power)
        {
            string key = $"{playerId}_{power.PowerId}";
            return _cooldowns.TryGetValue(key, out float remaining) ? remaining : 0f;
        }

        public List<PowerDefinition> GetPurchasedPowers(int playerId)
        {
            return _purchasedPowers.TryGetValue(playerId, out var list) ? list : new List<PowerDefinition>();
        }

        private void Update()
        {
            // Tick cooldowns
            var keys = new List<string>(_cooldowns.Keys);
            foreach (var key in keys)
            {
                _cooldowns[key] -= Time.deltaTime;
                if (_cooldowns[key] <= 0f)
                {
                    _cooldowns.Remove(key);
                }
            }
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<PowerManager>();
        }
    }
}
