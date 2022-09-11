using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.BaseUI;
using _Game.Scripts.Data;
using _Game.Scripts.FeatureRequestPrototype.Utils;
using UnityEngine;

namespace _Game.Scripts.FeatureRequestPrototype.UI {
    public class TestRoundRunner : MonoBehaviour {
        [SerializeField] private EmployeeSlot[] _leftSlots;
        [SerializeField] private EmployeeSlot[] _rightSlots;
        [SerializeField] private Employee _employeePrefab;
        [SerializeField] private SimpleButton _startButton;
        [SerializeField] private SkillPanel _skillPanel;

        private EmployeeSlot[] _allies;
        private EmployeeSlot[] _enemies;
        private Selection.SkillSelectionProcess _skillSelectionProcess;

        private IEnumerable<EmployeeSlot> Slots => _leftSlots.Concat(_rightSlots);

        private void Awake() {
            _startButton.OnClick.Subscribe(StartTest);
        }

        private void StartTest(SimpleButton _) {
            DataStorage.Instance.Init();

            Clear();

            foreach (var slot in Slots) {
                slot.InstantiateEmployee(_employeePrefab, DataStorage.Instance.Employees.First());
            }

            _allies = _leftSlots;
            _enemies = _rightSlots;

            var allies = _allies.Select(slot => slot.Employee).ToArray();
            var enemies = _enemies.Select(slot => slot.Employee).ToArray();

            var groups = allies
                .Select(employee => new[] { employee })
                .ToArray();

            Selection.SelectEmployees(groups, selected => {
                // only one selected in this case
                var employee = selected.First();
                _skillPanel.Load(employee, enemies, allies, StartTargetSelection);
                _skillPanel.Show();
            });
        }

        private void StartTargetSelection(Employee employee, Skill skill) {
            Debug.Log($"Start target selection for skill {skill.Name}");
            _skillSelectionProcess?.Abort();

            var enemies = _enemies.Select(slot => slot.Employee).ToArray();
            var allies = _allies.Select(slot => slot.Employee).ToArray();
            _skillSelectionProcess = new Selection.SkillSelectionProcess(employee, skill, enemies, allies,
                (selectedEnemies, selectedAllies) => {
                    OnSelectedTargets(employee, skill, selectedEnemies, selectedAllies);
                });
        }

        private void OnSelectedTargets(Employee employee, Skill skill, Employee[] selectedEnemies, Employee[] selectedAllies) {
            Debug.LogWarning($"Employee: {employee.Position}\nSkill: {skill.Name}\n" +
                             $"Enemies: {string.Join(",", selectedEnemies.Select(e => e.Position))}\n" +
                             $"Allies: {string.Join(",", selectedAllies.Select(e => e.Position))}\n");
        }

        private void Clear() {
            foreach (var slot in Slots) {
                slot.Clear();
            }

            _skillPanel.Hide();
        }
    }
}