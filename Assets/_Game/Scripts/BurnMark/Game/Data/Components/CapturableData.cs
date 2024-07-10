using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.BurnMark.Game.Data.Components {
    public struct CapturableData : ISame<CapturableData> {
        public bool CanBeCaptured;

        public bool IsSame(CapturableData other) {
            return CanBeCaptured == other.CanBeCaptured;
        }

        public static CapturableData Default => new CapturableData { CanBeCaptured = true };
    }
}