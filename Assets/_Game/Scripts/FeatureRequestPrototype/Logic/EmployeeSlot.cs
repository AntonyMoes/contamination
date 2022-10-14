using _Game.Scripts.Data;
using JetBrains.Annotations;
using UnityEngine;

namespace _Game.Scripts.FeatureRequestPrototype.Logic {
    public class EmployeeSlot : MonoBehaviour , IPositionProvider {
        [SerializeField] private Transform _slot;
        [SerializeField] private int _position;

        public int Position => _position;
        [CanBeNull] public Employee Employee { get; private set; }

        public void InstantiateEmployee(Employee prefab, EmployeeData employeeData) {
            var employee = Instantiate(prefab, _slot);
            employee.Load(employeeData);
            employee.SetPositionProvider(this);

            Employee = employee;
        }

        public void Clear() {
            if (Employee != null) {
                Destroy(Employee.gameObject);
            }

            Employee = null;
        }
    }
}