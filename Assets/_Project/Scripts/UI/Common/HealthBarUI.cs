using BFME2.Core;
using UnityEngine;
using UnityEngine.UI;

namespace BFME2.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private Image _fillImage;
        [SerializeField] private Vector3 _offset = new(0f, 3f, 0f);
        [SerializeField] private bool _onlyShowWhenDamaged = true;
        [SerializeField] private bool _onlyShowWhenSelected;

        [Header("Colors")]
        [SerializeField] private Color _fullHealthColor = Color.green;
        [SerializeField] private Color _halfHealthColor = Color.yellow;
        [SerializeField] private Color _lowHealthColor = Color.red;

        private IDamageable _target;
        private Canvas _canvas;
        private UnityEngine.Camera _mainCamera;
        private bool _isSelected;

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
        }

        private void Start()
        {
            _mainCamera = UnityEngine.Camera.main;
        }

        public void SetTarget(IDamageable target)
        {
            _target = target;
        }

        public void SetSelected(bool selected)
        {
            _isSelected = selected;
        }

        private void LateUpdate()
        {
            if (_target == null || !_target.IsAlive)
            {
                gameObject.SetActive(false);
                return;
            }

            // Visibility logic
            bool shouldShow = true;
            if (_onlyShowWhenDamaged && _target.CurrentHealth >= _target.MaxHealth)
                shouldShow = false;
            if (_onlyShowWhenSelected && !_isSelected)
                shouldShow = false;

            if (_healthSlider != null)
                _healthSlider.gameObject.SetActive(shouldShow);

            if (!shouldShow) return;

            // Update health value
            float healthPercent = _target.CurrentHealth / _target.MaxHealth;
            if (_healthSlider != null)
            {
                _healthSlider.value = healthPercent;
            }

            // Update color
            if (_fillImage != null)
            {
                if (healthPercent > 0.6f)
                    _fillImage.color = _fullHealthColor;
                else if (healthPercent > 0.3f)
                    _fillImage.color = _halfHealthColor;
                else
                    _fillImage.color = _lowHealthColor;
            }

            // Billboard toward camera
            if (_mainCamera != null)
            {
                transform.position = _target.Transform.position + _offset;
                transform.forward = _mainCamera.transform.forward;
            }
        }
    }
}
