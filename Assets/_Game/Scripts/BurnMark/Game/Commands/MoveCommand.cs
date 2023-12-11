using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.ModelV4;
using LiteNetLib.Utils;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Commands {
    public class MoveCommand : GameCommand {
        public int EntityId;
        public Vector2Int Position;

        protected override void SerializeContents(NetDataWriter writer) {
            writer.Put(EntityId);
            writer.Put(Position.x);
            writer.Put(Position.y);
        }

        protected override void DeserializeContents(NetDataReader reader) {
            EntityId = reader.GetInt();
            Position = new Vector2Int(reader.GetInt(), reader.GetInt());
        }

        protected override void PerformDoOnAPI(GameDataAPI api) {
            api.GetModifiableEntity(EntityId)
                !.GetModifiableComponent<PositionData>()
                !.Data = new PositionData { Position = Position };
        }
    }
}