using System;
using System.Linq;
using _Game.Scripts.FeatureRequestPrototype.Utils;
using GeneralUtils;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace _Game.Scripts.Data {
    public class DataStorage : SingletonBehaviour<DataStorage> {
        [SerializeField] private TextAsset _employees;
        [SerializeField] private TextAsset _skills;
        
        public EmployeeData[] Employees { get; private set; }
        public SkillData[] Skills { get; private set; }

        public void Init() {
            // order matters
            Skills = LoadRecords<SkillData>(_skills);
            Employees = LoadRecords<EmployeeData>(_employees);
        }

        private static T[] LoadRecords<T>(TextAsset asset) where T : IData {
            var records = JsonConvert.DeserializeObject<Records<T>>(asset.text)!.records;
            var errors = records.Select(r => r.LoadAndValidate()).ToArray();
            if (!errors.SelectMany(e => e).Any()) {
                return records;
            }

            foreach (var errorList in errors) {
                if (errorList.Count == 0) {
                    continue;
                }

                var index = errors.IndexOf(errorList);
                Debug.LogError($"{typeof(T)} index {index} contains following errors:\n" + string.Join("\n", errorList));
            }

            throw new Exception($"Could not load asset \"{asset.name}\" due to aforementioned errors");
        }

        [Serializable]
        private class Records<T> {
            public T[] records;
        }
    }
}