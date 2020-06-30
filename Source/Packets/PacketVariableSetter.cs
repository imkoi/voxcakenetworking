using System;
using System.Collections.Generic;
using VoxCake.Common;
using VoxCake.Networking.Common.Serialization;

namespace VoxCake.Networking
{
    public class PacketVariableSetter
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

		private int _setterIndex;

		internal PacketVariableSetter(Packet packet, List<int> variableIndexCollection, List<Type> variableTypeCollection)
        {
            _packet = packet;
            _variableIndexCollection = variableIndexCollection;
            _variableTypeCollection = variableTypeCollection;
        }

		public void SetVariables(object[] values)
		{
			var count = values.Length;
			for(var i = 0; i < count; i++)
			{
				SetVariable(values[i]);
			}
		}

        private void SetVariable(object value)
        {
			var index = _variableIndexCollection[_setterIndex];
			var variableType = _variableTypeCollection[_setterIndex];

			var variableName = variableType.Name;
			switch (variableName)
			{
				case Byte:
					PacketSerializer.SerializeByte(value, _packet.data, index);
					break;
				case Short:
					PacketSerializer.SerializeShort(value, _packet.data, index);
					break;
				case Int:
					PacketSerializer.SerializeInt(value, _packet.data, index);
					break;
				case Long:
					PacketSerializer.SerializeLong(value, _packet.data, index);
					break;
				case Float:
					PacketSerializer.SerializeFloat(value, _packet.data, index);
					break;
				case Double:
					PacketSerializer.SerializeDouble(value, _packet.data, index);
					break;
				case Vector2:
					PacketSerializer.SerializeVector2(value, _packet.data, index);
					break;
				case Vector3:
					PacketSerializer.SerializeVector3(value, _packet.data, index);
					break;
				default:
					VoxCakeDebugger.LogError($"Trying to Get unknown type \"{variableName}\"");
					break;
			}

			_setterIndex++;
			if (_setterIndex == _variableIndexCollection.Count)
			{
				_setterIndex = 0;
			}
		}
    }
}
