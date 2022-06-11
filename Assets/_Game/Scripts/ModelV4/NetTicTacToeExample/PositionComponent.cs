using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.ModelV4.NetTicTacToeExample {
    public class PositionComponent : Component<PositionComponent.PositionData> {
        public struct PositionData : ISame<PositionData> {
            public int Row;
            public int Column;
            
            public bool IsSame(ISame<PositionData> other) {
                var o = other.Get();
                return Row == o.Row && Column == o.Column;
            }
        }

        public PositionComponent(Entity correspondingEntity) : base(correspondingEntity) { }
    }
}
