using BFME2.Core;
using UnityEngine;

namespace BFME2.Units.Commands
{
    public class AttackMoveCommand : ICommand
    {
        private readonly BattalionController _battalion;
        private readonly Vector3 _destination;
        private float _scanTimer;
        private const float SCAN_INTERVAL = 0.5f;

        public bool IsComplete { get; private set; }

        public AttackMoveCommand(BattalionController battalion, Vector3 destination)
        {
            _battalion = battalion;
            _destination = destination;
        }

        public void Execute()
        {
            _battalion.StateMachine.ChangeState(new UnitMovingState(_battalion, _destination));
        }

        public void Tick(float deltaTime)
        {
            // Periodically scan for enemies while moving
            _scanTimer += deltaTime;
            if (_scanTimer >= SCAN_INTERVAL)
            {
                _scanTimer = 0f;

                var enemy = _battalion.CombatHandler?.FindNearestEnemy();
                if (enemy != null)
                {
                    // Engage the enemy
                    _battalion.CombatHandler.SetTarget(enemy);
                    _battalion.StateMachine.ChangeState(new UnitAttackingState(_battalion));
                    return;
                }
            }

            // Check if we've arrived at destination
            if (_battalion.Movement != null && _battalion.Movement.HasArrived)
            {
                IsComplete = true;
            }
        }

        public void Cancel()
        {
            _battalion.CombatHandler?.ClearTarget();
            _battalion.Movement?.Stop();
            IsComplete = true;
        }
    }
}
