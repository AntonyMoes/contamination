using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Data.Configs {
    public abstract class FieldObjectConfig : Config {
        [SerializeField] private Sprite _icon;
        public Sprite Icon => _icon;

        [SerializeField] private GameObject _prefab;
        public GameObject Prefab => _prefab;
    }
}