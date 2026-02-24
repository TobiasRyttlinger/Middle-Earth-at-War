using System;
using System.Collections.Generic;
using UnityEngine;

namespace BFME2.Core
{
    /// <summary>
    /// Static C# event bus for high-frequency code-driven events.
    /// Use this for events between systems that don't need SO assets.
    /// </summary>
    public static class GameEvents
    {
        // Selection
        public static event Action<IReadOnlyList<ISelectable>> OnSelectionChanged;
        public static void RaiseSelectionChanged(IReadOnlyList<ISelectable> selection)
            => OnSelectionChanged?.Invoke(selection);

        // Combat
        public static event Action<IDamageable, float, IDamageable> OnDamageDealt;
        public static void RaiseDamageDealt(IDamageable target, float amount, IDamageable source)
            => OnDamageDealt?.Invoke(target, amount, source);

        public static event Action<IDamageable> OnEntityKilled;
        public static void RaiseEntityKilled(IDamageable entity)
            => OnEntityKilled?.Invoke(entity);

        // Units
        public static event Action<IBattalion> OnUnitSpawned;
        public static void RaiseUnitSpawned(IBattalion battalion)
            => OnUnitSpawned?.Invoke(battalion);

        public static event Action<IBattalion> OnUnitDestroyed;
        public static void RaiseUnitDestroyed(IBattalion battalion)
            => OnUnitDestroyed?.Invoke(battalion);

        // Buildings
        public static event Action<IBuilding> OnBuildingPlaced;
        public static void RaiseBuildingPlaced(IBuilding building)
            => OnBuildingPlaced?.Invoke(building);

        public static event Action<IBuilding> OnBuildingCompleted;
        public static void RaiseBuildingCompleted(IBuilding building)
            => OnBuildingCompleted?.Invoke(building);

        public static event Action<IBuilding> OnBuildingDestroyed;
        public static void RaiseBuildingDestroyed(IBuilding building)
            => OnBuildingDestroyed?.Invoke(building);

        // Resources
        public static event Action<int, int> OnResourcesChanged; // playerId, newAmount
        public static void RaiseResourcesChanged(int playerId, int newAmount)
            => OnResourcesChanged?.Invoke(playerId, newAmount);

        public static event Action<int, int, int> OnCommandPointsChanged; // playerId, used, max
        public static void RaiseCommandPointsChanged(int playerId, int used, int max)
            => OnCommandPointsChanged?.Invoke(playerId, used, max);

        // Game State
        public static event Action OnGamePaused;
        public static void RaiseGamePaused() => OnGamePaused?.Invoke();

        public static event Action OnGameResumed;
        public static void RaiseGameResumed() => OnGameResumed?.Invoke();

        public static event Action<FactionId> OnGameOver;
        public static void RaiseGameOver(FactionId winner) => OnGameOver?.Invoke(winner);

        // Heroes
        public static event Action<IHero> OnHeroSpawned;
        public static void RaiseHeroSpawned(IHero hero) => OnHeroSpawned?.Invoke(hero);

        public static event Action<IHero> OnHeroDied;
        public static void RaiseHeroDied(IHero hero) => OnHeroDied?.Invoke(hero);

        public static event Action<IHero> OnHeroRevived;
        public static void RaiseHeroRevived(IHero hero) => OnHeroRevived?.Invoke(hero);

        public static event Action<IHero, int> OnHeroLevelUp; // hero, newLevel
        public static void RaiseHeroLevelUp(IHero hero, int newLevel)
            => OnHeroLevelUp?.Invoke(hero, newLevel);

        /// <summary>
        /// Call this on scene unload to prevent stale references.
        /// </summary>
        public static void ClearAll()
        {
            OnSelectionChanged = null;
            OnDamageDealt = null;
            OnEntityKilled = null;
            OnUnitSpawned = null;
            OnUnitDestroyed = null;
            OnBuildingPlaced = null;
            OnBuildingCompleted = null;
            OnBuildingDestroyed = null;
            OnResourcesChanged = null;
            OnCommandPointsChanged = null;
            OnGamePaused = null;
            OnGameResumed = null;
            OnGameOver = null;
            OnHeroSpawned = null;
            OnHeroDied = null;
            OnHeroRevived = null;
            OnHeroLevelUp = null;
        }
    }
}
