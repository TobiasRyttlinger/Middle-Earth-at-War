using BFME2.Core;
using UnityEngine;

namespace BFME2.Units
{
    public class ProjectileController : MonoBehaviour
    {
        [SerializeField] private float _arcHeight = 3f;
        [SerializeField] private GameObject _impactEffect;

        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private IDamageable _target;
        private float _speed;
        private float _damage;
        private DamageTypeDefinition _damageType;
        private float _areaRadius;
        private int _sourcePlayerId;
        private float _progress;
        private bool _isLaunched;

        public void Launch(Vector3 origin, IDamageable target, float damage, DamageTypeDefinition damageType, float speed, int sourcePlayerId)
        {
            _startPosition = origin;
            _target = target;
            _targetPosition = target.Transform.position;
            _damage = damage;
            _damageType = damageType;
            _speed = speed;
            _areaRadius = 0f;
            _sourcePlayerId = sourcePlayerId;
            _progress = 0f;
            _isLaunched = true;

            transform.position = origin;
        }

        public void LaunchAtPosition(Vector3 origin, Vector3 targetPosition, float damage, DamageTypeDefinition damageType, float speed, float areaRadius, int sourcePlayerId)
        {
            _startPosition = origin;
            _target = null;
            _targetPosition = targetPosition;
            _damage = damage;
            _damageType = damageType;
            _speed = speed;
            _areaRadius = areaRadius;
            _sourcePlayerId = sourcePlayerId;
            _progress = 0f;
            _isLaunched = true;

            transform.position = origin;
        }

        private void Update()
        {
            if (!_isLaunched) return;

            // Update target position if tracking a moving target
            if (_target != null && _target.IsAlive)
            {
                _targetPosition = _target.Transform.position;
            }

            float totalDistance = Vector3.Distance(_startPosition, _targetPosition);
            if (totalDistance < 0.01f)
            {
                OnImpact();
                return;
            }

            _progress += (_speed / totalDistance) * Time.deltaTime;

            if (_progress >= 1f)
            {
                OnImpact();
                return;
            }

            // Calculate position along arc
            Vector3 flatPosition = Vector3.Lerp(_startPosition, _targetPosition, _progress);
            float height = _arcHeight * 4f * _progress * (1f - _progress); // Parabolic arc
            transform.position = new Vector3(flatPosition.x, flatPosition.y + height, flatPosition.z);

            // Face movement direction
            var nextPos = Vector3.Lerp(_startPosition, _targetPosition, _progress + 0.01f);
            float nextHeight = _arcHeight * 4f * (_progress + 0.01f) * (1f - _progress - 0.01f);
            var lookTarget = new Vector3(nextPos.x, nextPos.y + nextHeight, nextPos.z);
            if ((lookTarget - transform.position).sqrMagnitude > 0.001f)
            {
                transform.forward = (lookTarget - transform.position).normalized;
            }
        }

        private void OnImpact()
        {
            _isLaunched = false;

            if (_areaRadius > 0f)
            {
                // Area damage (siege projectile)
                if (ServiceLocator.TryGet<CombatManager>(out var combatManager))
                {
                    combatManager.ApplyAreaDamage(_targetPosition, _areaRadius, _damage, _damageType, null, _sourcePlayerId);
                }
            }
            else if (_target != null && _target.IsAlive)
            {
                // Direct hit
                if (ServiceLocator.TryGet<CombatManager>(out var combatManager))
                {
                    combatManager.ApplyDamage(_target, _damage, _damageType, null);
                }
                else
                {
                    _target.TakeDamage(_damage, _damageType, null);
                }
            }

            // Spawn impact effect
            if (_impactEffect != null)
            {
                var fx = Instantiate(_impactEffect, _targetPosition, Quaternion.identity);
                Destroy(fx, 3f);
            }

            // Return to pool or destroy
            Destroy(gameObject);
        }
    }
}
