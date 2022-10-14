using _Game.Scripts.Data;
using _Game.Scripts.FeatureRequestPrototype.Logic;
using JetBrains.Annotations;
using UnityEngine;

namespace _Game.Scripts.FeatureRequestPrototype.GameObjects {
    public class EmployeeSlot : MonoBehaviour , IPositionProvider {
        [SerializeField] private Transform _slot;
        [SerializeField] private int _position;

        public int Position => _position;
        [CanBeNull] public EmployeeObject Employee { get; private set; }

        public void InstantiateEmployee(EmployeeObject prefab, EmployeeData employeeData) {
            var employee = new Employee(employeeData, this);
            var employeeObject = Instantiate(prefab, _slot);
            employeeObject.Load(employee);

            Employee = employeeObject;
        }

        public void Clear() {
            if (Employee != null) {
                Destroy(Employee.gameObject);
            }

            Employee = null;
        }
    }
}