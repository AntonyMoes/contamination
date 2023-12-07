using _Game.Scripts.BurnMark.Game.Presentation.GameField;
using UnityEngine;

namespace _Game.Scripts.BurnMark {
    public class FieldTester : MonoBehaviour {
        [SerializeField] private Field _field;
        [SerializeField] private Vector2Int _size;

        private void OnEnable() {
            _field.Initialize(_size);
        }

        private void OnDisable() {
            _field.Clear();
        }
    }
}