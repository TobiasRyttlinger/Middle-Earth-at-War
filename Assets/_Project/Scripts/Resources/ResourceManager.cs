using System.Collections.Generic;
using BFME2.Core;
using UnityEngine;

namespace BFME2.Resources
{
    public class ResourceManager : MonoBehaviour, IResourceManager
    {
        private readonly Dictionary<int, int> _resources = new();
        private readonly Dictionary<int, int> _usedCommandPoints = new();
        private readonly Dictionary<int, int> _maxCommandPoints = new();

        private void Awake()
        {
            ServiceLocator.Register<IResourceManager>(this);
        }

        public void InitializePlayer(int playerId, int startingResources, int maxCp)
        {
            _resources[playerId] = startingResources;
            _usedCommandPoints[playerId] = 0;
            _maxCommandPoints[playerId] = maxCp;
        }

        public int GetResources(int playerId)
        {
            return _resources.TryGetValue(playerId, out int val) ? val : 0;
        }

        public int GetMaxCommandPoints(int playerId)
        {
            return _maxCommandPoints.TryGetValue(playerId, out int val) ? val : GameConstants.DEFAULT_MAX_COMMAND_POINTS;
        }

        public int GetUsedCommandPoints(int playerId)
        {
            return _usedCommandPoints.TryGetValue(playerId, out int val) ? val : 0;
        }

        public int GetAvailableCommandPoints(int playerId)
        {
            return GetMaxCommandPoints(playerId) - GetUsedCommandPoints(playerId);
        }

        public void AddResources(int playerId, int amount)
        {
            if (!_resources.ContainsKey(playerId))
                _resources[playerId] = 0;

            _resources[playerId] += amount;
            GameEvents.RaiseResourcesChanged(playerId, _resources[playerId]);
        }

        public void SpendResources(int playerId, int amount)
        {
            if (!_resources.ContainsKey(playerId)) return;

            _resources[playerId] = Mathf.Max(0, _resources[playerId] - amount);
            GameEvents.RaiseResourcesChanged(playerId, _resources[playerId]);
        }

        public bool CanAfford(int playerId, int amount)
        {
            return GetResources(playerId) >= amount;
        }

        public void AddCommandPointUsage(int playerId, int points)
        {
            if (!_usedCommandPoints.ContainsKey(playerId))
                _usedCommandPoints[playerId] = 0;

            _usedCommandPoints[playerId] += points;
            GameEvents.RaiseCommandPointsChanged(playerId, _usedCommandPoints[playerId], GetMaxCommandPoints(playerId));
        }

        public void RemoveCommandPointUsage(int playerId, int points)
        {
            if (!_usedCommandPoints.ContainsKey(playerId)) return;

            _usedCommandPoints[playerId] = Mathf.Max(0, _usedCommandPoints[playerId] - points);
            GameEvents.RaiseCommandPointsChanged(playerId, _usedCommandPoints[playerId], GetMaxCommandPoints(playerId));
        }

        public bool HasCommandPoints(int playerId, int points)
        {
            return GetAvailableCommandPoints(playerId) >= points;
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<IResourceManager>();
        }
    }
}
