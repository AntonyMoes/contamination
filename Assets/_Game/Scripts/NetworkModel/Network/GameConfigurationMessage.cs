using _Game.Scripts.Network;
using _Game.Scripts.NetworkModel.Commands;
using LiteNetLib.Utils;

namespace _Game.Scripts.NetworkModel.Network {
    public class GameConfigurationMessage : Message {
        public GameCommand InitialCommand;
        public int[] Players;
        public string[] PlayerNames;
        public int CurrenUser;
        
        public override void SerializeContents(NetDataWriter writer) {
            InitialCommand.Serialize(writer);
            writer.PutArray(Players);
            writer.PutArray(PlayerNames);
            writer.Put(CurrenUser);
        }

        public override void DeserializeContents(NetDataReader reader) {
            InitialCommand = GameCommand.Deserialize(reader);
            Players = reader.GetIntArray();
            PlayerNames = reader.GetStringArray();
            CurrenUser = reader.GetInt();
        }
    }
}
