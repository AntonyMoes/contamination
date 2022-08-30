using System;
using System.Collections.Generic;
using System.Linq;
using GeneralUtils;
using JetBrains.Annotations;
using UnityEngine;

namespace _Game.Scripts.Data {
    [Serializable]
    public class SkillData : IData {
        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField] [CanBeNull] private Target enemyTarget;
        [SerializeField] [CanBeNull] private Target allyTarget;
        [SerializeField] private bool allyTargetIncludeSelf;
        [SerializeField] private EffectData[] effects;
        [SerializeField] private EffectData[] selfEffects;

        public string Name => name;
        public string Description { get; private set; }

        public (int[] enemyPositions, int[] allyPositions, bool exclusive) GetTargetPositions(int selfPosition) {
            throw new NotImplementedException();
        }

        public List<string> LoadAndValidate() {
            Description = description.Replace("\\n", "\n");

            var validationErrors = new List<string>();

            var allEffects = effects.Concat(selfEffects);
            var effectErrors = allEffects
                .Select(effect => effect.LoadAndValidate())
                .Select(errors => "Could not validate effect:\n" + string.Join("\n", errors.Select(e => $"\t* {e}")));
            validationErrors.AddRange(effectErrors);

            var targetErrors = new[] { enemyTarget, allyTarget }
                .Select(t => t.LoadAndValidate())
                .Select(errors => "Could not validate target:\n" + string.Join("\n", errors.Select(e => $"\t* {e}")));
            validationErrors.AddRange(targetErrors);

            return validationErrors;
        }
    }

    [Serializable]
    public class Target : IData {
        [SerializeField] private int[] positions;
        [SerializeField] private bool exclusive;

        public int[] Positions => positions;
        public bool Exclusive => exclusive;

        public List<string> LoadAndValidate() {
            var validationErrors = new List<string>();

            foreach (var position in positions) {
                if (position > 4 || position < 1) {
                    validationErrors.Add($"Invalid position {position}: should be >=1 && <=4");
                }
            }

            if (validationErrors.Count > 0) {
                return validationErrors;
            }

            if (Positions.ToHashSet().Count < Positions.Length) {
                validationErrors.Add("Target position sequence contains duplicates!");
            }

            return validationErrors;
        }
    }
}