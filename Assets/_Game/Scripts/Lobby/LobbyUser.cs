using _Game.Scripts.Network;
using LiteNetLib.Utils;

namespace _Game.Scripts.Lobby {
    public class LobbyUser {
        public readonly string Id;
        public readonly IPeer Peer;
        public readonly string Name;

        public LobbyUser(string id, IPeer peer, string name) {
            Id = id;
            Peer = peer;
            Name = name;
        }

        public Data GetData() {
            return new Data {
                Id = Id,
                Name = Name
            };
        }

        public class Data : INetSerializable {
            public string Id;
            public string Name;

            public void Serialize(NetDataWriter writer) {
                writer.Put(Id);
                writer.Put(Name);
            }

            public void Deserialize(NetDataReader reader) {
                Id = reader.GetString();
                Name = reader.GetString();
            }
        }
    }
}
