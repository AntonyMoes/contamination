using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Data {
    [Serializable]
    public class EmployeeData : IData {
        [SerializeField] private string name;
        [SerializeField] private string department;
        [SerializeField] private int sanity;

        public string Name => name;
        public EDepartment Department { get; private set; }
        public int Sanity => sanity;

        public List<string> LoadAndValidate() {
            var validationErrors = new List<string>();

            if (!Enum.TryParse(department, out EDepartment dep)) {
                validationErrors.Add($"Could not parse department \"{department}\"!");
            } else {
                Department = dep;
            }

            if (sanity <= 0) {
                validationErrors.Add($"Sanity can't be <= 0: {sanity}");
            }

            return validationErrors;
        }
    }
}