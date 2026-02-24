using BFME2.Core;
using UnityEngine;

namespace BFME2.Buildings
{
    public class FreePlacementStrategy : MonoBehaviour, IBuildingPlacementStrategy
    {
        [SerializeField] private float _gridSize = 1f;
        [SerializeField] private float _overlapCheckRadius = 3f;
        [SerializeField] private float _maxTerrainSlope = 20f;
        [SerializeField] private Material _ghostValidMaterial;
        [SerializeField] private Material _ghostInvalidMaterial;

        private GameObject _currentGhost;

        public bool CanPlace(BuildingDefinition building, Vector3 position, Quaternion rotation)
        {
            // Check for overlapping buildings/units
            float checkRadius = Mathf.Max(building.FootprintSize.x, building.FootprintSize.y) * 0.5f;
            int overlapMask = (1 << GameConstants.LAYER_INDEX_BUILDING)
                            | (1 << GameConstants.LAYER_INDEX_UNIT)
                            | (1 << GameConstants.LAYER_INDEX_WALL);

            var overlaps = Physics.OverlapBox(
                position,
                new Vector3(building.FootprintSize.x * 0.5f, 1f, building.FootprintSize.y * 0.5f),
                rotation,
                overlapMask
            );

            if (overlaps.Length > 0) return false;

            // Check terrain slope
            if (Physics.Raycast(position + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 20f, GameConstants.TerrainLayerMask))
            {
                float angle = Vector3.Angle(hit.normal, Vector3.up);
                if (angle > _maxTerrainSlope) return false;
            }
            else
            {
                return false; // No terrain found
            }

            return true;
        }

        public Vector3 SnapPosition(Vector3 rawPosition)
        {
            if (_gridSize <= 0) return rawPosition;

            float x = Mathf.Round(rawPosition.x / _gridSize) * _gridSize;
            float z = Mathf.Round(rawPosition.z / _gridSize) * _gridSize;

            // Snap Y to terrain height
            float y = rawPosition.y;
            if (Physics.Raycast(new Vector3(x, 100f, z), Vector3.down, out RaycastHit hit, 200f, GameConstants.TerrainLayerMask))
            {
                y = hit.point.y;
            }

            return new Vector3(x, y, z);
        }

        public void ShowGhost(BuildingDefinition building, Vector3 position, bool valid)
        {
            if (building.PlacementGhostPrefab == null) return;

            if (_currentGhost == null)
            {
                _currentGhost = Instantiate(building.PlacementGhostPrefab);
                _currentGhost.name = "BuildingGhost";
            }

            _currentGhost.transform.position = position;

            var renderers = _currentGhost.GetComponentsInChildren<MeshRenderer>();
            var mat = valid ? _ghostValidMaterial : _ghostInvalidMaterial;
            if (mat != null)
            {
                foreach (var r in renderers)
                {
                    r.material = mat;
                }
            }
        }

        public void HideGhost()
        {
            if (_currentGhost != null)
            {
                Destroy(_currentGhost);
                _currentGhost = null;
            }
        }
    }
}
