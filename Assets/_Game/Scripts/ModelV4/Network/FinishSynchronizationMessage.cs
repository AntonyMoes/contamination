﻿using _Game.Scripts.Network;
using LiteNetLib.Utils;

namespace _Game.Scripts.ModelV4.Network {
    public class FinishSynchronizationMessage : Message {
        public string Guid;
        public int UserId;

        public override void Serialize(NetDataWriter writer) {
            writer.Put(Guid);
            writer.Put(UserId);
        }

        public override void Deserialize(NetDataReader reader) {
            Guid = reader.GetString();
            UserId = reader.GetInt();
        }
    }
}