using System;
using LiteNetLib.Utils;

namespace _Game.Scripts {
    public static class Extensions {
        #region INetSerializable

        public static TNetSerializable Copy<TNetSerializable>(this TNetSerializable netSerializable)
            where TNetSerializable : INetSerializable, new() {
            var writer = new NetDataWriter();
            writer.Put(netSerializable);

            var reader = new NetDataReader(writer);
            return reader.Get<TNetSerializable>();
        }

        #endregion

        #region NetDataWriter
        
        public static void PutNullable<T>(this NetDataWriter writer, T nullable)
            where T : INetSerializable, new() {
            var notNull = nullable != null;
            writer.Put(notNull);
            if (nullable != null)
                writer.Put(nullable);
        }

        public static void PutArray<T>(this NetDataWriter writer, T[] array)
            where T : INetSerializable, new() {
            writer.Put(array.Length);
            foreach (var item in array) {
                writer.Put(item);
            }
        }
        
        
        public static void PutEnumArray<T>(this NetDataWriter writer, T[] array)
            where T : struct, Enum {
            writer.Put(array.Length);
            foreach (var item in array) {
                writer.Put(item.ToString());
            }
        }

        #endregion

        #region NetDataReader

        public static T GetNullable<T>(this NetDataReader reader)
            where T : INetSerializable, new() {
            var notNull = reader.GetBool();
            return notNull
                ? reader.Get<T>()
                : default;
        }

        public static T[] GetArray<T>(this NetDataReader reader)
            where T : INetSerializable, new() {
            var array = new T[reader.GetInt()];
            for (var i = 0; i < array.Length; i++) {
                array[i] = reader.Get<T>();
            }

            return array;
        }

        public static T[] GetEnumArray<T>(this NetDataReader reader)
            where T : struct, Enum {
            var array = new T[reader.GetInt()];
            for (var i = 0; i < array.Length; i++) {
                array[i] = (T) Enum.Parse(typeof(T), reader.GetString());
            }

            return array;
        }

        #endregion
    }
}
