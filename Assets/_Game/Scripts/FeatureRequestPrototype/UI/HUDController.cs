using System;
using JetBrains.Annotations;
using UnityEngine;

namespace _Game.Scripts.FeatureRequestPrototype.UI {
    public class HUDController : MonoBehaviour {
        [SerializeField] private SkillPanel _skillPanel;

        private Employee _selectedEmployee;

        public void SetSelectedEmployee(Employee employee, Employee[] enemies, Employee[] allies, Action<Employee, Skill> startTargetSelection) {
            ClearSelectedEmployee();

            _selectedEmployee = employee;
            _selectedEmployee.Selector.SetActive(true);
            _selectedEmployee.Selector.SetType(EmployeeSelector.SelectionType.Current);

            _skillPanel.Load(employee, enemies, allies, startTargetSelection);
            _skillPanel.Show();
        }

        public void ResetSelectedEmployee() {
            ClearSelectedEmployee();
            _skillPanel.Hide();
        }

        private void ClearSelectedEmployee() {
            if (_selectedEmployee != null) {
                _selectedEmployee.Selector.SetActive(false);
                _selectedEmployee = null;
            }
        }

        public void Clear() {
            ClearSelectedEmployee();
            _skillPanel.Hide();
        }
    }
}