using UnityEngine;

namespace BFME2.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class MinimapCameraController : MonoBehaviour
    {
        [SerializeField] private RTSCameraController _mainCameraController;
        [SerializeField] private float _orthoSize = 100f;

        private UnityEngine.Camera _camera;

        private void Awake()
        {
            _camera = GetComponent<UnityEngine.Camera>();
            _camera.orthographic = true;
            _camera.orthographicSize = _orthoSize;

            // Look straight down
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }

        private void LateUpdate()
        {
            if (_mainCameraController == null) return;

            // Follow the main camera's XZ position but stay high above
            var mainPos = _mainCameraController.transform.position;
            transform.position = new Vector3(mainPos.x, 200f, mainPos.z);
        }

        /// <summary>
        /// Converts a normalized minimap click (0-1 range) to a world position
        /// and moves the main camera there.
        /// </summary>
        public void OnMinimapClick(Vector2 normalizedPosition)
        {
            if (_mainCameraController == null || _mainCameraController.Config == null) return;

            var config = _mainCameraController.Config;
            float worldX = Mathf.Lerp(config.MapMinX, config.MapMaxX, normalizedPosition.x);
            float worldZ = Mathf.Lerp(config.MapMinZ, config.MapMaxZ, normalizedPosition.y);

            _mainCameraController.FocusOnPosition(new Vector3(worldX, 0f, worldZ));
        }

        /// <summary>
        /// Returns the visible area of the main camera projected onto the ground plane.
        /// Useful for drawing the camera frustum rect on the minimap.
        /// </summary>
        public Rect GetMainCameraVisibleArea()
        {
            if (_mainCameraController == null) return Rect.zero;

            var mainCam = _mainCameraController.GetComponent<UnityEngine.Camera>();
            if (mainCam == null) return Rect.zero;

            var groundPlane = new Plane(Vector3.up, Vector3.zero);

            // Cast rays from screen corners to ground
            Vector3 bl = ScreenToGround(mainCam, new Vector3(0, 0, 0), groundPlane);
            Vector3 br = ScreenToGround(mainCam, new Vector3(Screen.width, 0, 0), groundPlane);
            Vector3 tl = ScreenToGround(mainCam, new Vector3(0, Screen.height, 0), groundPlane);
            Vector3 tr = ScreenToGround(mainCam, new Vector3(Screen.width, Screen.height, 0), groundPlane);

            float minX = Mathf.Min(bl.x, br.x, tl.x, tr.x);
            float maxX = Mathf.Max(bl.x, br.x, tl.x, tr.x);
            float minZ = Mathf.Min(bl.z, br.z, tl.z, tr.z);
            float maxZ = Mathf.Max(bl.z, br.z, tl.z, tr.z);

            return new Rect(minX, minZ, maxX - minX, maxZ - minZ);
        }

        private Vector3 ScreenToGround(UnityEngine.Camera cam, Vector3 screenPoint, Plane ground)
        {
            var ray = cam.ScreenPointToRay(screenPoint);
            if (ground.Raycast(ray, out float enter))
            {
                return ray.GetPoint(enter);
            }
            return Vector3.zero;
        }

        public void SetOrthoSize(float size)
        {
            _orthoSize = size;
            if (_camera != null) _camera.orthographicSize = size;
        }
    }
}
