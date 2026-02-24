using System.Collections.Generic;
using BFME2.Core;
using UnityEngine;

namespace BFME2.Buildings
{
    public class WallPlacementController : MonoBehaviour
    {
        [SerializeField] private GameObject _wallSegmentPrefab;
        [SerializeField] private GameObject _towerPrefab;
        [SerializeField] private float _segmentLength = 3f;
        [SerializeField] private int _segmentsBetweenTowers = 4;
        [SerializeField] private UnityEngine.Camera _mainCamera;

        private BuildingDefinition _wallDefinition;
        private Vector3 _startPoint;
        private bool _hasStartPoint;
        private readonly List<GameObject> _previewSegments = new();

        public bool IsPlacingWall => _wallDefinition != null;

        private void Start()
        {
            if (_mainCamera == null)
                _mainCamera = UnityEngine.Camera.main;
        }

        public void StartWallPlacement(BuildingDefinition wallDef)
        {
            _wallDefinition = wallDef;
            _hasStartPoint = false;
        }

        public void SetStartPoint(Vector3 position)
        {
            _startPoint = position;
            _hasStartPoint = true;
        }

        public void SetEndPoint(Vector3 position)
        {
            if (!_hasStartPoint) return;
            UpdatePreview(_startPoint, position);
        }

        public void ConfirmWallPlacement(Vector3 endPoint, int ownerPlayerId)
        {
            if (!_hasStartPoint) return;

            ClearPreview();
            BuildWall(_startPoint, endPoint, ownerPlayerId);
            CancelWallPlacement();
        }

        public void CancelWallPlacement()
        {
            ClearPreview();
            _wallDefinition = null;
            _hasStartPoint = false;
        }

        private void BuildWall(Vector3 start, Vector3 end, int ownerPlayerId)
        {
            var direction = (end - start).normalized;
            float totalDistance = Vector3.Distance(start, end);
            int segmentCount = Mathf.FloorToInt(totalDistance / _segmentLength);

            for (int i = 0; i <= segmentCount; i++)
            {
                Vector3 pos = start + direction * (i * _segmentLength);

                // Snap to terrain
                if (Physics.Raycast(pos + Vector3.up * 50f, Vector3.down, out RaycastHit hit, 100f, GameConstants.TerrainLayerMask))
                {
                    pos.y = hit.point.y;
                }

                bool isTower = (i % (_segmentsBetweenTowers + 1) == 0);
                var prefab = isTower ? _towerPrefab : _wallSegmentPrefab;

                if (prefab != null)
                {
                    var rotation = Quaternion.LookRotation(direction);
                    var segment = Instantiate(prefab, pos, rotation);

                    var wallSeg = segment.GetComponent<WallSegment>();
                    if (wallSeg == null)
                        wallSeg = segment.AddComponent<WallSegment>();

                    wallSeg.Initialize(ownerPlayerId, _wallDefinition?.MaxHealth ?? 500f);
                }
            }
        }

        private void UpdatePreview(Vector3 start, Vector3 end)
        {
            ClearPreview();

            var direction = (end - start).normalized;
            float totalDistance = Vector3.Distance(start, end);
            int segmentCount = Mathf.FloorToInt(totalDistance / _segmentLength);

            for (int i = 0; i <= segmentCount; i++)
            {
                Vector3 pos = start + direction * (i * _segmentLength);

                // Snap to terrain
                if (Physics.Raycast(pos + Vector3.up * 50f, Vector3.down, out RaycastHit hit, 100f, GameConstants.TerrainLayerMask))
                {
                    pos.y = hit.point.y;
                }

                var preview = GameObject.CreatePrimitive(PrimitiveType.Cube);
                preview.transform.position = pos;
                preview.transform.rotation = Quaternion.LookRotation(direction);
                preview.transform.localScale = new Vector3(1f, 2f, _segmentLength * 0.9f);

                // Make semi-transparent
                var renderer = preview.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    var mat = renderer.material;
                    mat.color = new Color(0f, 1f, 0f, 0.3f);
                }

                // Disable collider on preview
                var col = preview.GetComponent<Collider>();
                if (col != null) col.enabled = false;

                _previewSegments.Add(preview);
            }
        }

        private void ClearPreview()
        {
            foreach (var segment in _previewSegments)
            {
                if (segment != null) Destroy(segment);
            }
            _previewSegments.Clear();
        }
    }
}
