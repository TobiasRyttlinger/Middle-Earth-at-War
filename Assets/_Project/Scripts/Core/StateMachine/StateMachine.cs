using UnityEngine;

namespace BFME2.Core
{
    public class StateMachine
    {
        public IState CurrentState { get; private set; }
        public IState PreviousState { get; private set; }

        public void ChangeState(IState newState)
        {
            if (newState == null)
            {
                Debug.LogWarning("[StateMachine] Attempted to change to null state.");
                return;
            }

            PreviousState = CurrentState;
            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }

        public void Tick(float deltaTime)
        {
            CurrentState?.Tick(deltaTime);
        }

        public void RevertToPreviousState()
        {
            if (PreviousState != null)
            {
                ChangeState(PreviousState);
            }
        }
    }
}
