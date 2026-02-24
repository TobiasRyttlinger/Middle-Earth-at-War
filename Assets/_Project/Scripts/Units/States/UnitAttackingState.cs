using BFME2.Core;
using UnityEngine;

namespace BFME2.Units
{
    public class UnitAttackingState : IState
    {
        private readonly BattalionController _battalion;
        private readonly BattalionMovement _movement;
        private readonly UnitCombatHandler _combat;

        public UnitAttackingState(BattalionController battalion)
        {
            _battalion = battalion;
            _movement = battalion.Movement;
            _combat = battalion.CombatHandler;
        }

        public void Enter()
        {
            // Stop moving momentarily when entering attack state
        }

        public void Tick(float deltaTime)
        {
            if (_combat == null) return;

            // Check if target is still valid
            if (!_combat.HasTarget)
            {
                // Try to find a new target
                var newTarget = _combat.FindNearestEnemy();
                if (newTarget != null)
                {
                    _combat.SetTarget(newTarget);
                }
                else
                {
                    _battalion.StateMachine.ChangeState(new UnitIdleState(_battalion));
                    return;
                }
            }

            var target = _combat.CurrentTarget;

            if (_combat.IsInRange(target))
            {
                // In range — attack
                _movement?.Stop();
                _movement?.FaceTarget(target.Transform.position);
                _combat.PerformAttack();
            }
            else
            {
                // Out of range — move closer
                _movement?.MoveTo(target.Transform.position);
            }
        }

        public void Exit()
        {
            _combat?.ClearTarget();
        }
    }
}
