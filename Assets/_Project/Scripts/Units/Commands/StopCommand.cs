using BFME2.Core;

namespace BFME2.Units.Commands
{
    public class StopCommand : ICommand
    {
        private readonly BattalionController _battalion;

        public bool IsComplete { get; private set; }

        public StopCommand(BattalionController battalion)
        {
            _battalion = battalion;
        }

        public void Execute()
        {
            _battalion.Movement?.Stop();
            _battalion.CombatHandler?.ClearTarget();
            _battalion.StateMachine.ChangeState(new UnitIdleState(_battalion));
            IsComplete = true;
        }

        public void Tick(float deltaTime) { }

        public void Cancel() { }
    }
}
