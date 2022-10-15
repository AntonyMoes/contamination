using System;
using _Game.Scripts.FeatureRequestPrototype.GameObjects;
using _Game.Scripts.FeatureRequestPrototype.Logic;
using JetBrains.Annotations;
using UnityEngine;

namespace _Game.Scripts.FeatureRequestPrototype.UI {
    public class HUDController : MonoBehaviour {
        [SerializeField] private SkillsPanel _skillsPanel;
        [SerializeField] private EmployeePanel _leftEmployeePanel;
        [SerializeField] private EmployeePanel _rightEmployeePanel;

        private EmployeeObject _selectedEmployee;
        private bool? _selectedLeft;

        private Employee _leftCurrentEmployee;
        private Employee _rightCurrentEmployee;

        public void SetSelectedEmployee(EmployeeObject employee, bool isLeft, Employee[] enemies, Employee[] allies, Action<Employee, Skill> startTargetSelection) {
            ClearSelectedEmployee();

            _selectedEmployee = employee;
            _selectedLeft = isLeft;
            _selectedEmployee.Selector.SetActive(true);
            _selectedEmployee.Selector.SetType(EmployeeSelector.ESelectionType.Current);

            _skillsPanel.Load(employee.Employee, enemies, allies, startTargetSelection);
            _skillsPanel.Show();
        }

        public void ResetSelectedEmployee() {
            ClearSelectedEmployee();
            _skillsPanel.Hide();
        }

        public void SetCurrentEmployee([CanBeNull] Employee employee, bool isLeft) {
            var newCurrentOrSelected = employee ?? GetSelected();
            SetCurrent(employee);

            var panel = GetPanel();
            if (newCurrentOrSelected != null) {
                panel.Load(newCurrentOrSelected);
                panel.Show();
            } else {
                panel.Hide();
            }

            EmployeePanel GetPanel() => isLeft ? _leftEmployeePanel : _rightEmployeePanel;
            Employee GetSelected() => isLeft == _selectedLeft ? _selectedEmployee.Employee : null;
            // Employee GetCurrent() => isLeft ? _leftCurrentEmployee : _rightCurrentEmployee;
            void SetCurrent(Employee newCurrent) {
                if (isLeft) {
                    _leftCurrentEmployee = newCurrent;
                } else {
                    _rightCurrentEmployee = newCurrent;
                }
            }
        }

        private void ClearSelectedEmployee() {
            if (_selectedEmployee != null) {
                _selectedEmployee.Selector.SetActive(false);
                _selectedEmployee = null;
                _selectedLeft = null;
            }
        }

        public void SwitchSides() {
            ClearSelectedEmployee();
            _skillsPanel.Hide();

            SetCurrentEmployee(_leftCurrentEmployee, true);
            SetCurrentEmployee(_rightCurrentEmployee, false);
        }

        public void Clear() {
            ClearSelectedEmployee();
            _skillsPanel.Hide();

            SetCurrentEmployee(null, true);
            SetCurrentEmployee(null, false);
        }
    }
}