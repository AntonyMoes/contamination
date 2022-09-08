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
        [SerializeField] private int[] selfPositions;
        [SerializeField] private EffectData[] effects;
        [SerializeField] private EffectData[] selfEffects;

        public string Name => name;
        public string Description { get; private set; }
        [CanBeNull] public Target EnemyTarget => enemyTarget;
        [CanBeNull] public Target AllyTarget => allyTarget;
        public bool AllyTargetIncludeSelf => allyTargetIncludeSelf;
        public int[] SelfPositions { get; private set; }
        public EffectData[] Effects { get; private set; }
        public EffectData[] SelfEffects { get; private set; }

        public List<string> LoadAndValidate() {
            Description = (description ?? "").Replace("\\n", "\n");

            var validationErrors = new List<string>();

            Effects = effects ?? Array.Empty<EffectData>();
            SelfEffects = selfEffects ?? Array.Empty<EffectData>();

            var allEffects = Effects.Concat(SelfEffects);
            var effectErrors = allEffects
                .Select(effect => effect.LoadAndValidate())
                .Select(errors => "Could not validate effect:\n" + string.Join("\n", errors.Select(e => $"\t* {e}")));
            validationErrors.AddRange(effectErrors);

            var targetErrors = new[] { enemyTarget, allyTarget }
                .Where(t => t != null)
                .Select(t => t.LoadAndValidate())
                .Where(errors => errors.Count != 0)
                .Select(errors => "Could not validate target:\n" + string.Join("\n", errors.Select(e => $"\t* {e}")));
            validationErrors.AddRange(targetErrors);

            validationErrors.AddRange(LoadAndValidatePositions(selfPositions, out var loadedSelfPositions));
            SelfPositions = loadedSelfPositions;

            return validationErrors;
        }

        public static List<string> LoadAndValidatePositions(int[] positions, out int[] loadedPositions) {
            var validationErrors = new List<string>();
            var truePositions = positions ?? Array.Empty<int>();

            foreach (var position in truePositions) {
                if (position > 4 || position < 1) {
                    validationErrors.Add($"Invalid position {position}: should be >= 1 && <= 4");
                }
            }

            if (truePositions.ToHashSet().Count < truePositions.Length) {
                validationErrors.Add("Position sequence contains duplicates!");
            }

            loadedPositions = truePositions.OrderBy(p => p).ToArray();

            return validationErrors;
        }
    }

    [Serializable]
    public class Target : IData {
        [SerializeField] private int[] positions;
        [SerializeField] private bool exclusive;

        public int[] Positions { get; private set; }
        public bool Exclusive => exclusive;

        public List<string> LoadAndValidate() {
            var validationErrors = SkillData.LoadAndValidatePositions(positions, out var loadedPositions);
            Positions = loadedPositions;
            return validationErrors;
        }
    }
}