using _Game.Scripts.Network;
using LiteNetLib.Utils;

namespace _Game.Scripts.ModelV4.Network {
    public class GameConfigurationMessage : Message {
        public GameCommand InitialCommand;
        public int[] UserSequence;
        public string[] UserNames;
        public int CurrenUser;
        
        public override void SerializeContents(NetDataWriter writer) {
            writer.Put(InitialCommand.Serialize());
            writer.PutArray(UserSequence);
            writer.PutArray(UserNames);
            writer.Put(CurrenUser);
        }

        public override void DeserializeContents(NetDataReader reader) {
            InitialCommand = GameCommand.Deserialize(reader.GetString());
            UserSequence = reader.GetIntArray();
            UserNames = reader.GetStringArray();
            CurrenUser = reader.GetInt();
        }
    }
}
