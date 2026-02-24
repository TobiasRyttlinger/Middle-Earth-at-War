using System.Collections.Generic;
using BFME2.Camera;
using BFME2.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BFME2.UI
{
    public class MinimapController : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private RawImage _minimapImage;
        [SerializeField] private RectTransform _minimapRect;
        [SerializeField] private MinimapCameraController _minimapCamera;
        [SerializeField] private RectTransform _cameraFrustumIndicator;

        private readonly Dictionary<Transform, RectTransform> _iconInstances = new();

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_minimapRect == null || _minimapCamera == null) return;

            // Convert click position to normalized coordinates on the minimap
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _minimapRect,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint
            );

            var rect = _minimapRect.rect;
            float normalizedX = (localPoint.x - rect.x) / rect.width;
            float normalizedY = (localPoint.y - rect.y) / rect.height;

            _minimapCamera.OnMinimapClick(new Vector2(normalizedX, normalizedY));
        }

        private void LateUpdate()
        {
            UpdateCameraFrustum();
        }

        private void UpdateCameraFrustum()
        {
            if (_cameraFrustumIndicator == null || _minimapCamera == null) return;

            var visibleArea = _minimapCamera.GetMainCameraVisibleArea();
            if (visibleArea == Rect.zero) return;

            // Convert world rect to minimap rect — this depends on your minimap setup
            // Placeholder: would need to map world coords to minimap UI coords
        }

        public void RegisterEntity(Transform entity, Color color, MinimapIconType type)
        {
            // In production, instantiate a small colored dot/icon on the minimap
            // that follows the entity's world position mapped to minimap coords
        }

        public void UnregisterEntity(Transform entity)
        {
            if (_iconInstances.TryGetValue(entity, out var icon))
            {
                if (icon != null) Destroy(icon.gameObject);
                _iconInstances.Remove(entity);
            }
        }

        public void ShowAlert(Vector3 worldPosition)
        {
            // Flash a ping indicator on the minimap at the given world position
            // Would create a temporary animated UI element
        }
    }
}
