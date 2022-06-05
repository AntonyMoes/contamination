using _Game.Scripts.Network;
using LiteNetLib.Utils;

namespace _Game.Scripts.ModelV4.Network {
    public class GameConfigurationMessage : Message {
        public GameCommand InitialCommand;
        public int[] UserSequence;
        public string[] UserNames;
        public int CurrenUser;
        
        public override void Serialize(NetDataWriter writer) {
            writer.Put(InitialCommand);
            writer.PutArray(UserSequence);
            writer.PutArray(UserNames);
            writer.Put(CurrenUser);
        }

        public override void Deserialize(NetDataReader reader) {
            InitialCommand = reader.GetCommand();
            UserSequence = reader.GetIntArray();
            UserNames = reader.GetStringArray();
            CurrenUser = reader.GetInt();
        }
    }
}
