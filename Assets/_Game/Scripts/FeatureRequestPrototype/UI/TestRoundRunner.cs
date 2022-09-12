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
        [SerializeField] private HUDController _hudController;

        private Dictionary<Employee, bool> _allies;  // employee, wasUsed
        private Dictionary<Employee, bool> _enemies;  // employee, wasUsed
        private Selection.EmployeeSelectionProcess _employeeSelectionProcess;
        private Selection.SkillSelectionProcess _skillSelectionProcess;

        private IEnumerable<EmployeeSlot> Slots => _leftSlots.Concat(_rightSlots);

        private void Awake() {
            _startButton.OnClick.Subscribe(StartTest);
        }

        private void StartTest(SimpleButton _) {
            DataStorage.Instance.Init();

            var allies = DataStorage.Instance.Employees.Take(4).ToArray();
            var enemies = DataStorage.Instance.Employees.Take(4).ToArray();
            StartGame(allies, enemies);
        }

        private void StartGame(EmployeeData[] allies, EmployeeData[] enemies) {
            Clear();

            _allies = CreateEmployees(allies, _leftSlots, _employeePrefab);
            _enemies = CreateEmployees(enemies, _rightSlots, _employeePrefab);

            static Dictionary<Employee, bool> CreateEmployees(EmployeeData[] data, EmployeeSlot[] slots, Employee prefab) {
                return data
                    .Zip(slots, (d, s) => (d, s))
                    .Select(pair => {
                        pair.s.InstantiateEmployee(prefab, pair.d);
                        return pair.s.Employee;
                    })
                    .ToDictionary(employee => employee, _ => false);
            }

            StartRound();
        }

        private void StartRound() {
            ResetUsage(_allies);
            ResetUsage(_enemies);

            StartTurn();

            static void ResetUsage(Dictionary<Employee, bool> employeeDictionary) {
                var employees = employeeDictionary.Keys.ToArray();
                foreach (var employee in employees) {
                    employeeDictionary[employee] = false;
                }
            }
        }

        private void StartTurn() {
            var groups = _allies
                .Where(pair => pair.Value == false)
                .Select(pair => new[] { pair.Key })
                .ToArray();

            _employeeSelectionProcess = new Selection.EmployeeSelectionProcess(groups, EmployeeSelector.SelectionType.UnitToUse, selected => {
                _employeeSelectionProcess = null;

                // only one selected in this case
                var employee = selected.First();
                _allies[employee] = true;

                _hudController.SetSelectedEmployee(employee, _enemies.Keys.ToArray(), _allies.Keys.ToArray(), StartTargetSelection);
            });
        }

        private void PassTurn() {
            (_allies, _enemies) = (_enemies, _allies);

            if (_allies.All(pair => pair.Value)) {
                if (_enemies.All(pair => pair.Value)) {
                    StartRound();
                } else {
                    PassTurn();
                }
            } else {
                StartTurn();
            }
        }

        private void StartTargetSelection(Employee employee, Skill skill) {
            Debug.Log($"Start target selection for skill {skill.Name}");
            _skillSelectionProcess?.Abort();

            var enemies = _enemies.Keys.ToArray();
            var allies = _allies.Keys.ToArray();
            _skillSelectionProcess = new Selection.SkillSelectionProcess(employee, skill, enemies, allies,
                (selectedEnemies, selectedAllies) => {
                    OnSelectedTargets(employee, skill, selectedEnemies, selectedAllies);
                });
        }

        private void OnSelectedTargets(Employee employee, Skill skill, Employee[] selectedEnemies, Employee[] selectedAllies) {
            _skillSelectionProcess = null;
            Debug.LogWarning($"Employee: {employee.Position}\nSkill: {skill.Name}\n" +
                             $"Enemies: {string.Join(",", selectedEnemies.Select(e => e.Position))}\n" +
                             $"Allies: {string.Join(",", selectedAllies.Select(e => e.Position))}\n");

            _hudController.ResetSelectedEmployee();

            PassTurn();
        }

        private void Clear() {
            foreach (var slot in Slots) {
                slot.Clear();
            }

            _employeeSelectionProcess?.Abort();
            _employeeSelectionProcess = null;
            _skillSelectionProcess?.Abort();
            _skillSelectionProcess = null;

            _hudController.Clear();
        }
    }
}