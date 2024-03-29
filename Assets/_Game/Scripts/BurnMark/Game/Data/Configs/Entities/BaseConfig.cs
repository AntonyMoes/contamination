﻿using System;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.BurnMark.Game.Entities;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Data.Configs.Entities {
    [CreateAssetMenu(menuName = Configs.EntityMenuItem + nameof(BaseConfig), fileName = nameof(BaseConfig))]
    public class BaseConfig : FieldObjectConfig {
        [SerializeField] private HealthData _healthData;
        public HealthData HealthData => _healthData;

        [SerializeField] private ResourceGainData _resourceGainData;
        public ResourceGainData ResourceGainData => _resourceGainData;

        [SerializeField] private UnitBuilderData _unitBuilderData;
        public UnitBuilderData UnitBuilderData => _unitBuilderData;

        public Func<int, Entity> Create(int user, Vector2Int position) {
            return Base.Create(user, position, this);
        }
    }
}