﻿using System;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.BurnMark.Game.Entities;
using _Game.Scripts.ModelV4.ECS;
using UnityEditor;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Data.Configs {
    public class BaseConfig : FieldObjectConfig {
        [SerializeField] private HealthData _healthData;
        public HealthData HealthData => _healthData;

        [SerializeField] private ResourceGainData _resourceGainData;
        public ResourceGainData ResourceGainData => _resourceGainData;

        public Func<int, Entity> Create(int user, Vector2Int position) {
            return Base.Create(user, position, this);
        }

        [MenuItem(Configs.MenuItem + nameof(BaseConfig), false)]
        public static void Create() => Configs.Create<BaseConfig>();
    }
}