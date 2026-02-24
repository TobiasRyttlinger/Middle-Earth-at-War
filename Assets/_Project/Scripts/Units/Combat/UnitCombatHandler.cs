using BFME2.Core;
using UnityEngine;

namespace BFME2.Units
{
    public class UnitCombatHandler : MonoBehaviour
    {
        private BattalionController _battalion;
        private float _attackTimer;

        public IDamageable CurrentTarget { get; private set; }
        public bool HasTarget => CurrentTarget != null && CurrentTarget.IsAlive;

        private void Awake()
        {
            _battalion = GetComponent<BattalionController>();
        }

        public void SetTarget(IDamageable target)
        {
            CurrentTarget = target;
            _attackTimer = 0f;
        }

        public void ClearTarget()
        {
            CurrentTarget = null;
        }

        public bool IsInRange(IDamageable target)
        {
            if (target == null || _battalion.Definition == null) return false;

            float distance = Vector3.Distance(transform.position, target.Transform.position);
            return distance <= _battalion.Definition.AttackRange;
        }

        public void PerformAttack()
        {
            if (CurrentTarget == null || !CurrentTarget.IsAlive || _battalion.Definition == null) return;

            _attackTimer += Time.deltaTime;
            float attackInterval = 1f / _battalion.Definition.AttackSpeed;

            if (_attackTimer >= attackInterval)
            {
                _attackTimer -= attackInterval;

                float damage = _battalion.Definition.AttackDamage * _battalion.AliveSoldiersCount;

                // Apply formation damage modifier
                if (_battalion.CurrentFormation != null)
                {
                    damage *= _battalion.CurrentFormation.DamageModifier;
                }

                // Apply level bonus
                if (_battalion.Definition.LevelBonusMultipliers != null
                    && _battalion.Level <= _battalion.Definition.LevelBonusMultipliers.Length)
                {
                    damage *= _battalion.Definition.LevelBonusMultipliers[_battalion.Level - 1];
                }

                // Deal damage through combat manager if available, otherwise directly
                if (ServiceLocator.TryGet<CombatManager>(out var combatManager))
                {
                    combatManager.ApplyDamage(CurrentTarget, damage, _battalion.Definition.DamageType, _battalion);
                }
                else
                {
                    CurrentTarget.TakeDamage(damage, _battalion.Definition.DamageType, _battalion);
                }

                // Trigger attack animations on soldiers
                foreach (var soldier in _battalion.Soldiers)
                {
                    if (soldier != null && soldier.IsAlive)
                    {
                        soldier.TriggerAttackAnimation();
                    }
                }
            }
        }

        /// <summary>
        /// Scans for the nearest enemy within auto-acquire range.
        /// </summary>
        public IDamageable FindNearestEnemy()
        {
            float range = GameConstants.UNIT_AUTO_ACQUIRE_RANGE;
            var colliders = Physics.OverlapSphere(transform.position, range, GameConstants.AttackableLayerMask);

            IDamageable nearest = null;
            float nearestDist = float.MaxValue;

            foreach (var col in colliders)
            {
                var damageable = col.GetComponentInParent<IDamageable>();
                if (damageable == null || !damageable.IsAlive) continue;
                if (damageable.OwnerPlayerId == _battalion.OwnerPlayerId) continue;

                float dist = Vector3.Distance(transform.position, damageable.Transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = damageable;
                }
            }

            return nearest;
        }
    }
}
