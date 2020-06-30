using System;
using System.Collections.Generic;

namespace VoxCake.Networking
{
    public abstract class Packet
    {
        public byte Id { get; internal set; }
        public byte SenderId { get; internal set; }
        public int Size { get; internal set; }
        
        internal byte[] data;

        private PacketVariableGetter _variableGetter;
        private PacketVariableSetter _variableSetter;

        public abstract PacketType Type { get; }

        protected virtual void VariableBindings(PacketVariableBinder variableBinder)
        {
        }

        protected virtual void Execute(PacketVariableGetter variables)
        { 
        }

        protected virtual void Execute()
        {
        }

        internal void ExecuteInternal(object[] data)
        {
            if (data.Length > 0)
            {
                _variableSetter.SetVariables(data);

                Execute(_variableGetter);

                _variableGetter.ResetIndex();
            }
            else
            {
                Execute();
            }
        }

        internal void Initialize(byte packetId)
        {
            Id = packetId;

            var variableIndexCollection = new List<int>();
            var variableTypeCollection = new List<Type>();

            VariableBindings(new PacketVariableBinder(this, variableIndexCollection, variableTypeCollection));
            PostBinding();

            _variableGetter = new PacketVariableGetter(this, variableIndexCollection, variableTypeCollection);
            _variableSetter = new PacketVariableSetter(this, variableIndexCollection, variableTypeCollection);
        }

        private void PostBinding()
        {
            data = new byte[Size];
            data[0] = Id;
        }

        internal void SetSenderId(byte senderId)
        {
            SenderId = senderId;
        }

        public override int GetHashCode()
        {
            return Size + _variableGetter.GetHashCode();
        }
    }
}