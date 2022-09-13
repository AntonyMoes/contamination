using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Data;
using _Game.Scripts.FeatureRequestPrototype.UI;
using UnityEngine;

namespace _Game.Scripts.FeatureRequestPrototype {
    public class Employee : MonoBehaviour {
        [SerializeField] private EmployeeSelector _selector;

        public EmployeeSelector Selector => _selector;
        public int Position => _positionProvider.Position;
        public Skill[] Skills { get; private set; }

        public string Name => _employeeData.Name;
        public EDepartment Department => _employeeData.Department;

        private EmployeeData _employeeData;
        private IPositionProvider _positionProvider;

        private void Awake() {
            Selector.SetActive(false);
        }

        public void Load(EmployeeData employeeData) {
            _employeeData = employeeData;
            Skills = employeeData.Skills
                .Select(skillData => new Skill(skillData))
                .ToArray();
        }

        public void SetPositionProvider(IPositionProvider positionProvider) {
            _positionProvider = positionProvider;
        }
    }
    
    public static class EmployeeHelper {
        public static Employee WithPosition(this IEnumerable<Employee> employees, int position) {
            return employees.First(employee => employee.Position == position);
        }
    }
}