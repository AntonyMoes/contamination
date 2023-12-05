using _Game.Scripts.Network;
using _Game.Scripts.NetworkModel.Commands;
using LiteNetLib.Utils;

namespace _Game.Scripts.NetworkModel.Network {
    public class GameCommandMessage : Message {
        public string Guid;
        public int UserId;
        public GameCommand Command;

        public override void SerializeContents(NetDataWriter writer) {
            writer.Put(UserId);
            Command.Serialize(writer);
        }

        public override void DeserializeContents(NetDataReader reader) {
            UserId = reader.GetInt();
            Command = GameCommand.Deserialize(reader);
        }
    }
}
