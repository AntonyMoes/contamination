using _Game.Scripts.BurnMark.Game.Presentation.Entities;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Data.Configs.Entities {
    public abstract class FieldObjectConfig : FieldEntityConfig {
        [SerializeField] private FieldEntity _prefab;
        public FieldEntity Prefab => _prefab;
    }
}