using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.BurnMark.Game.Data.Components {
    public struct PassableData : ISame<PassableData> {
        public PassableStatus Status;

        public bool IsSame(ISame<PassableData> other) {
            var o = other.Get();
            return Status == o.Status;
        }

        public enum PassableStatus {
            Passable,
            PassableToAllies,
            PassableToAlliesNotStoppable,
            NotPassable
        }
    }
}