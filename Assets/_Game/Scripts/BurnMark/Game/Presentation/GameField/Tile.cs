using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameField {
    public class Tile : MonoBehaviour {
        [SerializeField] private Transform _center;
        public Transform Center => _center;
    }
}