using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Data.Configs.Entities {
    public abstract class FieldObjectConfig : FieldEntityConfig {
        [SerializeField] private GameObject _prefab;
        public GameObject Prefab => _prefab;
    }
}