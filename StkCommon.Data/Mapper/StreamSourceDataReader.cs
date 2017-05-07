using System;
using System.Data;
using System.IO;
using System.Text;

namespace StkCommon.Data.Mapper
{
    /// <summary>
    /// Дата ридер по данным сохраненным в потоке
    /// </summary>
    public class StreamSourceDataReader : IDataReader
    {
        private string[] _dataTypeNames;
        private Type[] _fieldTypes;
        private string[] _names;
        private StreamBuffer _buffer;
        private bool _isRead;
        private byte[] _isDbNulls;
        private DataField[] _fields;
        private Action<StreamBuffer, DataField>[] _deserializeActions;
        private readonly DeserializeActionSwitcher _deserializeActionSwitcher = new DeserializeActionSwitcher();

        public StreamSourceDataReader(Stream source, bool isDisposeSource)
        {
            _buffer = new StreamBuffer(source, isDisposeSource);
            DeserializeHeader(_buffer);
        }

        private void DeserializeHeader(StreamBuffer buffer)
        {
            // Количество полей
            var count = ReadInt32(buffer);

            // Названия типов в SQL
            _dataTypeNames = new string[count];
            // Типы
            _fieldTypes = new Type[count];
            // Названия
            _names = new string[count];

            // Структуры для хранения значений текущей строки
            _fields = new DataField[count];

            // Структура для хранения признака null
            _isDbNulls = new byte[count];

            // Действия по вытаскиванию значений колонок из потока и запихиванию их в _fields
            _deserializeActions = new Action<StreamBuffer, DataField>[count];

            // Для каждого поля из потока прочитывается информация
            // И инициализируются соответствующие структуры
            for (var i = 0; i < count; i++)
            {
                // Прочитать название типа в SQL
                _dataTypeNames[i] = ReadString(buffer);
                // Прочитать название .net типа и получить этот тип
                _fieldTypes[i] = Type.GetType(ReadString(buffer), true);
                // Прочитать название поля
                _names[i] = ReadString(buffer);

                // Инициализировать структуру для хранения данных поля
                _fields[i] = new DataField(_fieldTypes[i]);

                // Инициализировать десериализатор на основе типа поля
                _deserializeActions[i] = GetDeserializeActionFor(_fieldTypes[i]);
            }

            // Все готово для Read()
            _isRead = true;
            FieldCount = count;
        }

        private Action<StreamBuffer, DataField> GetDeserializeActionFor(Type fieldType)
        {
            return _deserializeActionSwitcher.Switch(fieldType, d =>
            {
                var func = (Func<Action<StreamBuffer, DataField>>)d;
                return func();
            });
        }

        public void Dispose()
        {
            Close();
        }

        public string GetName(int i)
        {
            return _names[i];
        }

        public string GetDataTypeName(int i)
        {
            return _dataTypeNames[i];
        }

        public Type GetFieldType(int i)
        {
            return _fieldTypes[i];
        }

        public object GetValue(int i)
        {
            return 1 == _isDbNulls[i]
                ? _fields[i].GetValueObject()
                : DBNull.Value;
        }

        public int GetValues(object[] values)
        {
            var res = Math.Min(FieldCount, values.Length);
            for (int i = res - 1; i >= 0; i--)
            {
                values[i] = GetValue(i);
            }
            return res;
        }

        public int GetOrdinal(string name)
        {
            return Array.IndexOf(_names, name);
        }

        public bool GetBoolean(int i)
        {
            return _fields[i].IsValueBoolean
                ? _fields[i].ValueBoolean
                : Convert.ToBoolean(_fields[i].GetValueObject());
        }

        public byte GetByte(int i)
        {
            return _fields[i].IsValueByte
                ? _fields[i].ValueByte
                : Convert.ToByte(_fields[i].GetValueObject());
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            if (_fields[i].IsValueByteArray)
            {
                if (null == buffer)
                {
                    return _fields[i].ValueByteArray.Length;
                }
                Array.Copy(_fields[i].ValueByteArray, fieldOffset, buffer, bufferoffset, length);
                return _fields[i].ValueByteArray.Length - fieldOffset - length;
            }
            throw new NotSupportedException();
        }

