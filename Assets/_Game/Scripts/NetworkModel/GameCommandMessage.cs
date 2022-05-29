using System;
using _Game.Scripts.ModelV3;
using _Game.Scripts.Network;
using LiteNetLib.Utils;

namespace _Game.Scripts.NetworkModel {
    public class GameCommandMessage : Message {
        public GameCommand Command;

        public override void Serialize(NetDataWriter writer) {
            writer.Put(Command.GetType().AssemblyQualifiedName);
            // writer.Put(Command);  TODO
        }

        public override void Deserialize(NetDataReader reader) {
            var typeName = reader.GetString();
            var type = Type.GetType(typeName);
            Command = (GameCommand) Activator.CreateInstance(type);
            // Command.Deserialize(reader);  TODO
        }
    }
}
