using System;
using _Game.Scripts.Network;
using LiteNetLib.Utils;

namespace _Game.Scripts.Lobby {
    public class RoomTransitResponseMessage<TRoomSettings> : BaseLobbyResponseMessage<TRoomSettings>
        where TRoomSettings : IRoomSettings<TRoomSettings>, new() {
        public ERefuseReason RefuseReason;

        public override void SerializeContents(NetDataWriter writer) {
            base.SerializeContents(writer);
            writer.Put(RefuseReason.ToString());
        }

        public override void DeserializeContents(NetDataReader reader) {
            base.DeserializeContents(reader);
            Enum.TryParse(reader.GetString(), out RefuseReason);
        }

        public enum ERefuseReason {
            None,
            NoRoom,
            NotAuthorized,
            NotAllowed,
            WrongPassword
        }
    }
}
