using UnityEngine;

namespace BFME2.Core
{
    public interface IBuilding : ISelectable, IDamageable
    {
        float ConstructionProgress { get; }
        bool IsConstructed { get; }
        BuildingState CurrentBuildingState { get; }
        void SetRallyPoint(Vector3 position);
    }
}
