using UnityEngine;

namespace BFME2.Core
{
    public interface IBattalion : ISelectable, IDamageable
    {
        int AliveSoldiersCount { get; }
        int MaxSoldiers { get; }
        int Level { get; }
        float Experience { get; }
        void IssueCommand(ICommand command, bool queue = false);
        void GainExperience(float amount);
    }
}
