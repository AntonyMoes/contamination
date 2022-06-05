using System;
using _Game.Scripts.ModelV4;
using LiteNetLib.Utils;

namespace _Game.Scripts {
    public static class Extensions {
        #region INetSerializable

        public static TNetSerializable Copy<TNetSerializable>(this TNetSerializable netSerializable)
            where TNetSerializable : INetSerializable, new() {
            var writer = new NetDataWriter();
            writer.Put(netSerializable);

            var reader = new NetDataReader(writer);
            return reader.Get<TNetSerializable>();
        }

        #endregion

        #region NetDataWriter

        public static void Put(this NetDataWriter writer, GameCommand command) {
            writer.Put(command.GetType().AssemblyQualifiedName);
            writer.Put(command.SerializeContents());
        }

        #endregion

        #region NetDataReader

        public static GameCommand GetCommand(this NetDataReader reader) {
            var typeName = reader.GetString();
            var type = Type.GetType(typeName);
            var command = (GameCommand) Activator.CreateInstance(type);
            command.DeserializeContents(reader.GetString());
            return command;
        }

        #endregion
    }
}
