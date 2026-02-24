using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BFME2.Core.Input
{
    public class InputManager : MonoBehaviour, IInputManager
    {
        public event Action<Vector2> OnCameraMoveInput;
        public event Action<float> OnCameraZoomInput;
        public event Action<float> OnCameraRotateInput;
        public event Action<Vector2> OnSelectInput;
        public event Action<Vector2> OnBoxSelectStart;
        public event Action<Vector2> OnBoxSelectEnd;
        public event Action<Vector2> OnActionCommand;
        public event Action OnDoubleClick;
        public event Action OnCancelAction;
        public event Action<Vector2> OnBuildPlaceInput;
        public event Action<float> OnBuildRotateInput;
        public event Action OnBuildCancelInput;

        public Vector2 MouseScreenPosition => Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
        public bool IsShiftHeld => Keyboard.current != null && Keyboard.current.shiftKey.isPressed;
        public bool IsCtrlHeld => Keyboard.current != null && Keyboard.current.ctrlKey.isPressed;

        [Header("Double Click Detection")]
        [SerializeField] private float _doubleClickThreshold = 0.3f;

        private float _lastClickTime;
        private bool _isBoxSelecting;
        private Vector2 _boxSelectOrigin;

        private void Awake()
        {
            ServiceLocator.Register<IInputManager>(this);
        }

        private void Update()
        {
            ProcessCameraInput();
            ProcessSelectionInput();
            ProcessActionInput();
            ProcessCancelInput();
        }

        private void ProcessCameraInput()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            // WASD / Arrow key camera movement
            var moveInput = Vector2.zero;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) moveInput.y += 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) moveInput.y -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) moveInput.x += 1f;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) moveInput.x -= 1f;

            if (moveInput != Vector2.zero)
            {
                OnCameraMoveInput?.Invoke(moveInput.normalized);
            }

            // Edge scrolling
            var mouse = Mouse.current;
            if (mouse != null)
            {
                var mousePos = mouse.position.ReadValue();
                var edgeScroll = Vector2.zero;
                float threshold = 10f;

                if (mousePos.x <= threshold) edgeScroll.x = -1f;
                else if (mousePos.x >= Screen.width - threshold) edgeScroll.x = 1f;
                if (mousePos.y <= threshold) edgeScroll.y = -1f;
                else if (mousePos.y >= Screen.height - threshold) edgeScroll.y = 1f;

                if (edgeScroll != Vector2.zero)
                {
                    OnCameraMoveInput?.Invoke(edgeScroll.normalized);
                }

                // Scroll wheel zoom
                var scroll = mouse.scroll.ReadValue().y;
                if (Mathf.Abs(scroll) > 0.01f)
                {
                    OnCameraZoomInput?.Invoke(scroll > 0 ? 1f : -1f);
                }

                // Middle mouse rotation
                if (mouse.middleButton.isPressed)
                {
                    var delta = mouse.delta.ReadValue().x;
                    if (Mathf.Abs(delta) > 0.01f)
                    {
                        OnCameraRotateInput?.Invoke(delta);
                    }
                }
            }
        }

        private void ProcessSelectionInput()
        {
            var mouse = Mouse.current;
            if (mouse == null) return;

            // Left click down — start potential box select
            if (mouse.leftButton.wasPressedThisFrame)
            {
                _boxSelectOrigin = mouse.position.ReadValue();
                _isBoxSelecting = false;
            }

            // Left click held — check if dragging (box select)
            if (mouse.leftButton.isPressed)
            {
                var currentPos = mouse.position.ReadValue();
                float dragDist = Vector2.Distance(_boxSelectOrigin, currentPos);
                if (dragDist > 5f && !_isBoxSelecting)
                {
                    _isBoxSelecting = true;
                    OnBoxSelectStart?.Invoke(_boxSelectOrigin);
                }
            }

            // Left click released
            if (mouse.leftButton.wasReleasedThisFrame)
            {
                var releasePos = mouse.position.ReadValue();

                if (_isBoxSelecting)
                {
                    OnBoxSelectEnd?.Invoke(releasePos);
                    _isBoxSelecting = false;
                }
                else
                {
                    // Check for double-click
                    float timeSinceLastClick = Time.unscaledTime - _lastClickTime;
                    if (timeSinceLastClick <= _doubleClickThreshold)
                    {
                        OnDoubleClick?.Invoke();
                    }
                    else
                    {
                        OnSelectInput?.Invoke(releasePos);
                    }
                    _lastClickTime = Time.unscaledTime;
                }
            }
        }

        private void ProcessActionInput()
        {
            var mouse = Mouse.current;
            if (mouse == null) return;

            // Right-click = action command (move/attack)
            if (mouse.rightButton.wasPressedThisFrame)
            {
                OnActionCommand?.Invoke(mouse.position.ReadValue());
            }
        }

        private void ProcessCancelInput()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (keyboard.escapeKey.wasPressedThisFrame)
            {
                OnCancelAction?.Invoke();
                OnBuildCancelInput?.Invoke();
            }
        }

        public void SwitchToGameplayMap()
        {
            // Placeholder for input action map switching
        }

        public void SwitchToBuildMap()
        {
            // In build mode, left click = place, right click / escape = cancel
        }

        public void SwitchToUIMap()
        {
            // UI-only input
        }

        /// <summary>
        /// Call from build mode to handle placement input.
        /// </summary>
        public void ProcessBuildModeInput()
        {
            var mouse = Mouse.current;
            if (mouse == null) return;

            if (mouse.leftButton.wasPressedThisFrame)
            {
                OnBuildPlaceInput?.Invoke(mouse.position.ReadValue());
            }

            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.rKey.wasPressedThisFrame)
                {
                    OnBuildRotateInput?.Invoke(90f);
                }
            }
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<IInputManager>();
        }
    }
}
