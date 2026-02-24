using BFME2.Core;
using UnityEngine;

namespace BFME2.Camera
{
    public class RTSCameraController : MonoBehaviour
    {
        [SerializeField] private RTSCameraConfig _config;

        private IInputManager _input;
        private Vector3 _targetPosition;
        private float _targetHeight;
        private float _targetRotationY;
        private Vector3 _velocity;
        private float _heightVelocity;
        private float _rotationVelocity;
        private bool _isEnabled = true;

        public RTSCameraConfig Config => _config;

        private void Start()
        {
            _input = ServiceLocator.Get<IInputManager>();

            _targetPosition = transform.position;
            _targetHeight = transform.position.y;
            _targetRotationY = transform.eulerAngles.y;

            if (_input != null)
            {
                _input.OnCameraMoveInput += HandleCameraMove;
                _input.OnCameraZoomInput += HandleCameraZoom;
                _input.OnCameraRotateInput += HandleCameraRotate;
            }
        }

        private void LateUpdate()
        {
            if (!_isEnabled || _config == null) return;

            // Smooth position
            var currentPos = transform.position;
            var targetXZ = new Vector3(_targetPosition.x, 0f, _targetPosition.z);
            var currentXZ = new Vector3(currentPos.x, 0f, currentPos.z);
            var smoothedXZ = Vector3.SmoothDamp(currentXZ, targetXZ, ref _velocity, _config.PanSmoothTime);

            // Smooth height
            float smoothedHeight = Mathf.SmoothDamp(currentPos.y, _targetHeight, ref _heightVelocity, _config.ZoomSmoothTime);

            transform.position = new Vector3(smoothedXZ.x, smoothedHeight, smoothedXZ.z);

            // Smooth rotation
            float currentRotY = transform.eulerAngles.y;
            float smoothedRotY = Mathf.SmoothDampAngle(currentRotY, _targetRotationY, ref _rotationVelocity, _config.RotationSmoothTime);

            // Calculate pitch based on zoom level
            float zoomT = Mathf.InverseLerp(_config.MinHeight, _config.MaxHeight, smoothedHeight);
            float pitch = Mathf.Lerp(_config.MinZoomAngle, _config.MaxZoomAngle, zoomT);

            transform.rotation = Quaternion.Euler(pitch, smoothedRotY, 0f);
        }

        private void HandleCameraMove(Vector2 input)
        {
            if (!_isEnabled) return;

            // Move relative to camera's forward direction on XZ plane
            var forward = transform.forward.Flat().normalized;
            var right = transform.right.Flat().normalized;
            var moveDir = (forward * input.y + right * input.x).normalized;

            _targetPosition += moveDir * (_config.PanSpeed * Time.unscaledDeltaTime);

            // Clamp to map bounds
            _targetPosition.x = Mathf.Clamp(_targetPosition.x, _config.MapMinX, _config.MapMaxX);
            _targetPosition.z = Mathf.Clamp(_targetPosition.z, _config.MapMinZ, _config.MapMaxZ);
        }

        private void HandleCameraZoom(float direction)
        {
            if (!_isEnabled) return;

            _targetHeight -= direction * _config.ZoomSpeed;
            _targetHeight = Mathf.Clamp(_targetHeight, _config.MinHeight, _config.MaxHeight);
        }

        private void HandleCameraRotate(float delta)
        {
            if (!_isEnabled) return;

            _targetRotationY += delta * _config.RotationSpeed;
        }

        public void FocusOnPosition(Vector3 worldPos)
        {
            _targetPosition = new Vector3(worldPos.x, _targetPosition.y, worldPos.z);
        }

        public void SetBounds(float minX, float maxX, float minZ, float maxZ)
        {
            if (_config != null)
            {
                _config.MapMinX = minX;
                _config.MapMaxX = maxX;
                _config.MapMinZ = minZ;
                _config.MapMaxZ = maxZ;
            }
        }

        public void SetEnabled(bool enabled)
        {
            _isEnabled = enabled;
        }

        private void OnDestroy()
        {
            if (_input != null)
            {
                _input.OnCameraMoveInput -= HandleCameraMove;
                _input.OnCameraZoomInput -= HandleCameraZoom;
                _input.OnCameraRotateInput -= HandleCameraRotate;
            }
        }
    }
}
