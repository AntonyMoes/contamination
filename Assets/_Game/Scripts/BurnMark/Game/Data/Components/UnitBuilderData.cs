using System;
using System.Linq;
using _Game.Scripts.BurnMark.Game.Data.Configs.Entities;
using _Game.Scripts.ModelV4.ECS;
using JetBrains.Annotations;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Data.Components {
    [Serializable]
    public struct UnitBuilderData : ISame<UnitBuilderData> {
        public const int CurrentPosition = -1;

        public UnitConfig[] BuildableUnits;
        [HideInInspector] public UnitConfig[] Queue;
        [HideInInspector] [CanBeNull] public UnitConfig Current;
        [HideInInspector] public int WorkLeft;

        public bool IsSame(UnitBuilderData other) {
            return BuildableUnits.ListEquals(other.BuildableUnits)
                && Queue.ListEquals(other.Queue)
                && Current == other.Current
                && WorkLeft == other.WorkLeft;
        }

        public UnitBuilderData BuildUnit(int position) {
            var config = BuildableUnits[position];
            if (Current == null) {
                return new UnitBuilderData {
                    BuildableUnits = BuildableUnits,
                    Queue = Queue,
                    Current = config,
                    WorkLeft = config.WorkToBuild
                };
            }

            return new UnitBuilderData {
                BuildableUnits = BuildableUnits,
                Queue = Queue.Append(config).ToArray(),
                Current = Current,
                WorkLeft = WorkLeft
            };
        }

        public UnitBuilderData CancelBuilding(int queuePosition) {
            if (queuePosition == CurrentPosition) {
                var newCurrent = Queue.Length > 0 ? Queue[0] : null;
                return new UnitBuilderData {
                    BuildableUnits = BuildableUnits,
                    Queue = newCurrent != null ? Queue.Skip(1).ToArray() : Queue,
                    Current = newCurrent,
                    WorkLeft = newCurrent != null ? newCurrent.WorkToBuild : 0
                };
            }

            var newQueue = new ArraySegment<UnitConfig>(Queue, 0, queuePosition)
                    .Concat(new ArraySegment<UnitConfig>(Queue, queuePosition + 1, Queue.Length - queuePosition - 1))
                    .ToArray();

            return new UnitBuilderData {
                BuildableUnits = BuildableUnits,
                Queue = newQueue,
                Current = Current,
                WorkLeft = WorkLeft
            };
        }

        public UnitBuilderData PerformWork([CanBeNull] out UnitConfig unitReadyToBeBuilt) {
            var workLeft = Math.Max(0, WorkLeft - 1);
            unitReadyToBeBuilt = workLeft == 0 ? Current : null;

            return new UnitBuilderData {
                BuildableUnits = BuildableUnits,
                Queue = Queue,
                Current = Current,
                WorkLeft = workLeft
            };
        }

        public UnitBuilderData FinishBuilding([CanBeNull] out UnitConfig builtUnit) {
            if (WorkLeft > 0) {
                builtUnit = null;
                return this;
            }

            builtUnit = Current;
            if (Queue.Length > 0) {
                return new UnitBuilderData {
                    BuildableUnits = BuildableUnits,
                    Queue = Queue.Skip(1).ToArray(),
                    Current = Queue[0],
                    WorkLeft = Queue[0].WorkToBuild
                };
            }

            return new UnitBuilderData {
                BuildableUnits = BuildableUnits,
                Queue = Array.Empty<UnitConfig>(),
                Current = null,
                WorkLeft = 0
            };
        }
    }
}