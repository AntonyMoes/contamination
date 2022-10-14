using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.BaseUI;
using _Game.Scripts.FeatureRequestPrototype.Logic;
using _Game.Scripts.FeatureRequestPrototype.UI;
using UnityEngine;

namespace _Game.Scripts.FeatureRequestPrototype.GameObjects {
    public class EmployeeObject : MonoBehaviour {
        [SerializeField] private EmployeeSelector _selector;
        [SerializeField] private ProgressBar _sanityBar;

        public EmployeeSelector Selector => _selector;

        public Employee Employee { get; private set; }

        public void Load(Employee employee) {
            Employee = employee;

            _sanityBar.Load(0, Employee.MaxSanity);
            Employee.Sanity.Subscribe(value => _sanityBar.CurrentValue = value, true);
        }

        private void Awake() {
            Selector.SetActive(false);
        }
    }

    public static class EmployeeObjectHelper {
        public static EmployeeObject WithPosition(this IEnumerable<EmployeeObject> employees, int position) {
            return employees.First(employee => employee.Employee.Position == position);
        }
    }
}