        public char GetChar(int i)
        {
            return _fields[i].IsValueChar
                ? _fields[i].ValueChar
                : Convert.ToChar(_fields[i].GetValueObject());
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotSupportedException();
        }

        public Guid GetGuid(int i)
        {
            return _fields[i].IsValueGuid
                ? _fields[i].ValueGuid
                : new Guid(Convert.ToString(_fields[i].GetValueObject()));
        }

        public short GetInt16(int i)
        {
            return _fields[i].IsValueInt16
                ? _fields[i].ValueInt16
                : Convert.ToInt16(_fields[i].GetValueObject());
        }

        public int GetInt32(int i)
        {
            return _fields[i].IsValueInt32
                ? _fields[i].ValueInt32
                : Convert.ToInt32(_fields[i].GetValueObject());
        }

        public long GetInt64(int i)
        {
            return _fields[i].IsValueInt64
                ? _fields[i].ValueInt64
                : Convert.ToInt64(_fields[i].GetValueObject());
        }

        public float GetFloat(int i)
        {
            return _fields[i].IsValueFloat
                ? _fields[i].ValueFloat
                : Convert.ToSingle(_fields[i].GetValueObject());
        }

        public double GetDouble(int i)
        {
            return _fields[i].IsValueDouble
                ? _fields[i].ValueDouble
                : Convert.ToDouble(_fields[i].GetValueObject());
        }

        public string GetString(int i)
        {
            return _fields[i].IsValueString
                ? _fields[i].ValueString
                : Convert.ToString(_fields[i].GetValueObject());
        }

        public decimal GetDecimal(int i)
        {
            return _fields[i].IsValueDecimal
                ? _fields[i].ValueDecimal
                : Convert.ToDecimal(_fields[i].GetValueObject());
        }

        public DateTime GetDateTime(int i)
        {
            return _fields[i].IsValueDateTime
                ? _fields[i].ValueDateTime
                : Convert.ToDateTime(_fields[i].GetValueObject());
        }

        public IDataReader GetData(int i)
        {
            throw new NotSupportedException();
        }

        public bool IsDBNull(int i)
        {
            return 0 == _isDbNulls[i];
        }

        public int FieldCount { get; private set; }

        object IDataRecord.this[int i] => _fields[i].GetValueObject();

        object IDataRecord.this[string name] => _fields[GetOrdinal(name)].GetValueObject();

        public void Close()
        {
            if (null != _buffer)
            {
                _buffer.Dispose();
                _buffer = null;
            }
            IsClosed = true;
        }

        public DataTable GetSchemaTable()
        {
            throw new NotSupportedException();
        }

        public bool NextResult()
        {
            // Вначале до конца вычитывается текущая таблица
            while (Read())
            {
            }
            // Проверка есть ли в буфере еще данные
            var res = sizeof(int) == _buffer.Prepare(sizeof(int));
            if (res)
            {
                // Данные есть значит там заголовок новой таблицы
                // И вся обработка по новому
                DeserializeHeader(_buffer);
            }
            return res;
        }

        public bool Read()
        {
            if (!_isRead)
            {
                // Чтение уже закончено, так что нет
                return false;
            }
            // Считывание признака наличия следующей строки
            // Следующей строки нет значит все больше никаких рид
            _isRead = ReadBoolean(_buffer);
            if (_isRead)
            {
                // Если строка есть то считать ее
                ReadContent(_buffer);
            }
            return _isRead;
        }

