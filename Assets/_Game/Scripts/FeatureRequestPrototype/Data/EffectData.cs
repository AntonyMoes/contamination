using System;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;

namespace _Game.Scripts.FeatureRequestPrototype.Data {
    [Serializable]
    public class EffectData : IData {
        [JsonProperty] private string type;
        [JsonProperty] private int powerMin;
        [JsonProperty] private int powerMax;
        [JsonProperty] private int duration;

        public EEffectType EffectType { get; private set; }
        public int PowerMin { get; private set; }
        public int PowerMax { get; private set; }
        public int Duration { get; private set; }

        public List<string> LoadAndValidate() {
            var validationErrors = new List<string>();

            if (!Enum.TryParse(type, out EEffectType effectType)) {
                validationErrors.Add($"Could not parse effect type \"{type}\"!");
            } else {
                EffectType = effectType;
            }

            if (powerMin < 0) {
                validationErrors.Add($"Effect powerMin can't be < 0: {powerMin}");
            } else {
                PowerMin = powerMin;
            }

            if (powerMax < 0 || powerMax < powerMin) {
                validationErrors.Add($"Effect powerMax can't be < 0 and < powerMin: {powerMax}");
            } else {
                PowerMax = powerMax;
            }

            if (duration < 0) {
                validationErrors.Add($"Effect duration can't be < 0: {duration}");
            } else {
                Duration = duration;
            }

            return validationErrors;
        }

        public static EffectData CreateFakeMoveData(int moveForward, int moveBackward) {
            return new EffectData {
                EffectType = EEffectType.Move,
                PowerMin = -moveForward,
                PowerMax = moveBackward,
                Duration = 0
            };
        }
    }
}