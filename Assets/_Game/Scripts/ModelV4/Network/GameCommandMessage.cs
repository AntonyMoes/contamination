using System;
using _Game.Scripts.Network;
using LiteNetLib.Utils;

namespace _Game.Scripts.ModelV4.Network {
    public class GameCommandMessage : Message {
        public string Guid;
        public int UserId;
        public GameCommand Command;

        public override void SerializeContents(NetDataWriter writer) {
            writer.Put(UserId);
            writer.Put(Command.Serialize());
        }

        public override void DeserializeContents(NetDataReader reader) {
            UserId = reader.GetInt();
            Command = GameCommand.Deserialize(reader.GetString());
        }
    }
}
