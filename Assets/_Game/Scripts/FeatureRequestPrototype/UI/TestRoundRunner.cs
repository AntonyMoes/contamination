﻿using System;
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

        private Dictionary<Employee, bool> _left;  // employee, wasUsed
        private Dictionary<Employee, bool> _right;  // employee, wasUsed
        private Selection.EmployeeSelectionProcess _employeeSelectionProcess;
        private Selection.SkillSelectionProcess _skillSelectionProcess;

        private bool _currentSideIsLeft;
        private Dictionary<Employee, bool> Allies => _currentSideIsLeft ? _left : _right;
        private Dictionary<Employee, bool> Enemies => _currentSideIsLeft ? _right : _left;

        private IEnumerable<EmployeeSlot> Slots => _leftSlots.Concat(_rightSlots);

        private void Awake() {
            _startButton.OnClick.Subscribe(StartTest);
        }

        private void StartTest(SimpleButton _) {
            DataStorage.Instance.Init();

            var left = DataStorage.Instance.Employees.Take(4).ToArray();
            var right = DataStorage.Instance.Employees.Take(4).ToArray();
            StartGame(left, right, true);
        }

        private void StartGame(EmployeeData[] left, EmployeeData[] right, bool startingSideIsLeft) {
            Clear();

            _left = CreateEmployees(left, _leftSlots, _employeePrefab, employee => _hudController.SetCurrentEmployee(employee, true));
            _right = CreateEmployees(right, _rightSlots, _employeePrefab, employee => _hudController.SetCurrentEmployee(employee, false));
            _currentSideIsLeft = startingSideIsLeft;

            static Dictionary<Employee, bool> CreateEmployees(EmployeeData[] data, EmployeeSlot[] slots, Employee prefab, Action<Employee> onHover) {
                return data
                    .Zip(slots, (d, s) => (d, s))
                    .Select(pair => {
                        pair.s.InstantiateEmployee(prefab, pair.d);
                        var employee = pair.s.Employee!;
                        employee.Selector.Button.OnHover.Subscribe((_, hover) => onHover?.Invoke(hover ? employee : null));
                        return employee;
                    })
                    .ToDictionary(employee => employee, _ => false);
            }

            StartRound();
        }

        private void StartRound() {
            ResetUsage(_left);
            ResetUsage(_right);

            StartTurn();

            static void ResetUsage(Dictionary<Employee, bool> employeeDictionary) {
                var employees = employeeDictionary.Keys.ToArray();
                foreach (var employee in employees) {
                    employeeDictionary[employee] = false;
                }
            }
        }

        private void StartTurn() {
            var groups = Allies
                .Where(pair => pair.Value == false)
                .Select(pair => new[] { pair.Key })
                .ToArray();

            _employeeSelectionProcess = new Selection.EmployeeSelectionProcess(groups, EmployeeSelector.SelectionType.UnitToUse, selected => {
                _employeeSelectionProcess = null;

                // only one selected in this case
                var employee = selected.First();
                Allies[employee] = true;

                _hudController.SetSelectedEmployee(employee, _currentSideIsLeft, Enemies.Keys.ToArray(), Allies.Keys.ToArray(), StartTargetSelection);
            });
        }

        private void PassTurn() {
            _hudController.Clear();
            _currentSideIsLeft = !_currentSideIsLeft;

            if (Allies.All(pair => pair.Value)) {
                if (Enemies.All(pair => pair.Value)) {
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

            var enemies = Enemies.Keys.ToArray();
            var allies = Allies.Keys.ToArray();
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