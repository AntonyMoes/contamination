using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.FeatureRequestPrototype.Logic;
using GeneralUtils;
using JetBrains.Annotations;
using Unity.Plastic.Newtonsoft.Json;

namespace _Game.Scripts.Data {
    [Serializable]
    public class SkillData : IData {
        [JsonProperty] private string name;
        [JsonProperty] private string description;
        [JsonProperty] private Target enemyTarget;
        [JsonProperty] private Target allyTarget;
        [JsonProperty] private bool allyTargetExcludeSelf;
        [JsonProperty] private int[] selfPositions;
        [JsonProperty] private EffectData[] selfEffects;
        [JsonProperty] private int accuracy;

        public string Name => name;
        public string Description { get; private set; }
        [CanBeNull] public Target EnemyTarget => enemyTarget;
        [CanBeNull] public Target AllyTarget => allyTarget;
        public bool AllyTargetExcludeSelf => allyTargetExcludeSelf;
        public int[] SelfPositions { get; private set; }
        public EffectData[] SelfEffects { get; private set; }
        public int Accuracy => accuracy;

        public List<string> LoadAndValidate() {
            Description = (description ?? "").Replace("\\n", "\n");

            var validationErrors = new List<string>();

            validationErrors.AddRange(LoadAndValidateEffects(selfEffects, out var loadedSelfEffects));
            SelfEffects = loadedSelfEffects;

            var targetErrors = new[] { enemyTarget, allyTarget }
                // .Select(t => t == default)
                .Where(t => t != null)
                .Select(t => t.LoadAndValidate())
                .Where(errors => errors.Count != 0)
                .Select(errors => "Could not validate target:\n" + string.Join("\n", errors.Select(e => $"\t* {e}")));
            validationErrors.AddRange(targetErrors);

            validationErrors.AddRange(LoadAndValidatePositions(selfPositions, out var loadedSelfPositions));
            SelfPositions = loadedSelfPositions;
            
            if (accuracy < 0) {
                validationErrors.Add($"Skill accuracy can't be < 0: {accuracy}");
            } else if (accuracy == 0) {
                accuracy = Constants.MaxAccuracy;
            }

            return validationErrors;
        }

        public static List<string> LoadAndValidatePositions(int[] positions, out int[] loadedPositions) {
            var validationErrors = new List<string>();
            var truePositions = positions ?? Array.Empty<int>();

            foreach (var position in truePositions) {
                if (position > Constants.MaxPosition || position < Constants.MinPosition) {
                    validationErrors.Add($"Invalid position {position}: should be >= {Constants.MinPosition} && <= {Constants.MaxPosition}");
                }
            }

            if (truePositions.ToHashSet().Count < truePositions.Length) {
                validationErrors.Add("Position sequence contains duplicates!");
            }

            loadedPositions = truePositions.OrderBy(p => p).ToArray();

            return validationErrors;
        }

        public static List<string> LoadAndValidateEffects(EffectData[] effects, out EffectData[] loadedEffects) {
            loadedEffects = effects ?? Array.Empty<EffectData>();
            return loadedEffects
                .Select(effect => effect.LoadAndValidate())
                .Where(errors => errors.Count != 0)
                .Select(errors => "Could not validate effect:\n" + string.Join("\n", errors.Select(e => $"\t* {e}")))
                .ToList();
        }
    }

    [Serializable]
    public class Target : IData {
        [JsonProperty] private int[] positions;
        [JsonProperty] private bool exclusive;
        [JsonProperty] private EffectData[] effects;
        
        public int[] Positions { get; private set; }
        public bool Exclusive => exclusive;
        public EffectData[] Effects { get; private set; }

        public List<string> LoadAndValidate() {
            var validationErrors = new List<string>();

            validationErrors.AddRange(SkillData.LoadAndValidatePositions(positions, out var loadedPositions));
            Positions = loadedPositions;

            validationErrors.AddRange(SkillData.LoadAndValidateEffects(effects, out var loadedEffects));
            Effects = loadedEffects;

            return validationErrors;
        }
    }
}