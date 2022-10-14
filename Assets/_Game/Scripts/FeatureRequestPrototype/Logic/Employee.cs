using System.Linq;
using _Game.Scripts.Data;
using GeneralUtils;

namespace _Game.Scripts.FeatureRequestPrototype.Logic {
    public class Employee {
        public string Name => _employeeData.Name;
        public EDepartment Department => _employeeData.Department;

        public int Position => _positionProvider.Position;
        public readonly Skill[] Skills;

        public int MaxSanity => _employeeData.Sanity;
        public readonly UpdatedValue<int> Sanity = new UpdatedValue<int>();

        // public IEnumerable<Effect> appliedEffects

        private readonly EmployeeData _employeeData;
        private readonly IPositionProvider _positionProvider;

        public Employee(EmployeeData employeeData, IPositionProvider positionProvider) {
            _employeeData = employeeData;
            _positionProvider = positionProvider;

            Skills = employeeData.Skills
                .Select(skillData => new Skill(skillData))
                .ToArray();

            Sanity.Value = MaxSanity;
        }
    }
}