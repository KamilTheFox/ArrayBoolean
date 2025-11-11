using ModiferTypes;

namespace ArrayBooleanTests
{
    class Test
    {
        public static void TestConstructorWithBoolArray()
        {
            Console.WriteLine("Test: Constructor with bool[]");
            var bools = new bool[] { true, false, true, true, false };
            var arr = new ArrayBoolean(bools);

            for (int i = 0; i < bools.Length; i++)
            {
                Assert(arr[i] == bools[i], $"Index {i} mismatch");
            }
            Assert(arr.Count == bools.Length, "Count mismatch");
            Console.WriteLine("✓ Passed\n");
        }

        public static void TestConstructorWithByteArray()
        {
            Console.WriteLine("Test: Constructor with byte[] (VLQ-encoded)");
            var original = new ArrayBoolean(new bool[] { true, false, true });
            var encoded = original.GetByte;
            var restored = new ArrayBoolean(encoded);

            Assert(restored.Count == original.Count, "Count mismatch");
            for (int i = 0; i < original.Count; i++)
            {
                Assert(restored[i] == original[i], $"Index {i} mismatch");
            }
            Console.WriteLine("✓ Passed\n");
        }

        public static void TestIndexerGetSet()
        {
            Console.WriteLine("Test: Indexer get/set");
            var arr = new ArrayBoolean(new bool[10]);

            arr[0] = true;
            arr[5] = true;
            arr[9] = true;

            Assert(arr[0] == true, "Index 0 should be true");
            Assert(arr[1] == false, "Index 1 should be false");
            Assert(arr[5] == true, "Index 5 should be true");
            Assert(arr[9] == true, "Index 9 should be true");

            arr[5] = false;
            Assert(arr[5] == false, "Index 5 should be false after set");
            Console.WriteLine("✓ Passed\n");
        }

        public static void TestCount()
        {
            Console.WriteLine("Test: Count property");
            var arr1 = new ArrayBoolean(new bool[0]);
            Assert(arr1.Count == 0, "Empty array count should be 0");

            var arr2 = new ArrayBoolean(new bool[100]);
            Assert(arr2.Count == 100, "Count should be 100");

            var arr3 = new ArrayBoolean(new bool[1000]);
            Assert(arr3.Count == 1000, "Count should be 1000");
            Console.WriteLine("✓ Passed\n");
        }

        public static void TestVLQEncoding()
        {
            Console.WriteLine("Test: VLQ encoding for different sizes");

            // Маленькое число (< 128) - 1 байт VLQ
            var arr1 = new ArrayBoolean(new bool[10]);
            var bytes1 = arr1.GetByte;
            Assert(bytes1[0] == 10 && (bytes1[0] & 0x80) == 0, "VLQ for 10 should be 1 byte");

            // Число >= 128 - несколько байт VLQ
            var arr2 = new ArrayBoolean(new bool[200]);
            var bytes2 = arr2.GetByte;
            Assert((bytes2[0] & 0x80) != 0, "VLQ for 200 should have continuation bit");

            // Большое число
            var arr3 = new ArrayBoolean(new bool[16384]); // 2^14
            var bytes3 = arr3.GetByte;
            var restored = new ArrayBoolean(bytes3);
            Assert(restored.Count == 16384, "Large count should be preserved");

            Console.WriteLine("✓ Passed\n");
        }

        public static void TestClone()
        {
            Console.WriteLine("Test: Clone method");
            var original = new ArrayBoolean(new bool[] { true, false, true, true });
            var clone = (ArrayBoolean)original.Clone();

            Assert(clone.Count == original.Count, "Clone count mismatch");
            for (int i = 0; i < original.Count; i++)
            {
                Assert(clone[i] == original[i], $"Clone index {i} mismatch");
            }

            // Проверка независимости
            clone[0] = false;
            Assert(original[0] == true, "Original should not change");
            Assert(clone[0] == false, "Clone should change");
            Console.WriteLine("✓ Passed\n");
        }

        public static void TestEquals()
        {
            Console.WriteLine("Test: Equals method");
            var arr1 = new ArrayBoolean(new bool[] { true, false, true });
            var arr2 = new ArrayBoolean(new bool[] { true, false, true });
            var arr3 = new ArrayBoolean(new bool[] { true, false, false });

            Assert(arr1.Equals(arr2), "Equal arrays should return true");
            Assert(!arr1.Equals(arr3), "Different arrays should return false");
            Assert(!arr1.Equals(null), "Should not equal null");
            Console.WriteLine("✓ Passed\n");
        }

        public static void TestEnumerator()
        {
            Console.WriteLine("Test: IEnumerable (GetEnumerator)");
            var bools = new bool[] { true, false, true, true, false };
            var arr = new ArrayBoolean(bools);

            int index = 0;
            foreach (bool value in arr)
            {
                Assert(value == bools[index], $"Enumerator index {index} mismatch");
                index++;
            }
            Assert(index == bools.Length, "Enumerator did not iterate all elements");
            Console.WriteLine("✓ Passed\n");
        }

        public static void TestImplicitOperators()
        {
            Console.WriteLine("Test: Implicit operators");

            // bool[] -> ArrayBoolean
            bool[] bools = new bool[] { true, false, true };
            ArrayBoolean arr = bools;
            Assert(arr.Count == 3, "Implicit conversion count mismatch");
            Assert(arr[0] == true, "Implicit conversion value mismatch");

            // ArrayBoolean -> bool[]
            bool[] restored = arr;
            Assert(restored.Length == 3, "Implicit back-conversion length mismatch");
            for (int i = 0; i < bools.Length; i++)
            {
                Assert(restored[i] == bools[i], $"Implicit back-conversion index {i} mismatch");
            }
            Console.WriteLine("✓ Passed\n");
        }

        public static void TestExplicitOperator()
        {
            Console.WriteLine("Test: Explicit operator byte[] -> ArrayBoolean");
            var original = new ArrayBoolean(new bool[] { true, false, true });
            byte[] encoded = original.GetByte;

            ArrayBoolean restored = (ArrayBoolean)encoded;
            Assert(restored.Count == original.Count, "Explicit conversion count mismatch");
            for (int i = 0; i < original.Count; i++)
            {
                Assert(restored[i] == original[i], $"Explicit conversion index {i} mismatch");
            }
            Console.WriteLine("✓ Passed\n");
        }

        public static void TestEdgeCases()
        {
            Console.WriteLine("Test: Edge cases");

            // Пустой массив
            var empty = new ArrayBoolean(new bool[0]);
            Assert(empty.Count == 0, "Empty array count should be 0");

            // Один элемент
            var single = new ArrayBoolean(new bool[] { true });
            Assert(single.Count == 1 && single[0] == true, "Single element array");

            // Ровно 8 элементов (1 байт данных)
            var eightBits = new ArrayBoolean(new bool[8]);
            eightBits[0] = true;
            eightBits[7] = true;
            Assert(eightBits[0] == true && eightBits[7] == true, "8-bit array boundary");

            // 9 элементов (2 байта данных)
            var nineBits = new ArrayBoolean(new bool[9]);
            nineBits[8] = true;
            Assert(nineBits[8] == true, "9-bit array boundary");

            // Проверка выхода за границы
            try
            {
                var arr = new ArrayBoolean(new bool[5]);
                var _ = arr[10];
                Assert(false, "Should throw ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException)
            {
                // Expected
            }

            Console.WriteLine("✓ Passed\n");
        }

        static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                throw new Exception($"Assertion failed: {message}");
            }
        }
    }
}