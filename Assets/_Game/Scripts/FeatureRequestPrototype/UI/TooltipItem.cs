using _Game.Scripts.FeatureRequestPrototype.Logic.Effects;
using TMPro;
using UnityEngine;

namespace _Game.Scripts.FeatureRequestPrototype.UI {
    public class TooltipItem : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI _label;

        private AppliedEffect _effect;

        public void Load(AppliedEffect effect) {
            _effect = effect;
            _effect.OnUpdate.Subscribe(Refresh);
            Refresh();
        }

        private void Refresh() {
            _label.text = _effect.GetSerialization();
        }

        private void OnDestroy() {
            _effect.OnUpdate.Unsubscribe(Refresh);
        }
    }
}