        private void ReadContent(StreamBuffer buffer)
        {
            var count = FieldCount;

            // Первым делом считываются признаки null для полей текущей строки
            buffer.Prepare(count);
            Array.Copy(buffer.Data, buffer.Position, _isDbNulls, 0, count);
            buffer.IncPosition(count);

            // Поля записаны в обратном порядке вот и считываться они будут в обратном порядке
            for (int i = count - 1; i >= 0; i--)
            {
                // Если поле не null то оно считывается из буфера
                // Ранее инициализированным в зависимости от типа поля десериализером
                // Null поля в поток просто не записаны и соответственно не считываются
                if (1 == _isDbNulls[i])
                {
                    _deserializeActions[i](buffer, _fields[i]);
                }
            }
        }

        private static byte ReadByte(StreamBuffer buffer)
        {
            // Просто один байт из потока
            buffer.Prepare(1);
            var res = buffer.Data[buffer.Position];
            buffer.IncPosition(1);
            return res;
        }

        private static byte[] ReadByteArray(StreamBuffer buffer)
        {
            // Размер массива а затем данные для массива из потока
            var arrLen = ReadInt32(buffer);
            buffer.Prepare(arrLen);
            var res = new byte[arrLen];
            Array.Copy(buffer.Data, buffer.Position, res, 0, arrLen);
            buffer.IncPosition(arrLen);
            return res;
        }

        private static bool ReadBoolean(StreamBuffer buffer)
        {
            // Один байт. 1 - true, 0 - false
            return 1 == ReadByte(buffer);
        }

        private static Int16 ReadInt16(StreamBuffer buffer)
        {
            // 2 байта из потока
            const int size = sizeof(Int16);
            buffer.Prepare(size);
            var res = BitConverter.ToInt16(buffer.Data, buffer.Position);
            buffer.IncPosition(size);
            return res;
        }

        private static Int32 ReadInt32(StreamBuffer buffer)
        {
            // Четыре байта из потока
            const int size = sizeof(Int32);
            buffer.Prepare(size);
            var res = BitConverter.ToInt32(buffer.Data, buffer.Position);
            buffer.IncPosition(size);
            return res;
        }

        private static Int64 ReadInt64(StreamBuffer buffer)
        {
            // 8 байт из потока
            const int size = sizeof(Int64);
            buffer.Prepare(size);
            var res = BitConverter.ToInt64(buffer.Data, buffer.Position);
            buffer.IncPosition(size);
            return res;
        }

        private static string ReadString(StreamBuffer buffer)
        {
            // Длина строки 4 байта, и далее UTF8 кодированная строка
            var strLen = ReadInt32(buffer);
            buffer.Prepare(strLen);
            var res = Encoding.UTF8.GetString(buffer.Data, buffer.Position, strLen);
            buffer.IncPosition(strLen);
            return res;
        }

        private static DateTime ReadDateTime(StreamBuffer buffer)
        {
            // 8 байт из потока
            const int size = sizeof(long);
            buffer.Prepare(size);
            var res = BitConverter.ToInt64(buffer.Data, buffer.Position);
            buffer.IncPosition(size);
            return DateTime.FromBinary(res);
        }

        private static Guid ReadGuid(StreamBuffer buffer)
        {
            // 16 байт из потока
            const int size = 16;
            buffer.Prepare(size);
            var guidBytes = new byte[size];
            Array.Copy(buffer.Data, buffer.Position, guidBytes, 0, size);
            buffer.IncPosition(size);
            return new Guid(guidBytes);
        }

        private static char ReadChar(StreamBuffer buffer)
        {
            // 2 байт из потока
            const int size = 2;
            buffer.Prepare(size);
            var res = BitConverter.ToChar(buffer.Data, buffer.Position);
            buffer.IncPosition(size);
            return res;
        }

        private static float ReadFloat(StreamBuffer buffer)
        {
            // Четыре байта из потока
            const int size = sizeof(float);
            buffer.Prepare(size);
            var res = BitConverter.ToSingle(buffer.Data, buffer.Position);
            buffer.IncPosition(size);
            return res;
        }

        private static double ReadDouble(StreamBuffer buffer)
        {
            // 8 байт из потока
            const int size = sizeof(double);
            buffer.Prepare(size);
            var res = BitConverter.ToDouble(buffer.Data, buffer.Position);
            buffer.IncPosition(size);
            return res;
        }

