using _Game.Scripts.Network;
using _Game.Scripts.NetworkModel.Commands;
using LiteNetLib.Utils;

namespace _Game.Scripts.NetworkModel.Network {
    public class GameConfigurationMessage : Message {
        public GameCommand InitialCommand;
        public int[] UserSequence;
        public string[] UserNames;
        public int CurrenUser;
        
        public override void SerializeContents(NetDataWriter writer) {
            InitialCommand.Serialize(writer);
            writer.PutArray(UserSequence);
            writer.PutArray(UserNames);
            writer.Put(CurrenUser);
        }

        public override void DeserializeContents(NetDataReader reader) {
            InitialCommand = GameCommand.Deserialize(reader);
            UserSequence = reader.GetIntArray();
            UserNames = reader.GetStringArray();
            CurrenUser = reader.GetInt();
        }
    }
}
