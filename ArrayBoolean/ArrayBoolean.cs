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
            Count = array.Length;
            value = EncodeToBytes(array);
        }

        public ArrayBoolean(byte[] encodedData)
        {
            var (count, bytesRead) = DecodeVLQ(encodedData, 0);
            Count = count;
            value = encodedData;
        }

        private const byte OFFSET_BIT = 128;
        private const byte SIZE_BYTE = 8;

        private byte[] value; // Всегда содержит: [VLQ-длина][данные]

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

        private int GetDataOffset()
        {
            var (_, bytesRead) = DecodeVLQ(value, 0);
            return bytesRead;
        }

        private byte[] EncodeToBytes(bool[] array)
        {
            byte[] countBytes = EncodeVLQ(array.Length);
            int dataBytesLength = GetIndexArrayValue(array.Length) + 1;
            byte[] result = new byte[countBytes.Length + dataBytesLength];

            Array.Copy(countBytes, 0, result, 0, countBytes.Length);

            int offset = countBytes.Length;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i])
                {
                    int indexArray = GetIndexArrayValue(i);
                    int indexBit = GetIndexBitValue(i);
                    result[offset + indexArray] = InvertBit(result[offset + indexArray], indexBit);
                }
            }

            return result;
        }

        private void InvertBitAt(int index)
        {
            int offset = GetDataOffset();
            int indexArray = GetIndexArrayValue(index);
            int indexBit = GetIndexBitValue(index);
            value[offset + indexArray] = InvertBit(value[offset + indexArray], indexBit);
        }

        private byte InvertBit(byte _byte, int indexBit)
        {
            return (byte)(_byte ^ (OFFSET_BIT >> indexBit));
        }

        private bool GetBoolAt(int index)
        {
            int offset = GetDataOffset();
            int indexArray = GetIndexArrayValue(index);
            int indexBit = GetIndexBitValue(index);
            return GetBoolAtBit(value[offset + indexArray], indexBit);
        }

        private bool GetBoolAtBit(byte _byte, int indexBit)
        {
            int mask = (OFFSET_BIT >> indexBit);
            return ((_byte & mask) == mask);
        }

        private bool[] ConvertToArray()
        {
            bool[] array = new bool[Count];
            for (int i = 0; i < Count; i++)
            {
                array[i] = GetBoolAt(i);
            }
            return array;
        }

        private static byte[] EncodeVLQ(int count)
        {
            if (count == 0)
                return new byte[] { 0 };

            var bytes = new System.Collections.Generic.List<byte>();

            while (count > 0)
            {
                byte b = (byte)(count & 0x7F);
                count >>= 7;

                if (count > 0)
                    b |= 0x80;

                bytes.Add(b);
            }

            return bytes.ToArray();
        }

        private static (int count, int bytesRead) DecodeVLQ(byte[] data, int startIndex = 0)
        {
            int result = 0;
            int shift = 0;
            int bytesRead = 0;

            while (true)
            {
                if (startIndex + bytesRead >= data.Length)
                    throw new ArgumentException("Invalid VLQ encoding");

                byte b = data[startIndex + bytesRead];
                bytesRead++;

                result |= (b & 0x7F) << shift;

                if ((b & 0x80) == 0)
                    break;

                shift += 7;
            }

            return (result, bytesRead);
        }

        public IEnumerator GetEnumerator()
        {
            return ConvertToArray().GetEnumerator();
        }

        public object Clone()
        {
            byte[] clonedValue = new byte[value.Length];
            Array.Copy(value, clonedValue, value.Length);

            return new ArrayBoolean()
            {
                value = clonedValue,
                Count = this.Count
            };
        }

        public bool Equals(ArrayBoolean other)
        {
            if (other == null || value.Length != other.value.Length)
                return false;

            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] != other.value[i])
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
            return array.ConvertToArray();
        }

        public static explicit operator ArrayBoolean(byte[] array)
        {
            return new ArrayBoolean(array);
        }
    }
}