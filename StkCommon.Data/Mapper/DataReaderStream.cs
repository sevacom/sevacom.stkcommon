using System;
using System.Data;
using System.IO;
using System.Text;

namespace StkCommon.Data.Mapper
{
    /// <summary>
    /// Поток формирующийся на основе данных из IDataReader
    /// </summary>
    public class DataReaderStream : Stream
    {
        private IDataReader _reader;
        private bool _isDisposeReader;
        private Action _extDisposeAction;
        private MemoryStream _stream = new MemoryStream();
        private bool _isEndRead;
        private Action<MemoryStream, IDataReader, int>[] _serializeActions;
        private byte[] _isDbNull;
        private readonly SerializeActionSwitcher _serializeActionSwitcher = new SerializeActionSwitcher();
        private int _fieldCount;

        public DataReaderStream(IDataReader reader, bool isDisposeReader, Action extDisposeAction = null)
        {
            _reader = reader;
            _isDisposeReader = isDisposeReader;
            _extDisposeAction = extDisposeAction;

            // Поместить во внутренний буфер заголовочню информацию о текущих данных ридера
            SerializeHeader(_stream, reader);
            _stream.Flush();
            _stream.Position = 0;
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var position = _stream.Position;

            _stream.Seek(0, SeekOrigin.End);
            // Пока во внутреннем потоке не хватает данных для результата и данные есть в ридере
            // Наполняем данным внутренний поток
            while (!_isEndRead && ((_stream.Length - position) < count))
            {
                if (_reader.Read())
                {
                    // В ридере есть данные
                    // В поток признак наличия данных
                    ToStream(_stream, true);
                    // Формируется массив признаков Null для полей
                    for (var i = _fieldCount - 1; i >= 0; i--)
                    {
                        _isDbNull[i] = (byte)(_reader.IsDBNull(i) ? 0 : 1);
                    }

                    // И этот массив признаков тоже в поток
                    _stream.Write(_isDbNull, 0, _fieldCount);

                    // Проход по всем полям в обратном порядке.
                    for (var i = _fieldCount - 1; i >= 0; i--)
                    {
                        // Если поле не нулл
                        if (1 == _isDbNull[i])
                        {
                            // То ранее определенным по типу поля сериализером это поле помещается в поток
                            _serializeActions[i](_stream, _reader, i);
                        }
                    }
                    _stream.Flush();
                }
                else
                {
                    // В ридере данных больше нет
                    // Поэтому в поток признак отсутствия данных
                    ToStream(_stream, false);
                    if (_reader.NextResult())
                    {
                        // Но может ридер не так прост
                        // Сериализация заголовка для новой порции данных
                        SerializeHeader(_stream, _reader);
                    }
                    else
                    {
                        // Ридер прям совсем пуст
                        // Бльше он не нужен
                        TryDisposeReader();
                    }
                    _stream.Flush();
                }
            }
            // Позиция при наполнении могла меняться - вернуть ее на прежнее место
            _stream.Position = position;
            // Из внутреннего потока возвращаются данные
            var res = _stream.Read(buffer, offset, count);

            // Удаление из внутреннего потока уже прочитанной информации
            var tBuff = new byte[_stream.Length - _stream.Position];
            if (0 < tBuff.Length)
            {
                _stream.Read(tBuff, 0, tBuff.Length);
                _stream.Position = 0;
                _stream.Write(tBuff, 0, tBuff.Length);
                _stream.Flush();
            }
            _stream.Position = 0;
            _stream.SetLength(tBuff.Length);

            return res;
        }

        private static void ToStream(MemoryStream stream, Guid value)
        {
            // 16 байт в поток
            var buff = value.ToByteArray();
            stream.Write(buff, 0, buff.Length);
        }

        private static void ToStream(MemoryStream stream, bool value)
        {
            // 1 байт в поток. 1 - true, 0 - false
            stream.WriteByte((byte)(value ? 1 : 0));
        }

        private static void ToStream(MemoryStream stream, byte value)
        {
            // 1 байт в поток
            stream.WriteByte(value);
        }

        private static void ToStream(MemoryStream stream, Int16 value)
        {
            // 2 байта в поток
            stream.Write(BitConverter.GetBytes(value), 0, sizeof(Int16));
        }

        private static void ToStream(MemoryStream stream, Int32 value)
        {
            // 4 байта в поток
            stream.Write(BitConverter.GetBytes(value), 0, sizeof(Int32));
        }

        private static void ToStream(MemoryStream stream, Int64 value)
        {
            // 8 байт в поток
            stream.Write(BitConverter.GetBytes(value), 0, sizeof(Int64));
        }

        private static void ToStream(MemoryStream stream, string value)
        {
            // 4 байта длины строки в поток, а потом и вся строка кодированная в UTF8
            var buff = Encoding.UTF8.GetBytes(value);
            ToStream(stream, buff.Length);
            stream.Write(buff, 0, buff.Length);
        }

