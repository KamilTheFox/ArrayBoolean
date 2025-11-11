# ArrayBoolean

Компактное представление массива булевых значений с эффективной сериализацией для передачи по сети.

## Зачем?

Стандартный "bool[]" занимает 1 байт на элемент. "ArrayBoolean" использует битовую упаковку (8 bool = 1 байт) + VLQ-кодирование длины, что дает:
- **8x экономия памяти** на данных
- **Корректная сериализация** без потери количества элементов при передаче по сети
- **Поддержка любых размеров** (даже больше int.MaxValue через расширение до long)

## Формат сериализации

"[VLQ-длина][битовые данные]"

**VLQ (Variable-Length Quantity):**
- Каждый байт: 7 бит данных + 1 бит продолжения
- Старший бит = 1: есть следующий байт
- Старший бит = 0: последний байт

Примеры:
- 10 элементов → "0x0A" (1 байт)
- 200 элементов → "0xC8 0x01" (2 байта)
- 16384 элементов → "0x80 0x80 0x01" (3 байта)

## Установка

Скопируй "ArrayBoolean.cs" в свой проект.

## Использование

### Создание

```csharp
// Из bool[]
var arr1 = new ArrayBoolean(new bool[] { true, false, true, true });

// Из закодированного byte[]
byte[] encoded = arr1.GetByte;
var arr2 = new ArrayBoolean(encoded);

// Через implicit operator
ArrayBoolean arr3 = new bool[] { true, false };
```

### Чтение/запись

```csharp
var arr = new ArrayBoolean(new bool[100]);

arr[0] = true;
arr[50] = true;

bool value = arr[0]; // true
int count = arr.Count; // 100
```

### Сериализация для сети

```csharp
// Отправка
var data = new ArrayBoolean(new bool[] { true, false, true });
byte[] toSend = data.GetByte;
NetworkStream.Write(toSend, 0, toSend.Length);

// Получение
byte[] received = ReadFromNetwork();
var restored = new ArrayBoolean(received);
// restored.Count корректен, данные не потеряны
```

### Операторы преобразования

```csharp
// bool[] → ArrayBoolean (implicit)
bool[] bools = new bool[] { true, false };
ArrayBoolean arr = bools;

// ArrayBoolean → bool[] (implicit)
bool[] restored = arr;

// byte[] → ArrayBoolean (explicit)
ArrayBoolean fromBytes = (ArrayBoolean)encoded;
```

### Клонирование и сравнение

```csharp
var original = new ArrayBoolean(new bool[] { true, false });
var clone = (ArrayBoolean)original.Clone();

bool equal = original.Equals(clone); // true

clone[0] = false;
equal = original.Equals(clone); // false
```

### Перечисление

```csharp
var arr = new ArrayBoolean(new bool[] { true, false, true });

foreach (bool value in arr)
{
    Console.WriteLine(value);
}
```

## Производительность

| Операция | Сложность |
|----------|-----------|
| Конструктор | O(n) |
| Get/Set по индексу | O(1) |
| Clone | O(n) |
| Equals | O(n) |
| Сериализация | O(1)* |

*Данные уже закодированы в памяти

### Экономия памяти

| Размер массива | bool[] | ArrayBoolean | Экономия |
|----------------|--------|--------------|----------|
| 10 | 10 bytes | 3 bytes | 70% |
| 100 | 100 bytes | 14 bytes | 86% |
| 1000 | 1000 bytes | 127 bytes | 87.3% |
| 10000 | 10000 bytes | 1252 bytes | 87.5% |

## Потокобезопасность

Класс **не является потокобезопасным**. Для concurrent доступа используй wrapper:

```csharp
public class ConcurrentArrayBoolean 
{
    private readonly ArrayBoolean _array;
    private readonly ReaderWriterLockSlim _lock = new();

    public bool this[int index]
    {
        get 
        { 
            _lock.EnterReadLock();
            try { return _array[index]; }
            finally { _lock.ExitReadLock(); }
        }
        set 
        { 
            _lock.EnterWriteLock();
            try { _array[index] = value; }
            finally { _lock.ExitWriteLock(); }
        }
    }
}
```

## Ограничения

- Максимальный размер ограничен "int.MaxValue" (можно расширить до "long")
- Индексатор бросает "ArgumentOutOfRangeException" при выходе за границы
- "GetByte" возвращает полный закодированный массив (нельзя получить только данные без VLQ-заголовка)

## Тестирование

Запусти программу для проверки корректности.

TestA

Все публичные методы покрыты тестами.

## Лицензия

Используй как хочешь.