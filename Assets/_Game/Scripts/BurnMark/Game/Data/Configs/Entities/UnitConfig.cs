using System;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.BurnMark.Game.Entities;
using _Game.Scripts.BurnMark.Game.Presentation.Entities;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Data.Configs.Entities {
    [CreateAssetMenu(menuName = Configs.EntityMenuItem + nameof(UnitConfig), fileName = nameof(UnitConfig))]
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
    }
}