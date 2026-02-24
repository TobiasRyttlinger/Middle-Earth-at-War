using BFME2.Core;
using UnityEngine;

namespace BFME2.FogOfWar
{
    public class FogOfWarManager : MonoBehaviour
    {
        [SerializeField] private int _textureResolution = 256;
        [SerializeField] private float _cellSize = 2f;
        [SerializeField] private float _updateInterval = 0.2f;

        private int _mapWidth;
        private int _mapHeight;
        private Texture2D _fogTexture;
        private byte[,] _visibilityGrid; // 0 = unexplored, 1 = explored (fog), 2 = visible
        private float _updateTimer;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        public void Initialize(int mapWidth, int mapHeight, float cellSize)
        {
            _mapWidth = mapWidth;
            _mapHeight = mapHeight;
            _cellSize = cellSize;

            _visibilityGrid = new byte[mapWidth, mapHeight];
            _fogTexture = new Texture2D(_textureResolution, _textureResolution, TextureFormat.RGBA32, false);
            _fogTexture.filterMode = FilterMode.Bilinear;

            // Initialize to unexplored (black)
            var pixels = new Color[_textureResolution * _textureResolution];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.black;
            }
            _fogTexture.SetPixels(pixels);
            _fogTexture.Apply();
        }

        private void Update()
        {
            _updateTimer += Time.deltaTime;
            if (_updateTimer >= _updateInterval)
            {
                _updateTimer = 0f;
                UpdateVisibility();
            }
        }

        public void UpdateVisibility()
        {
            if (_visibilityGrid == null) return;

            // Reset visible cells to explored (fog)
            for (int x = 0; x < _mapWidth; x++)
            {
                for (int y = 0; y < _mapHeight; y++)
                {
                    if (_visibilityGrid[x, y] == 2)
                        _visibilityGrid[x, y] = 1; // Previously visible -> fog
                }
            }

            // Reveal areas around player units and buildings
            // This would iterate over all units/buildings owned by the local player
            // and call RevealArea for each one's vision range
            // Placeholder — implementation depends on how units track their vision range

            UpdateTexture();
        }

        public void RevealArea(Vector3 worldCenter, float radius, int playerId)
        {
            if (_visibilityGrid == null) return;

            int cx = Mathf.RoundToInt(worldCenter.x / _cellSize) + _mapWidth / 2;
            int cz = Mathf.RoundToInt(worldCenter.z / _cellSize) + _mapHeight / 2;
            int cellRadius = Mathf.CeilToInt(radius / _cellSize);

            for (int x = cx - cellRadius; x <= cx + cellRadius; x++)
            {
                for (int z = cz - cellRadius; z <= cz + cellRadius; z++)
                {
                    if (x < 0 || x >= _mapWidth || z < 0 || z >= _mapHeight) continue;

                    float dist = Vector2.Distance(new Vector2(x, z), new Vector2(cx, cz));
                    if (dist <= cellRadius)
                    {
                        _visibilityGrid[x, z] = 2; // Fully visible
                    }
                }
            }
        }

        public bool IsVisible(Vector3 worldPosition, int playerId)
        {
            var cell = WorldToCell(worldPosition);
            if (cell.x < 0 || cell.x >= _mapWidth || cell.y < 0 || cell.y >= _mapHeight)
                return false;

            return _visibilityGrid[cell.x, cell.y] == 2;
        }

        public bool IsExplored(Vector3 worldPosition, int playerId)
        {
            var cell = WorldToCell(worldPosition);
            if (cell.x < 0 || cell.x >= _mapWidth || cell.y < 0 || cell.y >= _mapHeight)
                return false;

            return _visibilityGrid[cell.x, cell.y] >= 1;
        }

        public Texture2D GetFogTexture()
        {
            return _fogTexture;
        }

        private Vector2Int WorldToCell(Vector3 worldPos)
        {
            int x = Mathf.RoundToInt(worldPos.x / _cellSize) + _mapWidth / 2;
            int z = Mathf.RoundToInt(worldPos.z / _cellSize) + _mapHeight / 2;
            return new Vector2Int(x, z);
        }

        private void UpdateTexture()
        {
            if (_fogTexture == null || _visibilityGrid == null) return;

            float scaleX = (float)_mapWidth / _textureResolution;
            float scaleY = (float)_mapHeight / _textureResolution;

            for (int px = 0; px < _textureResolution; px++)
            {
                for (int py = 0; py < _textureResolution; py++)
                {
                    int gx = Mathf.Clamp(Mathf.RoundToInt(px * scaleX), 0, _mapWidth - 1);
                    int gy = Mathf.Clamp(Mathf.RoundToInt(py * scaleY), 0, _mapHeight - 1);

                    byte vis = _visibilityGrid[gx, gy];
                    Color color = vis switch
                    {
                        0 => new Color(0, 0, 0, 1f),       // Unexplored (black)
                        1 => new Color(0, 0, 0, 0.5f),     // Explored (semi-transparent)
                        2 => new Color(0, 0, 0, 0f),       // Visible (transparent)
                        _ => Color.black
                    };

                    _fogTexture.SetPixel(px, py, color);
                }
            }

            _fogTexture.Apply();
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<FogOfWarManager>();
            if (_fogTexture != null) Destroy(_fogTexture);
        }
    }
}
