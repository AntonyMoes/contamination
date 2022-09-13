using System;
using JetBrains.Annotations;
using UnityEngine;

namespace _Game.Scripts.FeatureRequestPrototype.UI {
    public class HUDController : MonoBehaviour {
        [SerializeField] private SkillsPanel _skillsPanel;
        [SerializeField] private EmployeePanel _leftEmployeePanel;
        [SerializeField] private EmployeePanel _rightEmployeePanel;

        private Employee _selectedEmployee;
        private bool? _selectedLeft;

        private Employee _leftCurrentEmployee;
        private Employee _rightCurrentEmployee;

        public void SetSelectedEmployee(Employee employee, bool isLeft, Employee[] enemies, Employee[] allies, Action<Employee, Skill> startTargetSelection) {
            ClearSelectedEmployee();

            _selectedEmployee = employee;
            _selectedLeft = isLeft;
            _selectedEmployee.Selector.SetActive(true);
            _selectedEmployee.Selector.SetType(EmployeeSelector.SelectionType.Current);

            _skillsPanel.Load(employee, enemies, allies, startTargetSelection);
            _skillsPanel.Show();
        }

        public void ResetSelectedEmployee() {
            ClearSelectedEmployee();
            _skillsPanel.Hide();
        }

        public void SetCurrentEmployee([CanBeNull] Employee employee, bool isLeft) {
            var panel = GetPanel();

            var newCurrent = employee != null ? employee : GetSelected();
            var current = GetCurrent();

            if (newCurrent != current) {
                SetCurrent(newCurrent);
            }

            if (newCurrent != null) {
                panel.Load(newCurrent);
                panel.Show();
            } else {
                panel.Hide();
            }

            EmployeePanel GetPanel() => isLeft ? _leftEmployeePanel : _rightEmployeePanel;
            Employee GetSelected() => isLeft == _selectedLeft ? _selectedEmployee : null;
            Employee GetCurrent() => isLeft ? _leftCurrentEmployee : _rightCurrentEmployee;
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

        public void Clear() {
            ClearSelectedEmployee();
            _skillsPanel.Hide();

            SetCurrentEmployee(null, true);
            SetCurrentEmployee(null, false);
        }
    }
}