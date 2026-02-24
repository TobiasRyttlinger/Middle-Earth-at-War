using BFME2.Core;
using UnityEngine;

namespace BFME2.Units
{
    public class UnitDyingState : IState
    {
        private readonly BattalionController _battalion;
        private float _deathTimer;
        private const float DEATH_DURATION = 3f;

        public UnitDyingState(BattalionController battalion)
        {
            _battalion = battalion;
        }

        public void Enter()
        {
            _deathTimer = 0f;

            // Stop all movement
            _battalion.Movement?.Stop();

            // Disable collider so the unit can't be selected or targeted
            var collider = _battalion.GetComponent<Collider>();
            if (collider != null) collider.enabled = false;

            // Disable NavMeshAgent
            var agent = _battalion.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null) agent.enabled = false;
        }

        public void Tick(float deltaTime)
        {
            _deathTimer += deltaTime;

            if (_deathTimer >= DEATH_DURATION)
            {
                // Cleanup the battalion
                Object.Destroy(_battalion.gameObject);
            }
        }

        public void Exit() { }
    }
}