        private static void ToStream(MemoryStream stream, DateTime value)
        {
            // 8 байт даты в поток
            ToStream(stream, value.ToBinary());
        }

        private static void ToStream(MemoryStream stream, char value)
        {
            // 2 байт в поток
            stream.Write(BitConverter.GetBytes(value), 0, 2);
        }

        private static void ToStream(MemoryStream stream, float value)
        {
            // 4 байт в поток
            stream.Write(BitConverter.GetBytes(value), 0, sizeof(float));
        }

        private static void ToStream(MemoryStream stream, double value)
        {
            // 8 байт в поток
            stream.Write(BitConverter.GetBytes(value), 0, sizeof(double));
        }

        private static void ToStream(MemoryStream stream, decimal value)
        {
            // 4 Int32 в поток
            var buff = decimal.GetBits(value);
            ToStream(stream, buff[0]);
            ToStream(stream, buff[1]);
            ToStream(stream, buff[2]);
            ToStream(stream, buff[3]);
        }

        private static void ToStream(MemoryStream stream, byte[] value)
        {
            // Размер массива а затем и байты в поток
            ToStream(stream, value.Length);
            stream.Write(value, 0, value.Length);
        }

        private void SerializeHeader(MemoryStream stream, IDataReader reader)
        {
            // Количество полей в поток
            _fieldCount = reader.FieldCount;
            ToStream(stream, _fieldCount);

            _serializeActions = new Action<MemoryStream, IDataReader, int>[_fieldCount];
            _isDbNull = new byte[_fieldCount];

            // Цикл по всем полям
            for (int i = 0; i < _fieldCount; i++)
            {
                var fieldType = reader.GetFieldType(i);
                _serializeActions[i] = GetSerializeActionFor(fieldType);

                // Название типа в SQL
                ToStream(stream, reader.GetDataTypeName(i));
                // Название типа в .net
                ToStream(stream, fieldType.FullName);
                // Название колонки
                ToStream(stream, reader.GetName(i));
            }
        }

        private Action<MemoryStream, IDataReader, int> GetSerializeActionFor(Type fieldType)
        {
            return _serializeActionSwitcher.Switch(fieldType, d =>
            {
                var func = (Func<Action<MemoryStream, IDataReader, int>>)d;
                return func();
            });
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        protected override void Dispose(bool disposing)
        {
            TryDisposeReader();
            _stream?.Dispose();
            _stream = null;
            base.Dispose(disposing);
        }

        private void TryDisposeReader()
        {
            _isEndRead = true;
            if (_isDisposeReader)
            {
                _isDisposeReader = false;
                _reader.Dispose();
            }
            _reader = null;
            if (null != _extDisposeAction)
            {
                _extDisposeAction();
                _extDisposeAction = null;
            }
        }

        private class SerializeActionSwitcher : BaseDataReaderTypeSwitcher
        {
            private static readonly Func<Action<MemoryStream, IDataReader, int>> GuidFunc = () => (s, r, i) => ToStream(s, r.GetGuid(i));
            private static readonly Func<Action<MemoryStream, IDataReader, int>> BooleanFunc = () => (s, r, i) => ToStream(s, r.GetBoolean(i));
            private static readonly Func<Action<MemoryStream, IDataReader, int>> StringFunc = () => (s, r, i) => ToStream(s, r.GetString(i));
            private static readonly Func<Action<MemoryStream, IDataReader, int>> DateTimeFunc = () => (s, r, i) => ToStream(s, r.GetDateTime(i));
            private static readonly Func<Action<MemoryStream, IDataReader, int>> ByteFunc = () => (s, r, i) => ToStream(s, r.GetByte(i));
            private static readonly Func<Action<MemoryStream, IDataReader, int>> Int16Func = () => (s, r, i) => ToStream(s, r.GetInt16(i));
            private static readonly Func<Action<MemoryStream, IDataReader, int>> Int32Func = () => (s, r, i) => ToStream(s, r.GetInt32(i));
            private static readonly Func<Action<MemoryStream, IDataReader, int>> Int64Func = () => (s, r, i) => ToStream(s, r.GetInt64(i));
            private static readonly Func<Action<MemoryStream, IDataReader, int>> CharFunc = () => (s, r, i) => ToStream(s, r.GetChar(i));
            private static readonly Func<Action<MemoryStream, IDataReader, int>> FloatFunc = () => (s, r, i) => ToStream(s, r.GetFloat(i));
            private static readonly Func<Action<MemoryStream, IDataReader, int>> DoubleFunc = () => (s, r, i) => ToStream(s, r.GetDouble(i));
            private static readonly Func<Action<MemoryStream, IDataReader, int>> DecimalFunc = () => (s, r, i) => ToStream(s, r.GetDecimal(i));
            private static readonly Func<Action<MemoryStream, IDataReader, int>> ByteArrayFunc = () => (s, r, i) => ToStream(s, (byte[])r[i]);

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