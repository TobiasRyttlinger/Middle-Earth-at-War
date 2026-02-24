using BFME2.Core;
using UnityEngine;

namespace BFME2.Buildings
{
    public class BuildPlotPlacementStrategy : MonoBehaviour, IBuildingPlacementStrategy
    {
        [SerializeField] private float _plotDetectionRadius = 5f;
        [SerializeField] private Material _ghostValidMaterial;
        [SerializeField] private Material _ghostInvalidMaterial;

        private GameObject _currentGhost;
        private BuildPlot _hoveredPlot;

        public bool CanPlace(BuildingDefinition building, Vector3 position, Quaternion rotation)
        {
            var plot = FindNearestPlot(position);
            if (plot == null) return false;
            return plot.CanAcceptBuilding(building);
        }

        public Vector3 SnapPosition(Vector3 rawPosition)
        {
            var plot = FindNearestPlot(rawPosition);
            return plot != null ? plot.BuildPoint.position : rawPosition;
        }

        public void ShowGhost(BuildingDefinition building, Vector3 position, bool valid)
        {
            var plot = FindNearestPlot(position);

            // Highlight the hovered plot
            if (plot != _hoveredPlot)
            {
                _hoveredPlot?.ClearHighlight();
                _hoveredPlot = plot;
                _hoveredPlot?.Highlight(valid);
            }

            if (building.PlacementGhostPrefab == null) return;

            if (_currentGhost == null)
            {
                _currentGhost = Instantiate(building.PlacementGhostPrefab);
                _currentGhost.name = "BuildingGhost";
            }

            var snapPos = plot != null ? plot.BuildPoint.position : position;
            _currentGhost.transform.position = snapPos;

            // Apply valid/invalid material
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
            _hoveredPlot?.ClearHighlight();
            _hoveredPlot = null;
        }

        private BuildPlot FindNearestPlot(Vector3 position)
        {
            var colliders = Physics.OverlapSphere(position, _plotDetectionRadius, 1 << GameConstants.LAYER_INDEX_BUILD_PLOT);
            BuildPlot nearest = null;
            float nearestDist = float.MaxValue;

            foreach (var col in colliders)
            {
                var plot = col.GetComponent<BuildPlot>();
                if (plot == null || plot.IsOccupied) continue;

                float dist = Vector3.Distance(position, plot.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = plot;
                }
            }

            return nearest;
        }

        /// <summary>
        /// Returns the build plot at the current position, used when confirming placement.
        /// </summary>
        public BuildPlot GetPlotAtPosition(Vector3 position)
        {
            return FindNearestPlot(position);
        }
    }
}
