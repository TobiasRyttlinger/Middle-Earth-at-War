using UnityEngine;

namespace BFME2.Buildings
{
    public interface IBuildingPlacementStrategy
    {
        bool CanPlace(BuildingDefinition building, Vector3 position, Quaternion rotation);
        Vector3 SnapPosition(Vector3 rawPosition);
        void ShowGhost(BuildingDefinition building, Vector3 position, bool valid);
        void HideGhost();
    }
}
