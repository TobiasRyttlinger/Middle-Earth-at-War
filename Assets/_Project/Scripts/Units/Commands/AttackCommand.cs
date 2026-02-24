using BFME2.Core;

namespace BFME2.Units.Commands
{
    public class AttackCommand : ICommand
    {
        private readonly BattalionController _battalion;
        private readonly IDamageable _target;

        public bool IsComplete { get; private set; }

        public AttackCommand(BattalionController battalion, IDamageable target)
        {
            _battalion = battalion;
            _target = target;
        }

        public void Execute()
        {
            _battalion.CombatHandler?.SetTarget(_target);
            _battalion.StateMachine.ChangeState(new UnitAttackingState(_battalion));
        }

        public void Tick(float deltaTime)
        {
            if (_target == null || !_target.IsAlive)
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
