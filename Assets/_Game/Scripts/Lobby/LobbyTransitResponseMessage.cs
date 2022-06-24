using System;
using _Game.Scripts.Network;
using LiteNetLib.Utils;

namespace _Game.Scripts.Lobby {
    public class LobbyTransitResponseMessage<TRoomSettings> : BaseLobbyResponseMessage<TRoomSettings>
        where TRoomSettings : IRoomSettings<TRoomSettings>, new() {
        public ERefuseReason RefuseReason;
        public string UserId;

        public override void SerializeContents(NetDataWriter writer) {
            base.SerializeContents(writer);
            writer.Put(RefuseReason.ToString());
            writer.Put(UserId);
        }

        public override void DeserializeContents(NetDataReader reader) {
            base.DeserializeContents(reader);
            Enum.TryParse(reader.GetString(), out RefuseReason);
            UserId = reader.GetString();
        }

        public enum ERefuseReason {
            None,
            NameTaken
        }
    }
}
