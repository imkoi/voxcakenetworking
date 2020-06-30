using System;
using System.Collections.Generic;
using VoxCake.Common;
using VoxCake.Networking.Common.Serialization;

namespace VoxCake.Networking
{
    public class PacketVariableGetter
    {
        private const string Byte = "Byte";
        private const string Short = "Int16";
        private const string Int = "Int32";
        private const string Long = "Int64";
        private const string Float = "Single";
        private const string Double = "Double";
        private const string Vector2 = "Vector2";
        private const string Vector3 = "Vector3";

        private readonly Packet _packet;
        private readonly List<int> _variableIndexCollection;
        private readonly List<Type> _variableTypeCollection;

        private int _getterIndex;

        public PacketVariableGetter(Packet packet, List<int> variableIndexCollection, List<Type> variableTypeCollection)
        {
            _packet = packet;
            _variableIndexCollection = variableIndexCollection;
            _variableTypeCollection = variableTypeCollection;
        }
        public T Get<T>()
        {
            var variableType = typeof(T);
            var index = _variableIndexCollection[_getterIndex];

            object value = null;
            var variableName = variableType.Name;
            switch (variableName)
            {
                case Byte:
                    value = PacketDeserializer.DeserializeByte(_packet.data, index);
                    break;
                case Short:
                    value = PacketDeserializer.DeserializeShort(_packet.data, index);
                    break;
                case Int:
                    value = PacketDeserializer.DeserializeInt(_packet.data, index);
                    break;
                case Long:
                    value = PacketDeserializer.DeserializeLong(_packet.data, index);
                    break;
                case Float:
                    value = PacketDeserializer.DeserializeFloat(_packet.data, index);
                    break;
                case Double:
                    value = PacketDeserializer.DeserializeDouble(_packet.data, index);
                    break;
                case Vector2:
                    value = PacketDeserializer.DeserializeVector2(_packet.data, index);
                    break;
                case Vector3:
                    value = PacketDeserializer.DeserializeVector3(_packet.data, index);
                    break;
                default:
                    VoxCakeDebugger.LogError($"Trying to Get unknown type \"{variableName}\"");
                    break;
            }

            _getterIndex++;

            return (T)value;
        }

        internal void ResetIndex()
        {
            _getterIndex = 0;
        }

        public override int GetHashCode()
        {
            var hash = 0;

            var count = _variableTypeCollection.Count;
            for(var i = 0; i < count; i++)
            {
                var variable = _variableTypeCollection[i];
                switch (variable.Name)
                {
                    case Byte:
                        hash += 1;
                        break;
                    case Short:
                        hash += 2;
                        break;
                    case Int:
                        hash += 4;
                        break;
                    case Float:
                        hash += 8;
                        break;
                    case Long:
                        hash += 16;
                        break;
                    case Double:
                        hash += 32;
                        break;
                    case Vector2:
                        hash += 64;
                        break;
                    case Vector3:
                        hash += 256;
                        break;
                }
            }

            return hash;
        }
    }
}