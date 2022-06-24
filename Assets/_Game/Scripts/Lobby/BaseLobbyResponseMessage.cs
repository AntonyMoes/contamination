using _Game.Scripts.Network;
using LiteNetLib.Utils;

namespace _Game.Scripts.Lobby {
    public class BaseLobbyResponseMessage<TRoomSettings> : Message where TRoomSettings : IRoomSettings<TRoomSettings>, new() {
        public Lobby<TRoomSettings>.Data LobbyData;

        public override void SerializeContents(NetDataWriter writer) {
            writer.PutNullable(LobbyData);
        }

        public override void DeserializeContents(NetDataReader reader) {
            LobbyData = reader.GetNullable<Lobby<TRoomSettings>.Data>();
        }
    }
}
