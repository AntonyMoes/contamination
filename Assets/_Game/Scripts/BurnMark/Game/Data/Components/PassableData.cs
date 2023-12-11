using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.BurnMark.Game.Data.Components {
    public struct PassableData : ISame<PassableData> {
        public PassableStatus Status;

        public bool IsSame(PassableData other) {
            return Status == other.Status;
        }

        public enum PassableStatus {
            // Passable,
            // PassableToAllies,
            // PassableToAlliesNotStoppable,
            // NotPassable
        }
    }
}