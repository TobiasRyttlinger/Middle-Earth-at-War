using BFME2.Core;
using UnityEngine;

namespace BFME2.Units.Commands
{
    public class PatrolCommand : ICommand
    {
        private readonly BattalionController _battalion;
        private readonly Vector3 _pointA;
        private readonly Vector3 _pointB;
        private bool _movingToB;
        private float _scanTimer;
        private const float SCAN_INTERVAL = 0.5f;

        public bool IsComplete { get; private set; }

        public PatrolCommand(BattalionController battalion, Vector3 pointA, Vector3 pointB)
        {
            _battalion = battalion;
            _pointA = pointA;
            _pointB = pointB;
            _movingToB = true;
        }

        public void Execute()
        {
            _battalion.Movement?.MoveTo(_pointB);
        }

        public void Tick(float deltaTime)
        {
            // Scan for enemies while patrolling
            _scanTimer += deltaTime;
            if (_scanTimer >= SCAN_INTERVAL)
            {
                _scanTimer = 0f;
                var enemy = _battalion.CombatHandler?.FindNearestEnemy();
                if (enemy != null)
                {
                    _battalion.CombatHandler.SetTarget(enemy);
                    _battalion.StateMachine.ChangeState(new UnitAttackingState(_battalion));
                    return;
                }
            }

            // Alternate between patrol points
            if (_battalion.Movement != null && _battalion.Movement.HasArrived)
            {
                _movingToB = !_movingToB;
                _battalion.Movement.MoveTo(_movingToB ? _pointB : _pointA);
            }
        }

        public void Cancel()
        {
            _battalion.Movement?.Stop();
            IsComplete = true;
        }
    }
}
