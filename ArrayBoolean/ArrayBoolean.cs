using System;
using System.Collections;

namespace ModiferTypes
{
    public class ArrayBoolean : IEnumerable, ICloneable, IEquatable<ArrayBoolean>
    {
        private ArrayBoolean()
        {

        }
        public ArrayBoolean(bool[] array)
        {
            ConvertArrayToBytes(array);
            Count = array.Length;
        }
        public ArrayBoolean(byte[] bits)
        {
            value = bits;
            Count = bits.Length * 8;
        }

        private const byte OFFSET_BIT = 128;

        private const byte SIZE_BYTE = 8;

        private byte[] value;

        public byte[] GetByte => value;

        public int Count { get; private set; }

        public bool this[int index]
        {
            get
            {
                CheckRangeArray(index);
                return GetBoolAt(index);
            }
            set
            {
                CheckRangeArray(index);
                if (GetBoolAt(index) == value)
                    return;
                InvertBitAt(index);
            }
        }

        private void CheckRangeArray(int index)
        {
            if (Count - 1 < index || 0 > index)
                throw new ArgumentOutOfRangeException("index of class ArrayBoolean");
        }
        
        private int GetIndexArrayValue(int index) => index / SIZE_BYTE;

        private int GetIndexBitValue(int index) => index % SIZE_BYTE;

        private byte[] ConvertArrayToBytes(bool[] array)
        {
            value = new byte[GetIndexArrayValue(array.Length) + 1];
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i])
                {
                    InvertBitAt(i);
                }
            }
            return value;
        }

        private void InvertBitAt(int index)
        {
            int indexArray = GetIndexArrayValue(index);
            int indexBit = GetIndexBitValue(index);
            value[indexArray] = InvertBit(value[indexArray], indexBit);
        }

        private byte InvertBit(byte _byte, int indexBit)
        {
            return (byte)(_byte ^ (OFFSET_BIT >> indexBit));
        }
        private bool GetBoolAt(int index)
        {
            int indexArray = GetIndexArrayValue(index);
            int indexBit = GetIndexBitValue(index);
            return GetBoolAtBit(value[indexArray], indexBit);
        }
        private bool GetBoolAtBit(byte _byte, int indexBit)
        {
            int mask = (OFFSET_BIT >> indexBit);
            return ((_byte & mask) == mask);
        }
        private bool[] ConvertBytesToArray(byte[] bits)
        {
            bool[] array = new bool[Count];
            for(int i = 0; i < Count; i++)
            {
                array[i] = GetBoolAt(i);
            }
            return array;
        }

        public IEnumerator GetEnumerator()
        {
            return ConvertBytesToArray(value).GetEnumerator();
        }

        public object Clone()
        {
            return new ArrayBoolean()
            {
                value = this.value,
                Count = this.Count
            };
        }

        public bool Equals(ArrayBoolean other)
        {
            if(value.Length != other.value.Length)
                return false;
            for(int i = 0; i < value.Length; i++)
            {
                if(value[i] != other.value[i])
                    return false;
            }
            return true;
        }

        public static implicit operator ArrayBoolean(bool[] array)
        {
            return new ArrayBoolean(array);
        }

        public static implicit operator bool[](ArrayBoolean array)
        {
            return array.ConvertBytesToArray(array.value);
        }

        public static explicit operator ArrayBoolean(byte[] array)
        {
            return new ArrayBoolean(array);
        }

    }
}
