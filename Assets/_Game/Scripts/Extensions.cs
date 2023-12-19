using System;
using System.Collections.Generic;
using System.Linq;
using LiteNetLib.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace _Game.Scripts {
    public static class Extensions {
        #region UnityEvent

        public static void SetOnlyListener(this UnityEvent @event, Action listener) {
            @event.RemoveAllListeners();
            @event.AddListener(() => listener());
        }

        #endregion

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

        public static void Put<T>(this NetDataWriter writer, T @enum)
            where T : struct, Enum {
            writer.Put(@enum.ToString());
        }

        public static void PutEnumArray<T>(this NetDataWriter writer, T[] array)
            where T : struct, Enum {
            writer.Put(array.Length);
            foreach (var item in array) {
                writer.Put(item.ToString());
            }
        }

        public static void PutArray(this NetDataWriter writer, Color[] array) {
            writer.PutArray(array.Select(Serialize).ToArray());
        }

        public static void Put(this NetDataWriter writer, Color color) {
            writer.Put(color.Serialize());
        }

        private static string Serialize(this Color color) {
            return "#" + ColorUtility.ToHtmlStringRGBA(color);
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

        public static T GetEnum<T>(this NetDataReader reader) {
            return (T) Enum.Parse(typeof(T), reader.GetString());
        }

        public static T[] GetEnumArray<T>(this NetDataReader reader)
            where T : struct, Enum {
            var array = new T[reader.GetInt()];
            for (var i = 0; i < array.Length; i++) {
                array[i] = (T) Enum.Parse(typeof(T), reader.GetString());
            }

            return array;
        }

        public static Color GetColor(this NetDataReader reader) {
            ColorUtility.TryParseHtmlString(reader.GetString(), out var color);
            return color;
        }

        public static Color[] GetColorArray(this NetDataReader reader) {
            return reader.GetStringArray().Select(str => {
                ColorUtility.TryParseHtmlString(str, out var color);
                return color;
            }).ToArray();
        }

        #endregion

        #region Vector2Int

        public static IEnumerable<Vector2Int> EnumeratePositions(this Vector2Int vector) {
            for (var i = 0; i < vector.x; i++) {
                for (var j = 0; j < vector.y; j++) {
                    yield return new Vector2Int(i, j);
                }
            }
        }

        #endregion

        #region IReadOnlyList

        public static bool ListEquals<T>(this IReadOnlyList<T> list, IReadOnlyList<T> other) {
            var isNull = list == null;
            var otherIsNull = other == null;
            if (isNull != otherIsNull) {
                return false;
            }

            if (isNull) {
                return true;
            }

            if (list.Count != other.Count) {
                return false;
            }

            for (var i = 0; i < list.Count; i++) {
                if (!list[i].Equals(other[i])) {
                    return false;
                }
            }
            
            return true;
        }

        #endregion
    }
}
