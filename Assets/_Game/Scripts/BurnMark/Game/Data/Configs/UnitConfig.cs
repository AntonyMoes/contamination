using System;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.BurnMark.Game.Entities;
using _Game.Scripts.ModelV4.ECS;
using UnityEditor;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Data.Configs {
    public class UnitConfig : FieldEntityConfig {
        [SerializeField] private GameObject _prefab;
        public GameObject Prefab => _prefab;

        [SerializeField] private MoveData _moveData;
        public MoveData MoveData => _moveData;

        [SerializeField] private AttackData _attackData;
        public AttackData AttackData => _attackData;

        [SerializeField] private HealthData _healthData;
        public HealthData HealthData => _healthData;

        public Func<int, Entity> Create(int user, Vector2Int position) {
            return Unit.Create(user, position, this);
        }

        [MenuItem(Configs.MenuItem + nameof(UnitConfig), false)]
        public static void Create() => Configs.Create<UnitConfig>();
    }
}