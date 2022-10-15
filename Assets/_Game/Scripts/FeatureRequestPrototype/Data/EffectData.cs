using System;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;

namespace _Game.Scripts.Data {
    [Serializable]
    public class EffectData : IData {
        [JsonProperty] private string type;
        [JsonProperty] private int powerMin;
        [JsonProperty] private int powerMax;
        [JsonProperty] private int duration;
        // [JsonProperty] private int accuracy;

        public EEffectType EffectType { get; private set; }
        public int PowerMin => powerMin;
        public int PowerMax => powerMax;
        public int Duration => duration;
        // public int Accuracy => accuracy;

        public List<string> LoadAndValidate() {
            var validationErrors = new List<string>();

            if (!Enum.TryParse(type, out EEffectType effectType)) {
                validationErrors.Add($"Could not parse effect type \"{type}\"!");
            } else {
                EffectType = effectType;
            }

            if (powerMin < 0) {
                validationErrors.Add($"Effect powerMin can't be < 0: {powerMin}");
            }

            if (powerMax < 0 || powerMax < powerMin) {
                validationErrors.Add($"Effect powerMax can't be < 0 and < powerMin: {powerMax}");
            }

            if (duration < 0) {
                validationErrors.Add($"Effect duration can't be < 0: {duration}");
            }

            // if (accuracy < 0) {
            //     validationErrors.Add($"Effect accuracy can't be < 0: {accuracy}");
            // } else if (accuracy == 0) {
            //     accuracy = 100;
            // }

            return validationErrors;
        }
    }
}