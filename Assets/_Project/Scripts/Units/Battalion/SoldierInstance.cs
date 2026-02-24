using UnityEngine;

namespace BFME2.Units
{
    public class SoldierInstance : MonoBehaviour
    {
        public float CurrentHealth { get; private set; }
        public float MaxHealth { get; private set; }
        public int FormationIndex { get; private set; }
        public bool IsAlive { get; private set; } = true;

        [SerializeField] private float _positionLerpSpeed = 8f;

        private Vector3 _targetLocalPosition;
        private Animator _animator;
        private static readonly int AnimIsMoving = Animator.StringToHash("IsMoving");
        private static readonly int AnimAttack = Animator.StringToHash("Attack");
        private static readonly int AnimDie = Animator.StringToHash("Die");

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
        }

        public void Initialize(float maxHealth, int formationIndex)
        {
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            FormationIndex = formationIndex;
            IsAlive = true;
        }

        private void Update()
        {
            if (!IsAlive) return;

            // Smoothly move toward the assigned formation position
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                _targetLocalPosition,
                _positionLerpSpeed * Time.deltaTime
            );
        }

        public void UpdateFormationPosition(Vector3 localOffset)
        {
            _targetLocalPosition = localOffset;
        }

        public void TakeDamage(float amount)
        {
            if (!IsAlive) return;

            CurrentHealth -= amount;
            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                Die();
            }
        }

        public void Die()
        {
            IsAlive = false;
            _animator?.SetTrigger(AnimDie);

            // Disable collider
            var col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            // Destroy after death animation (or immediately if no animator)
            Destroy(gameObject, _animator != null ? 3f : 0.5f);
        }

        public void SetMoving(bool isMoving)
        {
            _animator?.SetBool(AnimIsMoving, isMoving);
        }

        public void TriggerAttackAnimation()
        {
            _animator?.SetTrigger(AnimAttack);
        }
    }
}
