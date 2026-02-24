using System.Collections.Generic;
using BFME2.Core;
using UnityEngine;

namespace BFME2.Factions
{
    public class FactionManager : MonoBehaviour, IFactionManager
    {
        [SerializeField] private FactionDefinition[] _availableFactions;

        private readonly Dictionary<int, FactionId> _playerFactions = new();
        private readonly Dictionary<FactionId, FactionDefinition> _factionLookup = new();

        private void Awake()
        {
            ServiceLocator.Register<IFactionManager>(this);

            // Build lookup table
            if (_availableFactions != null)
            {
                foreach (var faction in _availableFactions)
                {
                    if (faction != null)
                    {
                        _factionLookup[faction.FactionId] = faction;
                    }
                }
            }
        }

        public void AssignFaction(int playerId, FactionId factionId)
        {
            _playerFactions[playerId] = factionId;
        }

        public FactionId GetPlayerFactionId(int playerId)
        {
            return _playerFactions.TryGetValue(playerId, out var factionId) ? factionId : FactionId.Gondor;
        }

        public FactionDefinition GetFaction(FactionId factionId)
        {
            return _factionLookup.TryGetValue(factionId, out var def) ? def : null;
        }

        public FactionDefinition GetPlayerFaction(int playerId)
        {
            var factionId = GetPlayerFactionId(playerId);
            return GetFaction(factionId);
        }

        public FactionDefinition[] GetAllFactions()
        {
            return _availableFactions;
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<IFactionManager>();
        }
    }
}
