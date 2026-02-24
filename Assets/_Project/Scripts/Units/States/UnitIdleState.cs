using BFME2.Core;

namespace BFME2.Units
{
    public class UnitIdleState : IState
    {
        private readonly BattalionController _battalion;
        private readonly UnitCombatHandler _combat;
        private float _scanTimer;
        private const float SCAN_INTERVAL = 0.5f;

        public UnitIdleState(BattalionController battalion)
        {
            _battalion = battalion;
            _combat = battalion.GetComponent<UnitCombatHandler>();
        }

        public void Enter()
        {
            _battalion.Movement?.Stop();
            _scanTimer = 0f;
        }

        public void Tick(float deltaTime)
        {
            // Periodically scan for nearby enemies (auto-acquire)
            _scanTimer += deltaTime;
            if (_scanTimer >= SCAN_INTERVAL && _combat != null)
            {
                _scanTimer = 0f;

                var enemy = _combat.FindNearestEnemy();
                if (enemy != null)
                {
                    _combat.SetTarget(enemy);
                    _battalion.StateMachine.ChangeState(new UnitAttackingState(_battalion));
                }
            }
        }

        public void Exit() { }
    }
}
