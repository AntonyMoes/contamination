using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;

namespace _Game.Scripts.Data {
    [Serializable]
    public class EmployeeData : IData {
        [JsonProperty] private string name;
        [JsonProperty] private string department;
        [JsonProperty] private int sanity;
        [JsonProperty] private string[] skills;
        [JsonProperty] private int moveForward;
        [JsonProperty] private int moveBackward;

        public string Name => name;
        public EDepartment Department { get; private set; }
        public int Sanity => sanity;
        public SkillData[] Skills { get; private set; }
        public int MoveForward => moveForward;
        public int MoveBackward => moveBackward;

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

            validationErrors.AddRange(LoadAndValidateSkills(skills, out var loadedSkills));
            Skills = loadedSkills;

            if (moveForward < 0) {
                validationErrors.Add($"MoveForward can't be < 0: {moveForward}");
            }

            if (moveBackward < 0) {
                validationErrors.Add($"MoveBackward can't be < 0: {moveBackward}");
            }

            return validationErrors;
        }

        private List<string> LoadAndValidateSkills(string[] skillNames, out SkillData[] loadedSkills) {
            skillNames ??= Array.Empty<string>();
            if (skillNames.Length == 0) {
                loadedSkills = Array.Empty<SkillData>();
                return new List<string> { "An employee can't have no skills" };
            }

            var validationErrors = new List<string>();
            var skills = new List<SkillData>();
            foreach (var skillName in skillNames) {
                var skill = DataStorage.Instance.Skills.FirstOrDefault(skill => skill.Name == skillName);
                if (skill == null) {
                    validationErrors.Add($"Could not find skill with name \"{skillName}\"");
                    continue;
                }

                skills.Add(skill);
            }

            loadedSkills = skills.ToArray();

            var skillErrors = loadedSkills
                .Select(skill => skill.LoadAndValidate())
                .Where(errors => errors.Count != 0)
                .Select(errors => "Could not validate skill:\n" + string.Join("\n", errors.Select(e => $"\t* {e}")))
                .ToList();
            validationErrors.AddRange(skillErrors);
            return validationErrors;
        }
    }
}