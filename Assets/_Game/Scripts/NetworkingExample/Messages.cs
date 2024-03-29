﻿using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Network;
using LiteNetLib.Utils;

namespace _Game.Scripts.NetworkingExample {
    public class ChatMessage : Message {
        public string Text;

        public override void SerializeContents(NetDataWriter writer) {
            writer.Put(Text);
        }

        public override void DeserializeContents(NetDataReader reader) {
            Text = reader.GetString();
        }
    }

    public class InitialInfoRequestMessage : EmptyMessage {}

    public class InitialInfoMessage : Message {
        public int Seed;
        public List<int> List;

        public override void SerializeContents(NetDataWriter writer) {
            writer.Put(Seed);
            writer.PutArray(List.ToArray());
        }

        public override void DeserializeContents(NetDataReader reader) {
            Seed = reader.GetInt();
            List = reader.GetIntArray().ToList();
        }
    }
}
