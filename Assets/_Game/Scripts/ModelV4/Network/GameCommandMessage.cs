using System;
using _Game.Scripts.Network;
using LiteNetLib.Utils;

namespace _Game.Scripts.ModelV4.Network {
    public class GameCommandMessage : Message {
        public string Guid;
        public int UserId;
        public GameCommand Command;

        public override void Serialize(NetDataWriter writer) {
            writer.Put(UserId);
            writer.Put(Command);
        }

        public override void Deserialize(NetDataReader reader) {
            UserId = reader.GetInt();
            Command = reader.GetCommand();
        }
    }
}
