using BFME2.Core;
using UnityEngine;

namespace BFME2.Units
{
    public class BattalionFactory : MonoBehaviour
    {
        [SerializeField] private Transform _unitParent;

        public static BattalionFactory Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            if (_unitParent == null)
            {
                _unitParent = new GameObject("Units").transform;
            }
        }

        public BattalionController CreateBattalion(UnitDefinition definition, Vector3 position, int ownerPlayerId, FactionId faction)
        {
            if (definition == null)
            {
                Debug.LogError("[BattalionFactory] UnitDefinition is null.");
                return null;
            }

            // Instantiate the battalion parent
            GameObject battalionObj;
            if (definition.BattalionPrefab != null)
            {
                battalionObj = Instantiate(definition.BattalionPrefab, position, Quaternion.identity, _unitParent);
            }
            else
            {
                battalionObj = new GameObject($"Battalion_{definition.DisplayName}");
                battalionObj.transform.position = position;
                battalionObj.transform.SetParent(_unitParent);

                // Add required components if not on prefab
                if (battalionObj.GetComponent<UnityEngine.AI.NavMeshAgent>() == null)
                    battalionObj.AddComponent<UnityEngine.AI.NavMeshAgent>();
                if (battalionObj.GetComponent<BattalionController>() == null)
                    battalionObj.AddComponent<BattalionController>();
                if (battalionObj.GetComponent<BattalionMovement>() == null)
                    battalionObj.AddComponent<BattalionMovement>();
                if (battalionObj.GetComponent<UnitCombatHandler>() == null)
                    battalionObj.AddComponent<UnitCombatHandler>();
            }

            battalionObj.layer = GameConstants.LAYER_INDEX_UNIT;

            var controller = battalionObj.GetComponent<BattalionController>();

            // Spawn individual soldiers
            SpawnSoldiers(controller, definition, position);

            // Initialize the battalion
            controller.Initialize(definition, ownerPlayerId, faction);

            return controller;
        }

        private void SpawnSoldiers(BattalionController battalion, UnitDefinition definition, Vector3 center)
        {
            var formation = definition.DefaultFormation;
            var positions = formation != null
                ? formation.GetPositionsForCount(definition.SoldiersPerBattalion, definition.SoldierSpacing)
                : GenerateDefaultPositions(definition.SoldiersPerBattalion, definition.SoldierSpacing);

            for (int i = 0; i < definition.SoldiersPerBattalion; i++)
            {
                GameObject soldierObj;
                if (definition.SoldierPrefab != null)
                {
                    soldierObj = Instantiate(definition.SoldierPrefab, battalion.transform);
                }
                else
                {
                    // Create placeholder soldier (capsule)
                    soldierObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                    soldierObj.transform.SetParent(battalion.transform);
                    soldierObj.transform.localScale = Vector3.one * 0.5f;
                }

                soldierObj.name = $"Soldier_{i}";
                soldierObj.layer = GameConstants.LAYER_INDEX_UNIT;

                var soldier = soldierObj.GetComponent<SoldierInstance>();
                if (soldier == null)
                    soldier = soldierObj.AddComponent<SoldierInstance>();

                soldier.Initialize(definition.HealthPerSoldier, i);
                soldier.UpdateFormationPosition(i < positions.Length ? positions[i] : Vector3.zero);
                battalion.RegisterSoldier(soldier);
            }
        }

        private Vector3[] GenerateDefaultPositions(int count, float spacing)
        {
            int cols = Mathf.CeilToInt(Mathf.Sqrt(count));
            var positions = new Vector3[count];
            float halfWidth = (cols - 1) * spacing * 0.5f;

            for (int i = 0; i < count; i++)
            {
                int row = i / cols;
                int col = i % cols;
                positions[i] = new Vector3(col * spacing - halfWidth, 0f, -row * spacing);
            }

            return positions;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}