        private static decimal ReadDecimal(StreamBuffer buffer)
        {
            // 4 Int32 из потока
            var bits = new int[4];
            bits[0] = ReadInt32(buffer);
            bits[1] = ReadInt32(buffer);
            bits[2] = ReadInt32(buffer);
            bits[3] = ReadInt32(buffer);
            return new decimal(bits);
        }

        public int Depth => 0;
        public bool IsClosed { get; private set; }
        public int RecordsAffected => -1;

        /// <summary>
        /// Потоковый буфер
        /// Специальный класс оптимизирующий работу с потоком
        /// За счет считывания данных крупными порциями
        /// Но предоставляющий интерфес доступа к данным мелкими порциями
        /// </summary>
        private class StreamBuffer : IDisposable
        {
            private Stream _source;
            private bool _isDisposeSource;

            public StreamBuffer(Stream source, bool isDisposeSource)
            {
                _source = source;
                _isDisposeSource = isDisposeSource;
            }

            /// <summary>
            /// Текущая позиция в массив Data
            /// </summary>
            public int Position;

            /// <summary>
            /// Текущий прочитанный буфер данных
            /// </summary>
            public byte[] Data = new byte[0];

            public void Dispose()
            {
                TryDisposeSource();
            }

            private void TryDisposeSource()
            {
                if (_isDisposeSource)
                {
                    _isDisposeSource = false;
                    _source.Dispose();
                }
                _source = null;
            }

            /// <summary>
            /// Подготовить буфер (массив Data) для считывания заданного колчества байт
            /// Если буфер подготовить для запрощенного количества не получается
            /// Возвращает количество байт которые получиться считать
            /// </summary>
            /// <param name="count"></param>
            /// <returns></returns>
            public int Prepare(int count)
            {
                var lastCount = Data.Length - Position;
                // Пока в буфере на хватает данных
                while (count > lastCount)
                {
                    if (null == _source)
                    {
                        // Источника нет значит сколько хватает столько и хватает
                        return lastCount;
                    }
                    // Считывание блока данных из потока
                    const int l = 4096;
                    var buff = new byte[l];
                    var r = _source.Read(buff, 0, l);
                    if (0 == r)
                    {
                        // Если поток пуст, то больше данных в буфере и не будет
                        // Поток больше не нужен и он освобождается
                        TryDisposeSource();
                        return lastCount;
                    }
                    // Сколько то данных считалось
                    // Подготавливается новый буфер размером с остаток старого + новые данные
                    var data = new byte[lastCount + r];
                    // Заполняется новый буфер
                    Array.Copy(Data, Position, data, 0, lastCount);
                    Array.Copy(buff, 0, data, lastCount, r);
                    // И становить текущем буфером
                    Data = data;
                    // А позиция теперь в 0
                    Position = 0;
                    // И осталось данных столько сколько всего в буфере
                    lastCount = Data.Length;
                }
                // В буфере или еще хватает данных или они были досчитаны и теперь их опять хватает
                return count;
            }

            /// <summary>
            /// Сместить текущую позицию
            /// </summary>
            /// <param name="value"></param>
            public void IncPosition(int value)
            {
                Position += value;
            }
        }

        /// <summary>
        /// Специальный класс для хранения данных колонки в нативном представлении этой колонки
        /// Чтобы не было всяких боксингов унбоксингов
        /// </summary>
        private class DataField
        {
            private readonly OptimizeGetterSwitcher _getterSwitcher = new OptimizeGetterSwitcher();
            private Func<DataField, object> _getValueObject = df => null;

            public Int16 ValueInt16;
            public int ValueInt32;
            public Int64 ValueInt64;
            public byte ValueByte;
            public byte[] ValueByteArray;
            public char ValueChar;
            public string ValueString;
            public DateTime ValueDateTime;
            public bool ValueBoolean;
            public Guid ValueGuid;
            public float ValueFloat;
            public double ValueDouble;
            public decimal ValueDecimal;

