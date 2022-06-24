using System;
using _Game.Scripts.Network;
using LiteNetLib.Utils;

namespace _Game.Scripts.Lobby {
    public class GameStartResponseMessage : Message {
        public ERefuseReason RefuseReason;

        public override void SerializeContents(NetDataWriter writer) {
            writer.Put(RefuseReason.ToString());
        }

        public override void DeserializeContents(NetDataReader reader) {
            Enum.TryParse(reader.GetString(), out RefuseReason);
        }

        public enum ERefuseReason {
            None,
            NotAuthorized,
            NoRoom,
            NotAllowed
        }
    }
}
