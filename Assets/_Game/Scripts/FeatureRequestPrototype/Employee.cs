using _Game.Scripts.Data;
using _Game.Scripts.FeatureRequestPrototype.UI;
using UnityEngine;

namespace _Game.Scripts.FeatureRequestPrototype {
    public class Employee : MonoBehaviour {
        [SerializeField] private EmployeeSelector _selector;

        public EmployeeSelector Selector => _selector;
        public int Position => _positionProvider.Position;

        private EmployeeData _employeeData;
        private IPositionProvider _positionProvider;

        public void Load(EmployeeData employeeData) {
            _employeeData = employeeData;
        }

        public void SetPositionProvider(IPositionProvider positionProvider) {
            _positionProvider = positionProvider;
        }
    }
}