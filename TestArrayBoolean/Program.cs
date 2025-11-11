using ArrayBooleanTests;

Console.WriteLine("=== ArrayBoolean Test ===\n");

Test.TestConstructorWithBoolArray();
Test.TestConstructorWithByteArray();
Test.TestIndexerGetSet();
Test.TestCount();
Test.TestVLQEncoding();
Test.TestClone();
Test.TestEquals();
Test.TestEnumerator();
Test.TestImplicitOperators();
Test.TestExplicitOperator();
Test.TestEdgeCases();

Console.WriteLine("\n=== All Test Passed ===");

