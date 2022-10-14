using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.BaseUI;
using _Game.Scripts.Data;
using _Game.Scripts.FeatureRequestPrototype.UI;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.FeatureRequestPrototype.Logic {
    public class Employee : MonoBehaviour {
        [SerializeField] private EmployeeSelector _selector;
        [SerializeField] private ProgressBar _sanityBar;

        public EmployeeSelector Selector => _selector;
        public int Position => _positionProvider.Position;
        public Skill[] Skills { get; private set; }

        public string Name => _employeeData.Name;
        public EDepartment Department => _employeeData.Department;

        public int MaxSanity => _employeeData.Sanity;
        public readonly UpdatedValue<int> Sanity = new UpdatedValue<int>();


        // public IEnumerable<Effect> appliedEffects

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

            Sanity.Value = MaxSanity;
            
            _sanityBar.Load(0, MaxSanity);
            Sanity.Subscribe(value => _sanityBar.CurrentValue = value, true);
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