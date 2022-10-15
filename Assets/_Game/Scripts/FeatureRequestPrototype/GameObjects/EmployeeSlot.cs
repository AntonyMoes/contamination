using _Game.Scripts.Data;
using _Game.Scripts.FeatureRequestPrototype.Logic;
using JetBrains.Annotations;
using UnityEngine;

namespace _Game.Scripts.FeatureRequestPrototype.GameObjects {
    public class EmployeeSlot : MonoBehaviour , IEmployeeContainer {
        [SerializeField] private Transform _slot;
        [SerializeField] private int _position;

        public int Position => _position;

        [CanBeNull] public EmployeeObject EmployeeObject { get; private set; }
        [CanBeNull] public Employee Employee => EmployeeObject != null ? EmployeeObject.Employee : null;

        public void InstantiateEmployee(EmployeeObject prefab, EmployeeData employeeData, PositionSetter positionSetter) {
            var employee = new Employee(employeeData, positionSetter, _position);
            var employeeObject = Instantiate(prefab, _slot);
            employeeObject.Load(employee);

            EmployeeObject = employeeObject;
            ResetEmployee();
        }

        private void ResetEmployee() {
            if (EmployeeObject == null) {
                return;
            }

            var employeeTransform = EmployeeObject.transform; 
            employeeTransform.parent = _slot;
            employeeTransform.localPosition = Vector3.zero;
        }

        public void SwapWith(IEmployeeContainer other) {
            var otherSlot = (EmployeeSlot) other;
            (EmployeeObject, otherSlot.EmployeeObject) = (otherSlot.EmployeeObject, EmployeeObject);

            ResetEmployee();
            otherSlot.ResetEmployee();

            Debug.Log($"Swapping slot {Position} ({Employee?.Name}) and slot {otherSlot.Position} ({otherSlot.Employee?.Name})");
        }

        public void Clear() {
            if (EmployeeObject != null) {
                Destroy(EmployeeObject.gameObject);
            }

            EmployeeObject = null;
        }
    }
}