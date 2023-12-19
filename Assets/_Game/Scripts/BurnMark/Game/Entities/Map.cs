using System;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Entities {
    public static class Map {
        public static Func<int, Entity> Create(Vector2Int size) {
            var mapComponent = Entity.Add(new MapData {
                Size = size
            });
            return id => new Entity(id, mapComponent);
        }
    }
}