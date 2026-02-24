using BFME2.Core;
using UnityEngine;

namespace BFME2.Buildings
{
    public class BuildPlot : MonoBehaviour
    {
        [SerializeField] private BuildPlotSize _plotSize = BuildPlotSize.Medium;
        [SerializeField] private Transform _buildPoint;
        [SerializeField] private MeshRenderer _plotVisualRenderer;

        [Header("Highlight Colors")]
        [SerializeField] private Color _defaultColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
        [SerializeField] private Color _validColor = new Color(0f, 1f, 0f, 0.4f);
        [SerializeField] private Color _invalidColor = new Color(1f, 0f, 0f, 0.4f);

        public BuildPlotSize PlotSize => _plotSize;
        public bool IsOccupied => OccupyingBuilding != null;
        public BuildingController OccupyingBuilding { get; private set; }
        public Transform BuildPoint => _buildPoint != null ? _buildPoint : transform;

        private void Awake()
        {
            gameObject.layer = GameConstants.LAYER_INDEX_BUILD_PLOT;
            gameObject.tag = GameConstants.TAG_BUILD_PLOT;

            if (_buildPoint == null)
                _buildPoint = transform;
        }

        public void Occupy(BuildingController building)
        {
            OccupyingBuilding = building;
            SetVisual(false);
        }

        public void Vacate()
        {
            OccupyingBuilding = null;
            SetVisual(true);
        }

        public void Highlight(bool valid)
        {
            if (_plotVisualRenderer == null) return;

            _plotVisualRenderer.enabled = true;
            _plotVisualRenderer.material.color = valid ? _validColor : _invalidColor;
        }

        public void ClearHighlight()
        {
            if (_plotVisualRenderer == null) return;

            _plotVisualRenderer.material.color = _defaultColor;
        }

        private void SetVisual(bool visible)
        {
            if (_plotVisualRenderer != null)
            {
                _plotVisualRenderer.enabled = visible;
            }
        }

        public bool CanAcceptBuilding(BuildingDefinition building)
        {
            if (IsOccupied) return false;
            if (building.PlacementType != BuildingPlacementType.BuildPlot) return false;
            return building.RequiredPlotSize <= _plotSize;
        }
    }
}
