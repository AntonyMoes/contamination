using _Game.Scripts.FeatureRequestPrototype.Logic.Effects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.FeatureRequestPrototype.UI {
    public class EffectItem : MonoBehaviour {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _text;

        public void Load(Effect effect) {
            _image.sprite = effect.Sprite;
            _text.text = effect.Serialization;
        }
    }
}