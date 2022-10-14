using _Game.Scripts.BaseUI;
using _Game.Scripts.FeatureRequestPrototype.Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.FeatureRequestPrototype.UI {
    public class EmployeePanel : UIElement {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _department;
        [SerializeField] private TextMeshProUGUI _sanity;
        [SerializeField] private Image _image;

        private Employee _employee;

        public void Load(Employee employee) {
            if (_employee != employee) {
                Clear();
            }
            
            _employee = employee;
            _name.text = employee.Name;
            _department.text = employee.Department.ToString();
            
            employee.Sanity.Subscribe(OnSanityChange, true);
        }

        private void OnSanityChange(int newSanity) {
            _sanity.text = $"Sanity: {newSanity}/{_employee.MaxSanity}";
        }

        public override void Clear() {
            base.Clear();   

            if (_employee != null) {
                _employee.Sanity.Unsubscribe(OnSanityChange);
                _employee = null;
            }
        }
    }
}