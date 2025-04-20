using System;
using Unity.Netcode;

namespace GamesKeystoneFramework.MultiPlaySystem
{
    public struct MultiPlayData : INetworkSerializable, IEquatable<MultiPlayData>
    {
        public DataType dataType;
        public string serializedValue;

        public object Value
        {
            get
            {
                object obj = dataType switch
                {
                    DataType.Integer => int.TryParse(serializedValue, out var intValue) ? intValue : 0,
                    DataType.Float => float.TryParse(serializedValue, out var floatValue) ? floatValue : 0,
                    DataType.String => serializedValue,
                    DataType.Bool => bool.TryParse(serializedValue, out var boolValue) ? boolValue : false,
                    _ => null
                };
                return obj;
            }
            set
            {
                serializedValue = value.ToString();
                dataType = value switch
                {
                    int => DataType.Integer,
                    float => DataType.Float,
                    string => DataType.String,
                    bool => DataType.Bool,
                    _ => throw new InvalidOperationException($"未対応の型　: {value.GetType()}")
                };
            }
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref dataType);
            serializer.SerializeValue(ref serializedValue);
        }

        public bool Equals(MultiPlayData other)
        {
            return dataType == other.dataType && serializedValue == other.serializedValue;
        }

        public override bool Equals(object obj)
        {
            return obj is MultiPlayData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)dataType, serializedValue);
        }
    }

    public enum DataType
    {
        Integer,
        Float,
        Bool,
        String,
    }

    struct MyStruct
    {
        public int a;
        public string b; 
        public char c;
        public float d;
        public bool e;
    }
}