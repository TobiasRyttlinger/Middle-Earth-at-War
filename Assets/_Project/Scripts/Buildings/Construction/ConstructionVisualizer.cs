using UnityEngine;

namespace BFME2.Buildings
{
    public class ConstructionVisualizer : MonoBehaviour
    {
        [SerializeField] private BuildingController _building;
        [SerializeField] private GameObject[] _constructionStages;
        [SerializeField] private GameObject _completedModel;
        [SerializeField] private GameObject _destroyedModel;

        [Header("Scale Animation")]
        [SerializeField] private bool _useScaleAnimation = true;
        [SerializeField] private float _minScale = 0.1f;

        private int _currentStage = -1;

        private void Start()
        {
            if (_building == null)
                _building = GetComponent<BuildingController>();

            // Hide all initially
            HideAll();

            if (_constructionStages != null && _constructionStages.Length > 0)
            {
                _constructionStages[0]?.SetActive(true);
                _currentStage = 0;
            }
        }

        private void Update()
        {
            if (_building == null) return;

            if (!_building.IsConstructed && _building.CurrentBuildingState == Core.BuildingState.Constructing)
            {
                SetConstructionProgress(_building.ConstructionProgress);
            }
        }

        public void SetConstructionProgress(float progress)
        {
            progress = Mathf.Clamp01(progress);

            // Scale animation
            if (_useScaleAnimation)
            {
                float scale = Mathf.Lerp(_minScale, 1f, progress);
                transform.localScale = new Vector3(1f, scale, 1f);
            }

            // Stage switching
            if (_constructionStages != null && _constructionStages.Length > 0)
            {
                int targetStage = Mathf.FloorToInt(progress * _constructionStages.Length);
                targetStage = Mathf.Clamp(targetStage, 0, _constructionStages.Length - 1);

                if (targetStage != _currentStage)
                {
                    if (_currentStage >= 0 && _currentStage < _constructionStages.Length)
                        _constructionStages[_currentStage]?.SetActive(false);

                    _constructionStages[targetStage]?.SetActive(true);
                    _currentStage = targetStage;
                }
            }

            // Switch to completed model when done
            if (progress >= 1f)
            {
                ShowCompleted();
            }
        }

        public void ShowCompleted()
        {
            HideAll();
            if (_completedModel != null)
                _completedModel.SetActive(true);

            transform.localScale = Vector3.one;
        }

        public void ShowDestroyed()
        {
            HideAll();
            if (_destroyedModel != null)
                _destroyedModel.SetActive(true);
        }

        private void HideAll()
        {
            if (_constructionStages != null)
            {
                foreach (var stage in _constructionStages)
                {
                    stage?.SetActive(false);
                }
            }
            _completedModel?.SetActive(false);
            _destroyedModel?.SetActive(false);
        }
    }
}
