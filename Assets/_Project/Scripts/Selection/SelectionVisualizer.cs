using UnityEngine;

namespace BFME2.Selection
{
    public class SelectionVisualizer : MonoBehaviour
    {
        [Header("Box Selection")]
        [SerializeField] private Color _boxColor = new Color(0.3f, 0.8f, 0.3f, 0.2f);
        [SerializeField] private Color _boxBorderColor = new Color(0.3f, 0.8f, 0.3f, 0.8f);

        [Header("Selection Circle")]
        [SerializeField] private GameObject _selectionCirclePrefab;

        private bool _isDrawingBox;
        private Vector2 _boxStart;
        private Vector2 _boxEnd;

        public void DrawBoxSelection(Vector2 start, Vector2 end)
        {
            _isDrawingBox = true;
            _boxStart = start;
            _boxEnd = end;
        }

        public void HideBoxSelection()
        {
            _isDrawingBox = false;
        }

        private void OnGUI()
        {
            if (!_isDrawingBox) return;

            var rect = GetScreenRect(_boxStart, _boxEnd);
            // Convert to GUI space (Y is flipped)
            rect.y = Screen.height - rect.y - rect.height;

            DrawScreenRect(rect, _boxColor);
            DrawScreenRectBorder(rect, 2, _boxBorderColor);
        }

        private void DrawScreenRect(Rect rect, Color color)
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            GUI.DrawTexture(rect, tex);
            Object.Destroy(tex);
        }

        private void DrawScreenRectBorder(Rect rect, float thickness, Color color)
        {
            // Top
            DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
            // Bottom
            DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
            // Left
            DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
            // Right
            DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        }

        private Rect GetScreenRect(Vector2 start, Vector2 end)
        {
            float x = Mathf.Min(start.x, end.x);
            float y = Mathf.Min(start.y, end.y);
            float w = Mathf.Abs(start.x - end.x);
            float h = Mathf.Abs(start.y - end.y);
            return new Rect(x, y, w, h);
        }

        public void ShowSelectionIndicator(Transform target)
        {
            if (_selectionCirclePrefab == null) return;

            // Instantiate a selection circle as a child of the target
            var circle = Instantiate(_selectionCirclePrefab, target);
            circle.transform.localPosition = new Vector3(0f, 0.05f, 0f);
            circle.name = "SelectionCircle";
        }

        public void HideSelectionIndicator(Transform target)
        {
            var circle = target.Find("SelectionCircle");
            if (circle != null)
            {
                Destroy(circle.gameObject);
            }
        }
    }
}
