using BFME2.Core;
using UnityEngine;

namespace BFME2.Units.Commands
{
    public class MoveCommand : ICommand
    {
        private readonly BattalionController _battalion;
        private readonly Vector3 _destination;

        public bool IsComplete { get; private set; }

        public MoveCommand(BattalionController battalion, Vector3 destination)
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
            if (_battalion.Movement != null && _battalion.Movement.HasArrived)
            {
                IsComplete = true;
            }
        }

        public void Cancel()
        {
            _battalion.Movement?.Stop();
            IsComplete = true;
        }
    }
}
