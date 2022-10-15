using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Game.Scripts.FeatureRequestPrototype.Logic {
    public class PositionSetter {
        private readonly Dictionary<int, IEmployeeContainer> _containers;

        public PositionSetter(IEnumerable<IEmployeeContainer> containers) {
            _containers = containers.ToDictionary(container => container.Position, container => container);
        }

        public int SetPosition(Employee employee, int newPosition) {
            var actualNewPosition = Mathf.Clamp(newPosition, Constants.MinPosition, Constants.MaxPosition);
            var currentPosition = employee.Position;
            if (actualNewPosition == currentPosition) {
                return actualNewPosition;
            }

            var iteration = actualNewPosition > currentPosition ? (Func<int, int>) Up : Down;
            for (var position = currentPosition; position != actualNewPosition; position = iteration(position)) {
                var nextPosition = iteration(position);
                if (_containers[nextPosition].Employee != null) {
                    _containers[position].SwapWith(_containers[nextPosition]);
                }
            }

            return actualNewPosition;

            static int Up(int idx) => idx + 1;
            static int Down(int idx) => idx - 1;
        }
    }
}