            public bool IsValueInt16;
            public bool IsValueInt32;
            public bool IsValueInt64;
            public bool IsValueByte;
            public bool IsValueByteArray;
            public bool IsValueChar;
            public bool IsValueString;
            public bool IsValueDateTime;
            public bool IsValueBoolean;
            public bool IsValueGuid;
            public bool IsValueFloat;
            public bool IsValueDouble;
            public bool IsValueDecimal;

            public DataField(Type fieldType)
            {
                SetIsType(fieldType);
            }

            private void SetIsType(Type dataType)
            {
                _getterSwitcher.Switch<object>(dataType, d =>
                {
                    var action = (Action<DataField>)d;
                    action(this);
                    return null;
                });
            }

            /// <summary>
            /// Получить значение как объект
            /// </summary>
            /// <returns></returns>
            public object GetValueObject()
            {
                return _getValueObject(this);
            }

            private class OptimizeGetterSwitcher : BaseDataReaderTypeSwitcher
            {
                private readonly static Action<DataField> GuidAction = df =>
                {
                    df._getValueObject = field => field.ValueGuid;
                    df.IsValueGuid = true;
                };
                private readonly static Action<DataField> BooleanAction = df =>
                {
                    df._getValueObject = field => field.ValueBoolean;
                    df.IsValueBoolean = true;
                };
                private readonly static Action<DataField> StringAction = df =>
                {
                    df._getValueObject = field => field.ValueString;
                    df.IsValueString = true;
                };
                private readonly static Action<DataField> DateTimeAction = df =>
                {
                    df._getValueObject = field => field.ValueDateTime;
                    df.IsValueDateTime = true;
                };
                private readonly static Action<DataField> ByteAction = df =>
                {
                    df._getValueObject = field => field.ValueByte;
                    df.IsValueByte = true;
                };
                private readonly static Action<DataField> ByteArrayAction = df =>
                {
                    df._getValueObject = field => field.ValueByteArray;
                    df.IsValueByteArray = true;
                };
                private readonly static Action<DataField> Int16Action = df =>
                {
                    df._getValueObject = field => field.ValueInt16;
                    df.IsValueInt16 = true;
                };
                private readonly static Action<DataField> Int32Action = df =>
                {
                    df._getValueObject = field => field.ValueInt32;
                    df.IsValueInt32 = true;
                };
                private readonly static Action<DataField> Int64Action = df =>
                {
                    df._getValueObject = field => field.ValueInt64;
                    df.IsValueInt64 = true;
                };
                private readonly static Action<DataField> CharAction = df =>
                {
                    df._getValueObject = field => field.ValueChar;
                    df.IsValueChar = true;
                };
                private readonly static Action<DataField> FloatAction = df =>
                {
                    df._getValueObject = field => field.ValueFloat;
                    df.IsValueFloat = true;
                };
                private readonly static Action<DataField> DoubleAction = df =>
                {
                    df._getValueObject = field => field.ValueDouble;
                    df.IsValueDouble = true;
                };
                private readonly static Action<DataField> DecimalAction = df =>
                {
                    df._getValueObject = field => field.ValueDecimal;
                    df.IsValueDecimal = true;
                };

                protected override Delegate ActionForByteArray(bool isNullable)
                {
                    return ByteArrayAction;
                }

                protected override Delegate ActionForInt16(bool isNullable)
                {
                    return Int16Action;
                }

                protected override Delegate ActionForInt64(bool isNullable)
                {
                    return Int64Action;
                }

                protected override Delegate ActionForChar(bool isNullable)
                {
                    return CharAction;
                }

                protected override Delegate ActionForFloat(bool isNullable)
                {
                    return FloatAction;
                }

                protected override Delegate ActionForDouble(bool isNullable)
                {
                    return DoubleAction;
                }

                protected override Delegate ActionForDecimal(bool isNullable)
                {
                    return DecimalAction;
                }

