using LiteNetLib.Utils;

namespace _Game.Scripts {
    public static class Extensions {
        public static TNetSerializable Copy<TNetSerializable>(this TNetSerializable netSerializable)
            where TNetSerializable : INetSerializable, new() {
            var writer = new NetDataWriter();
            writer.Put(netSerializable);

            var reader = new NetDataReader(writer);
            return reader.Get<TNetSerializable>();
        }
    }
}
