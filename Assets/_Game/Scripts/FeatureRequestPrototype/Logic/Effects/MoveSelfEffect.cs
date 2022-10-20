using _Game.Scripts.FeatureRequestPrototype.Data;
using GeneralUtils;

namespace _Game.Scripts.FeatureRequestPrototype.Logic.Effects {
    public class MoveSelfEffect : MoveEffect {
        private Employee _moveTarget;

        public MoveSelfEffect(Employee employee) : base(EffectData.CreateFakeMoveData(employee.MoveForward, employee.MoveBackward)) { }

        public void SetMoveTarget(Employee employee) {
            _moveTarget = employee;
        }

        protected override void PerformApplyTo(Rng rng, Employee employee) {
            employee.Position = _moveTarget.Position;
        }
    }
}