                protected override Delegate ActionForGuid(bool isNullable)
                {
                    return GuidAction;
                }

                protected override Delegate ActionForBoolean(bool isNullable)
                {
                    return BooleanAction;
                }

                protected override Delegate ActionForString(bool isNullable)
                {
                    return StringAction;
                }

                protected override Delegate ActionForDateTime(bool isNullable)
                {
                    return DateTimeAction;
                }

                protected override Delegate ActionForByte(bool isNullable)
                {
                    return ByteAction;
                }

                protected override Delegate ActionForInt32(bool isNullable)
                {
                    return Int32Action;
                }
            }
        }

        private class DeserializeActionSwitcher : BaseDataReaderTypeSwitcher
        {
            private readonly static Func<Action<StreamBuffer, DataField>> GuidFunc = () => (b, f) => f.ValueGuid = ReadGuid(b);
            private readonly static Func<Action<StreamBuffer, DataField>> BooleanFunc = () => (b, f) => f.ValueBoolean = ReadBoolean(b);
            private readonly static Func<Action<StreamBuffer, DataField>> StringFunc = () => (b, f) => f.ValueString = ReadString(b);
            private readonly static Func<Action<StreamBuffer, DataField>> DateTimeFunc = () => (b, f) => f.ValueDateTime = ReadDateTime(b);
            private readonly static Func<Action<StreamBuffer, DataField>> ByteFunc = () => (b, f) => f.ValueByte = ReadByte(b);
            private readonly static Func<Action<StreamBuffer, DataField>> ByteArrayFunc = () => (b, f) => f.ValueByteArray = ReadByteArray(b);
            private readonly static Func<Action<StreamBuffer, DataField>> Int16Func = () => (b, f) => f.ValueInt16 = ReadInt16(b);
            private readonly static Func<Action<StreamBuffer, DataField>> Int32Func = () => (b, f) => f.ValueInt32 = ReadInt32(b);
            private readonly static Func<Action<StreamBuffer, DataField>> Int64Func = () => (b, f) => f.ValueInt64 = ReadInt64(b);
            private readonly static Func<Action<StreamBuffer, DataField>> CharFunc = () => (b, f) => f.ValueChar = ReadChar(b);
            private readonly static Func<Action<StreamBuffer, DataField>> FloatFunc = () => (b, f) => f.ValueFloat = ReadFloat(b);
            private readonly static Func<Action<StreamBuffer, DataField>> DoubleFunc = () => (b, f) => f.ValueDouble = ReadDouble(b);
            private readonly static Func<Action<StreamBuffer, DataField>> DecimalFunc = () => (b, f) => f.ValueDecimal = ReadDecimal(b);

            protected override Delegate ActionForByteArray(bool isNullable)
            {
                return ByteArrayFunc;
            }

            protected override Delegate ActionForInt16(bool isNullable)
            {
                return Int16Func;
            }

            protected override Delegate ActionForInt64(bool isNullable)
            {
                return Int64Func;
            }

            protected override Delegate ActionForChar(bool isNullable)
            {
                return CharFunc;
            }

            protected override Delegate ActionForFloat(bool isNullable)
            {
                return FloatFunc;
            }

            protected override Delegate ActionForDouble(bool isNullable)
            {
                return DoubleFunc;
            }

            protected override Delegate ActionForDecimal(bool isNullable)
            {
                return DecimalFunc;
            }

            protected override Delegate ActionForGuid(bool isNullable)
            {
                return GuidFunc;
            }

            protected override Delegate ActionForBoolean(bool isNullable)
            {
                return BooleanFunc;
            }

            protected override Delegate ActionForString(bool isNullable)
            {
                return StringFunc;
            }

            protected override Delegate ActionForDateTime(bool isNullable)
            {
                return DateTimeFunc;
            }

            protected override Delegate ActionForByte(bool isNullable)
            {
                return ByteFunc;
            }

            protected override Delegate ActionForInt32(bool isNullable)
            {
                return Int32Func;
            }
        }
    }
}