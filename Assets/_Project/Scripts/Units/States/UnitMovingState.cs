using BFME2.Core;
using UnityEngine;

namespace BFME2.Units
{
    public class UnitMovingState : IState
    {
        private readonly BattalionController _battalion;
        private readonly BattalionMovement _movement;
        private readonly Vector3 _destination;

        public UnitMovingState(BattalionController battalion, Vector3 destination)
        {
            _battalion = battalion;
            _movement = battalion.Movement;
            _destination = destination;
        }

        public void Enter()
        {
            _movement?.MoveTo(_destination);
        }

        public void Tick(float deltaTime)
        {
            if (_movement == null) return;

            if (_movement.HasArrived)
            {
                _battalion.StateMachine.ChangeState(new UnitIdleState(_battalion));
            }
        }

        public void Exit()
        {
            // Don't stop here — let the next state handle movement
        }
    }
}
