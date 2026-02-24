using BFME2.Core;
using UnityEngine;

namespace BFME2.Buildings
{
    public class WallSegment : MonoBehaviour, IDamageable
    {
        [SerializeField] private WallSegmentType _type = WallSegmentType.Wall;
        [SerializeField] private float _maxHealth = 500f;
        [SerializeField] private ArmorTypeDefinition _armorType;

        public WallSegmentType Type => _type;
        public float CurrentHealth { get; private set; }
        public float MaxHealth => _maxHealth;
        public bool IsAlive => CurrentHealth > 0;
        public Transform Transform => transform;
        public int OwnerPlayerId { get; private set; }
        public bool IsGate => _type == WallSegmentType.Gate;
        public bool IsGateOpen { get; private set; }

        private void Awake()
        {
            gameObject.layer = GameConstants.LAYER_INDEX_WALL;
            CurrentHealth = _maxHealth;
        }

        public void Initialize(int ownerPlayerId, float maxHealth)
        {
            OwnerPlayerId = ownerPlayerId;
            _maxHealth = maxHealth;
            CurrentHealth = maxHealth;
        }

        public void TakeDamage(float amount, DamageTypeDefinition damageType, IDamageable source)
        {
            if (!IsAlive) return;

            CurrentHealth -= amount;
            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                DestroySegment();
            }
        }

        public void ToggleGate()
        {
            if (!IsGate) return;
            IsGateOpen = !IsGateOpen;
            // Animation/visual update would happen here
        }

        private void DestroySegment()
        {
            // Play destruction effect, swap to rubble mesh, etc.
            Destroy(gameObject, 0.5f);
        }
    }
}
