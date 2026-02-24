using System.Collections.Generic;
using BFME2.Core;
using UnityEngine;

namespace BFME2.Selection
{
    public class SelectionManager : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Camera _mainCamera;
        [SerializeField] private SelectionVisualizer _visualizer;

        private IInputManager _input;
        private readonly List<ISelectable> _currentSelection = new();
        private Vector2 _boxSelectOrigin;
        private bool _isBoxSelecting;

        public IReadOnlyList<ISelectable> CurrentSelection => _currentSelection;
        public ISelectable PrimarySelection => _currentSelection.Count > 0 ? _currentSelection[0] : null;
        public int LocalPlayerId { get; set; } = 0;

        private void Start()
        {
            _input = ServiceLocator.Get<IInputManager>();

            if (_mainCamera == null)
                _mainCamera = UnityEngine.Camera.main;

            if (_input != null)
            {
                _input.OnSelectInput += HandleSelect;
                _input.OnBoxSelectStart += HandleBoxSelectStart;
                _input.OnBoxSelectEnd += HandleBoxSelectEnd;
                _input.OnDoubleClick += HandleDoubleClick;
                _input.OnActionCommand += HandleActionCommand;
            }

            ServiceLocator.Register(this);
        }

        private void HandleSelect(Vector2 screenPos)
        {
            var ray = _mainCamera.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out RaycastHit hit, GameConstants.GROUND_RAYCAST_DISTANCE, GameConstants.SelectableLayerMask))
            {
                var selectable = hit.collider.GetComponentInParent<ISelectable>();
                if (selectable != null && selectable.IsSelectable)
                {
                    if (_input.IsShiftHeld)
                    {
                        ToggleSelection(selectable);
                    }
                    else if (_input.IsCtrlHeld)
                    {
                        AddToSelection(selectable);
                    }
                    else
                    {
                        SelectSingle(selectable);
                    }
                    return;
                }
            }

            // Clicked on nothing — clear selection
            if (!_input.IsShiftHeld && !_input.IsCtrlHeld)
            {
                ClearSelection();
            }
        }

        private void HandleBoxSelectStart(Vector2 origin)
        {
            _boxSelectOrigin = origin;
            _isBoxSelecting = true;
        }

        private void HandleBoxSelectEnd(Vector2 endPos)
        {
            if (!_isBoxSelecting) return;
            _isBoxSelecting = false;

            var rect = GetScreenRect(_boxSelectOrigin, endPos);
            BoxSelect(rect);

            _visualizer?.HideBoxSelection();
        }

        private void HandleDoubleClick()
        {
            // Select all visible units of the same type as the current primary selection
            if (PrimarySelection == null || PrimarySelection.Type != SelectableType.Unit) return;

            var battalion = PrimarySelection as IBattalion;
            if (battalion == null) return;

            // Find all units of the same type visible on screen
            var allUnits = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            foreach (var mb in allUnits)
            {
                if (mb is ISelectable selectable
                    && selectable.Type == SelectableType.Unit
                    && selectable.OwnerPlayerId == LocalPlayerId
                    && selectable.IsSelectable
                    && IsOnScreen(selectable.Transform.position))
                {
                    AddToSelection(selectable);
                }
            }
        }

        private void HandleActionCommand(Vector2 screenPos)
        {
            if (_currentSelection.Count == 0) return;

            var ray = _mainCamera.ScreenPointToRay(screenPos);

            // Check if clicking on an enemy (attack command)
            if (Physics.Raycast(ray, out RaycastHit hit, GameConstants.GROUND_RAYCAST_DISTANCE, GameConstants.AttackableLayerMask))
            {
                var damageable = hit.collider.GetComponentInParent<IDamageable>();
                if (damageable != null && damageable.OwnerPlayerId != LocalPlayerId)
                {
                    // Issue attack command to all selected units
                    foreach (var selectable in _currentSelection)
                    {
                        if (selectable is IBattalion battalion)
                        {
                            // The actual AttackCommand creation happens in the unit system
                            // This is handled by whoever listens to selection + action commands
                        }
                    }
                    return;
                }
            }

            // Otherwise, move command to ground position
            if (Physics.Raycast(ray, out RaycastHit groundHit, GameConstants.GROUND_RAYCAST_DISTANCE, GameConstants.TerrainLayerMask))
            {
                // Move command will be issued by the command system
            }
        }

        private void Update()
        {
            if (_isBoxSelecting && _visualizer != null)
            {
                _visualizer.DrawBoxSelection(_boxSelectOrigin, _input.MouseScreenPosition);
            }
        }

        public void SelectSingle(ISelectable target)
        {
            ClearSelection();
            _currentSelection.Add(target);
            target.OnSelected();
            NotifySelectionChanged();
        }

        public void AddToSelection(ISelectable target)
        {
            if (target.OwnerPlayerId != LocalPlayerId) return;
            if (_currentSelection.Contains(target)) return;

            // Only select same owner entities, prioritize units
            if (_currentSelection.Count > 0 && _currentSelection[0].OwnerPlayerId != target.OwnerPlayerId) return;

            _currentSelection.Add(target);
            target.OnSelected();
            NotifySelectionChanged();
        }

        public void RemoveFromSelection(ISelectable target)
        {
            if (_currentSelection.Remove(target))
            {
                target.OnDeselected();
                NotifySelectionChanged();
            }
        }

        public void ToggleSelection(ISelectable target)
        {
            if (_currentSelection.Contains(target))
                RemoveFromSelection(target);
            else
                AddToSelection(target);
        }

        public void ClearSelection()
        {
            foreach (var selectable in _currentSelection)
            {
                selectable.OnDeselected();
            }
            _currentSelection.Clear();
            NotifySelectionChanged();
        }

        public void BoxSelect(Rect screenRect)
        {
            if (!_input.IsShiftHeld && !_input.IsCtrlHeld)
            {
                ClearSelection();
            }

            // Find all selectable objects whose screen positions fall within the rect
            var allSelectables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            foreach (var mb in allSelectables)
            {
                if (mb is ISelectable selectable
                    && selectable.IsSelectable
                    && selectable.OwnerPlayerId == LocalPlayerId
                    && selectable.Type != SelectableType.Building) // Don't box-select buildings
                {
                    var screenPoint = _mainCamera.WorldToScreenPoint(selectable.Transform.position);
                    if (screenPoint.z > 0 && screenRect.Contains(new Vector2(screenPoint.x, screenPoint.y)))
                    {
                        if (!_currentSelection.Contains(selectable))
                        {
                            _currentSelection.Add(selectable);
                            selectable.OnSelected();
                        }
                    }
                }
            }

            NotifySelectionChanged();
        }

        private void NotifySelectionChanged()
        {
            GameEvents.RaiseSelectionChanged(_currentSelection);
        }

        private bool IsOnScreen(Vector3 worldPos)
        {
            var screenPoint = _mainCamera.WorldToScreenPoint(worldPos);
            return screenPoint.z > 0
                && screenPoint.x >= 0 && screenPoint.x <= Screen.width
                && screenPoint.y >= 0 && screenPoint.y <= Screen.height;
        }

        private Rect GetScreenRect(Vector2 start, Vector2 end)
        {
            float x = Mathf.Min(start.x, end.x);
            float y = Mathf.Min(start.y, end.y);
            float w = Mathf.Abs(start.x - end.x);
            float h = Mathf.Abs(start.y - end.y);
            return new Rect(x, y, w, h);
        }

        private void OnDestroy()
        {
            if (_input != null)
            {
                _input.OnSelectInput -= HandleSelect;
                _input.OnBoxSelectStart -= HandleBoxSelectStart;
                _input.OnBoxSelectEnd -= HandleBoxSelectEnd;
                _input.OnDoubleClick -= HandleDoubleClick;
                _input.OnActionCommand -= HandleActionCommand;
            }
            ServiceLocator.Unregister<SelectionManager>();
        }
    }
}
