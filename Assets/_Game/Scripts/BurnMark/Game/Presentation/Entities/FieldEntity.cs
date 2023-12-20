using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Presentation.Entities {
    public class FieldEntity : MonoBehaviour {
        [SerializeField] private Renderer[] _coloredRenders;
        [SerializeField] private Transform _iconTarget;
        public Transform IconTarget => _iconTarget;

        public void SetColor(Color color) {
            foreach (var renderer in _coloredRenders) {
                renderer.material.color = color;
            }
        }
    }
}