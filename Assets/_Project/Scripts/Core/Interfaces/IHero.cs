using UnityEngine;

namespace BFME2.Core
{
    public interface IHero : ISelectable, IDamageable
    {
        int Level { get; }
        float Experience { get; }
        bool CanRevive { get; }
        float ReviveTimer { get; }
        void IssueCommand(ICommand command, bool queue = false);
        void GainExperience(float amount);
        void UseAbility(int abilityIndex);
        void UseAbility(int abilityIndex, Vector3 targetPosition);
        void UseAbility(int abilityIndex, IDamageable target);
    }
}
