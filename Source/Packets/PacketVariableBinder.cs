using System;
using System.Collections.Generic;
using VoxCake.Common;

namespace VoxCake.Networking
{
    public class PacketVariableBinder
    {
        private const string Byte = "Byte";
        private const string Short = "Int16";
        private const string Int = "Int32";
        private const string Long = "Int64";
        private const string Float = "Single";
        private const string Double = "Double";
        private const string Vector2 = "Vector2";
        private const string Vector3 = "Vector3";

        private const int HeaderSize = 1;

        private readonly Packet _packet;
        private readonly List<int> _variableIndexCollection;
        private readonly List<Type> _variableTypeCollection;

        internal PacketVariableBinder(Packet packet, List<int> variableIndexCollection, List<Type> variableTypeCollection)
        {
            _packet = packet;
            _variableIndexCollection = variableIndexCollection;
            _variableTypeCollection = variableTypeCollection;

            packet.Size += HeaderSize;
        }
        
        public void Bind<T>()
        {
            var variableType = typeof(T);

            _variableTypeCollection.Add(variableType);
            _variableIndexCollection.Add(_packet.Size);

            var variableName = variableType.Name;
            switch (variableName)
            {
                case Byte:
                    _packet.Size += 1;
                    break;
                case Short:
                    _packet.Size += 2;
                    break;
                case Int:
                    _packet.Size += 4;
                    break;
                case Long:
                    _packet.Size += 8;
                    break;
                case Float:
                    _packet.Size += 4;
                    break;
                case Double:
                    _packet.Size += 8;
                    break;
                case Vector2:
                    _packet.Size += 8;
                    break;
                case Vector3:
                    _packet.Size += 12;
                    break;
                default:
                    VoxCakeDebugger.LogError($"Trying to Get unknown type \"{variableName}\"");
                    break;
            }
        }
    }
}