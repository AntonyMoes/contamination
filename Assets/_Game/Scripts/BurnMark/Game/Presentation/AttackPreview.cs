using _Game.Scripts.BaseUI;
using _Game.Scripts.BurnMark.Game.Data.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.BurnMark.Game.Presentation {
    public class AttackPreview : MonoBehaviour {
        [SerializeField] private Image _targetIcon;
        [SerializeField] private TextMeshProUGUI _targetName;
        [SerializeField] private ProgressBar _targetHealth;
        [SerializeField] private ProgressBar _estimatedHealth;
        [SerializeField] private TextMeshProUGUI _damage;

        public void Initialize(Sprite icon, string targetName, HealthData data, float damage) {
            _targetIcon.sprite = icon;
            _targetName.text = targetName;
            _targetHealth.Load(0, data.MaxHealth);
            _targetHealth.CurrentValue = data.Health;
            _estimatedHealth.Load(0, data.MaxHealth);
            _estimatedHealth.CurrentValue = data.Health - damage;
            _damage.text = damage.ToString("F1");
        }
    }
}