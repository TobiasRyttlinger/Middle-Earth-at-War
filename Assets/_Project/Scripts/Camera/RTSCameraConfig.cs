using UnityEngine;

namespace BFME2.Camera
{
    [CreateAssetMenu(menuName = "BFME2/Camera/RTS Camera Config")]
    public class RTSCameraConfig : ScriptableObject
    {
        [Header("Pan")]
        public float PanSpeed = 30f;
        public float PanSmoothTime = 0.1f;
        public float EdgeScrollThreshold = 10f;

        [Header("Zoom")]
        public float ZoomSpeed = 10f;
        public float ZoomSmoothTime = 0.15f;
        public float MinHeight = 10f;
        public float MaxHeight = 60f;

        [Header("Zoom Angle")]
        [Tooltip("Camera pitch at minimum zoom (close up)")]
        public float MinZoomAngle = 40f;
        [Tooltip("Camera pitch at maximum zoom (far away)")]
        public float MaxZoomAngle = 70f;

        [Header("Rotation")]
        public float RotationSpeed = 3f;
        public float RotationSmoothTime = 0.1f;

        [Header("Bounds")]
        public float MapMinX = -100f;
        public float MapMaxX = 100f;
        public float MapMinZ = -100f;
        public float MapMaxZ = 100f;

        [Header("Focus")]
        public float FocusSpeed = 5f;
    }
}
