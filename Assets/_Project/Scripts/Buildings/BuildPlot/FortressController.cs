using BFME2.Core;
using UnityEngine;
using System.Collections.Generic;

namespace BFME2.Buildings
{
    public class FortressController : MonoBehaviour
    {
        [SerializeField] private BuildPlot[] _buildPlots;
        [SerializeField] private BuildingController _fortressBuilding;

        public BuildPlot[] BuildPlots => _buildPlots;
        public BuildingController FortressBuilding => _fortressBuilding;
        public int OccupiedPlotCount
        {
            get
            {
                int count = 0;
                foreach (var plot in _buildPlots)
                {
                    if (plot.IsOccupied) count++;
                }
                return count;
            }
        }

        private void Awake()
        {
            gameObject.tag = GameConstants.TAG_FORTRESS;

            // Auto-find build plots if not assigned
            if (_buildPlots == null || _buildPlots.Length == 0)
            {
                _buildPlots = GetComponentsInChildren<BuildPlot>();
            }
        }

        public BuildPlot GetNearestEmptyPlot(Vector3 worldPosition)
        {
            BuildPlot nearest = null;
            float nearestDist = float.MaxValue;

            foreach (var plot in _buildPlots)
            {
                if (plot.IsOccupied) continue;

                float dist = Vector3.Distance(worldPosition, plot.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = plot;
                }
            }

            return nearest;
        }

        public BuildPlot[] GetEmptyPlots()
        {
            var empty = new List<BuildPlot>();
            foreach (var plot in _buildPlots)
            {
                if (!plot.IsOccupied)
                    empty.Add(plot);
            }
            return empty.ToArray();
        }

        public BuildPlot[] GetPlotsBySize(BuildPlotSize size)
        {
            var result = new List<BuildPlot>();
            foreach (var plot in _buildPlots)
            {
                if (!plot.IsOccupied && plot.PlotSize >= size)
                    result.Add(plot);
            }
            return result.ToArray();
        }

        public void HighlightAvailablePlots(BuildingDefinition building)
        {
            foreach (var plot in _buildPlots)
            {
                if (plot.CanAcceptBuilding(building))
                {
                    plot.Highlight(true);
                }
                else if (!plot.IsOccupied)
                {
                    plot.Highlight(false);
                }
            }
        }

        public void ClearAllHighlights()
        {
            foreach (var plot in _buildPlots)
            {
                plot.ClearHighlight();
            }
        }
    }
}
