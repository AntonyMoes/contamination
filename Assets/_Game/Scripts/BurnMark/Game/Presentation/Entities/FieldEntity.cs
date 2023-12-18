using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Presentation.Entities {
    public class FieldEntity : MonoBehaviour {
        [SerializeField] private Renderer[] _coloredRenders;

        public void SetColor(Color color) {
            foreach (var renderer in _coloredRenders) {
                renderer.material.color = color;
            }
        }
    }
}