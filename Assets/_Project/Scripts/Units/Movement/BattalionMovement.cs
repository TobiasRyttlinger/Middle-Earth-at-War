using UnityEngine;
using UnityEngine.AI;

namespace BFME2.Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class BattalionMovement : MonoBehaviour
    {
        private NavMeshAgent _agent;
        private BattalionController _battalion;

        [SerializeField] private float _arrivalThreshold = 1f;

        public bool IsMoving => _agent.hasPath && _agent.remainingDistance > _arrivalThreshold;
        public bool HasArrived => !_agent.pathPending && _agent.remainingDistance <= _arrivalThreshold;
        public Vector3 CurrentDestination => _agent.destination;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _battalion = GetComponent<BattalionController>();
        }

        public void MoveTo(Vector3 destination)
        {
            if (_agent == null || !_agent.isOnNavMesh) return;

            _agent.isStopped = false;
            _agent.SetDestination(destination);

            // Update soldier animations
            SetSoldiersMoving(true);
        }

        public void Stop()
        {
            if (_agent == null || !_agent.isOnNavMesh) return;

            _agent.isStopped = true;
            _agent.ResetPath();

            SetSoldiersMoving(false);
        }

        public void SetSpeed(float speed)
        {
            if (_agent != null) _agent.speed = speed;
        }

        private void Update()
        {
            if (_battalion == null || !_battalion.IsAlive) return;

            // Update formation positions as the battalion moves
            if (IsMoving)
            {
                _battalion.UpdateSoldierPositions();
            }
            else if (_agent.hasPath && HasArrived)
            {
                SetSoldiersMoving(false);
            }
        }

        private void SetSoldiersMoving(bool moving)
        {
            if (_battalion == null) return;

            foreach (var soldier in _battalion.Soldiers)
            {
                if (soldier != null && soldier.IsAlive)
                {
                    soldier.SetMoving(moving);
                }
            }
        }

        /// <summary>
        /// Rotates the battalion to face a target position.
        /// </summary>
        public void FaceTarget(Vector3 targetPosition)
        {
            var direction = (targetPosition - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                var targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    Time.deltaTime * (_battalion.Definition?.RotationSpeed ?? 120f) * Mathf.Deg2Rad
                );
            }
        }
    }
}
