using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.BaseUI;
using _Game.Scripts.Data;
using UnityEditor.Graphs;
using UnityEngine;

namespace _Game.Scripts.FeatureRequestPrototype.UI {
    public class TestRoundRunner : MonoBehaviour {
        [SerializeField] private EmployeeSlot[] _leftSlots;
        [SerializeField] private EmployeeSlot[] _rightSlots;
        [SerializeField] private Employee _employeePrefab;
        [SerializeField] private SimpleButton _startButton;

        private void Awake() {
            _startButton.OnClick.Subscribe(StartTest);
        }

        private void StartTest(SimpleButton _) {
            DataStorage.Instance.Init();

            foreach (var slot in _leftSlots.Concat(_rightSlots)) {
                slot.Clear();
                slot.InstantiateEmployee(_employeePrefab, DataStorage.Instance.Employees.First());
            }

            SelectUnit(_leftSlots.Select(slot => slot.Employee).ToArray());
        }

        private void SelectUnit(Employee[] employees) {
            var groups = Array.Empty<EmployeeSelectorGroup>();
            groups = employees
                .Select(employee => new EmployeeSelectorGroup(EmployeeSelector.SelectionType.UnitToUse, OnSelect, employee.Selector))
                .ToArray();

            void OnSelect(EmployeeSelector[] selected) {
                foreach (var group in groups) {
                    group.Dispose();
                }

                var selectedPositions = selected
                    .Select(selector => employees.First(employee => employee.Selector == selector).Position);
                Debug.Log($"Selected indices: {string.Join(",", selectedPositions)}");
            }
        }

        [Serializable]
        private struct UnitPosition {
            public int position;
            public EmployeeSlot slot;
        }
    }
}