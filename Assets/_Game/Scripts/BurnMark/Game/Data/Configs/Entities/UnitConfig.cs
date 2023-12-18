using System;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.BurnMark.Game.Entities;
using _Game.Scripts.BurnMark.Game.Presentation.Entities;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace _Game.Scripts.BurnMark.Game.Data.Configs.Entities {
    public class UnitConfig : FieldEntityConfig {
        [SerializeField] private FieldEntity _prefab;
        public FieldEntity Prefab => _prefab;

        [SerializeField] private MoveData _moveData;
        public MoveData MoveData => _moveData;

        [SerializeField] private AttackData _attackData;
        public AttackData AttackData => _attackData;

        [SerializeField] private HealthData _healthData;
        public HealthData HealthData => _healthData;

        [SerializeField] private Cost _cost;
        public Cost Cost => _cost;

        [SerializeField] private int _workToBuild;
        public int WorkToBuild => _workToBuild;

        public Func<int, Entity> Create(int user, Vector2Int position) {
            return Unit.Create(user, position, this);
        }

#if UNITY_EDITOR
        [MenuItem(Configs.EntityMenuItem + nameof(UnitConfig), false)]
        public static void Create() => Configs.Create<UnitConfig>();
#endif
    }
}