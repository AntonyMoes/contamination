using _Game.Scripts.BaseUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.FeatureRequestPrototype.UI {
    public class EmployeePanel : UIElement {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _department;
        [SerializeField] private Image _image;

        public void Load(Employee employee) {
            _name.text = employee.Name;
            _department.text = employee.Department.ToString();
        }
    }
}