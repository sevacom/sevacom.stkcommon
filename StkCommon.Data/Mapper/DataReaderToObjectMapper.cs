using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace StkCommon.Data.Mapper
{
    /// <summary>
    /// Класс который позволяет организовать какую-то специальную обработку
    /// В зависимости от типов данных возвращаемых IDataReader
    /// Класс созданн чтобы никто не забывал сделать обработку для всех поддерживаемых IDataReader'ом типов
    /// </summary>
    public abstract class BaseDataReaderTypeSwitcher
    {
        public T Switch<T>(Type fieldType, Func<Delegate, T> handler, bool isAllowNullable = false)
        {
            bool isNullable = false;
            if (isAllowNullable
                && fieldType.IsGenericType && !fieldType.IsGenericTypeDefinition
                && (typeof(Nullable<>) == fieldType.GetGenericTypeDefinition()))
            {
                fieldType = fieldType.GetGenericArguments()[0];
                isNullable = true;
            }
            if (typeof(Int16) == fieldType)
            {
                return handler(ActionForInt16(isNullable));
            }
            if (typeof(Int32) == fieldType)
            {
                return handler(ActionForInt32(isNullable));
            }
            if (typeof(Int64) == fieldType)
            {
                return handler(ActionForInt64(isNullable));
            }
            if (typeof(byte) == fieldType)
            {
                return handler(ActionForByte(isNullable));
            }
            if (typeof(char) == fieldType)
            {
                return handler(ActionForChar(isNullable));
            }
            if (typeof(DateTime) == fieldType)
            {
                return handler(ActionForDateTime(isNullable));
            }
            if (typeof(string) == fieldType)
            {
                return handler(ActionForString(isNullable));
            }
            if (typeof(bool) == fieldType)
            {
                return handler(ActionForBoolean(isNullable));
            }
            if (typeof(Guid) == fieldType)
            {
                return handler(ActionForGuid(isNullable));
            }
            if (typeof(float) == fieldType)
            {
                return handler(ActionForFloat(isNullable));
            }
            if (typeof(double) == fieldType)
            {
                return handler(ActionForDouble(isNullable));
            }
            if (typeof(decimal) == fieldType)
            {
                return handler(ActionForDecimal(isNullable));
            }
            if (typeof(byte[]) == fieldType)
            {
                return handler(ActionForByteArray(isNullable));
            }

            if (fieldType.IsEnum)
            {
                return handler(ActionForInt32(isNullable));
            }

            throw new NotSupportedException();
        }

        protected abstract Delegate ActionForByteArray(bool isNullable);

        protected abstract Delegate ActionForInt16(bool isNullable);

        protected abstract Delegate ActionForInt64(bool isNullable);

        protected abstract Delegate ActionForChar(bool isNullable);

        protected abstract Delegate ActionForFloat(bool isNullable);

        protected abstract Delegate ActionForDouble(bool isNullable);

        protected abstract Delegate ActionForDecimal(bool isNullable);

        protected abstract Delegate ActionForGuid(bool isNullable);

        protected abstract Delegate ActionForBoolean(bool isNullable);

        protected abstract Delegate ActionForString(bool isNullable);

        protected abstract Delegate ActionForDateTime(bool isNullable);

        protected abstract Delegate ActionForByte(bool isNullable);

        protected abstract Delegate ActionForInt32(bool isNullable);
    }

    /// <summary>
    /// Фабрика для простых мапперов из датаридера в объекты
    /// </summary>
    public static class DataReaderToObjectMapper
    {
        private static readonly Dictionary<Type, Dictionary<string, object>> Mappers = new Dictionary<Type, Dictionary<string, object>>();
        private static readonly GetterMapperSwitcher GetterSwitcher = new GetterMapperSwitcher();

        /// <summary>
        /// Получить простой и быстрый маппер данных ридера в объект
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="ignoreTypeCastWarning"></param>
        /// <returns></returns>
        public static IDataReaderToObjectMapper<T> GetMapper<T>(IDataRecord reader, bool ignoreTypeCastWarning = false)
         where T:new()
        {
            return GetMapper<T>(reader, null, ignoreTypeCastWarning);
        }

        /// <summary>
        /// Получить простой и быстрый маппер данных ридера в объект
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="propertyFinder"> </param>
        /// <param name="ignoreTypeCastWarning"></param>
        /// <returns></returns>
        public static IDataReaderToObjectMapper<T> GetMapper<T>(IDataRecord reader, Func<string, MemberInfoEx> propertyFinder, bool ignoreTypeCastWarning = false)
            where T:new()
        {
            // Искать маппер по типу
            Dictionary<string, object> mappers;
            // ReSharper disable once InconsistentlySynchronizedField
            if (!Mappers.TryGetValue(typeof(T), out mappers))
            {
                lock (Mappers)
                {
                    if (!Mappers.TryGetValue(typeof(T), out mappers))
                    {
                        mappers = new Dictionary<string, object>();
                        Mappers.Add(typeof(T), mappers);
                    }
                }
            }

            // Искать маппер по ключу
            // Ключ строиться на основе полей ридера
            var sb = new StringBuilder();
            for (var i = reader.FieldCount - 1; i >= 0; i--)
            {
                sb.Append("_" + reader.GetName(i));
            }
            var key = (ignoreTypeCastWarning ? "1" : "0") + sb;

            object mapper;
            if (!mappers.TryGetValue(key, out mapper))
            {
                lock (mappers)
                {
                    if (!mappers.TryGetValue(key, out mapper))
                    {
                        // Маппер не найден значит создать
                        mapper = new Mapper<T>(reader, propertyFinder, ignoreTypeCastWarning);
                        mappers.Add(key, mapper);
                    }
                }
            }
            return (IDataReaderToObjectMapper<T>)mapper;
        }

        private class Mapper<T> : IDataReaderToObjectMapper<T>
        {
            private Func<string, MemberInfoEx> PropertyFinder { get; }
            private readonly Func<IDataRecord, T> _mapperFunc;

            public Mapper(IDataRecord reader, Func<string, MemberInfoEx> propertyFinder, bool ignoreTypeCastWarning)
            {
                PropertyFinder = propertyFinder ?? InnerPropertyFinder;

                ParameterExpression eReader;
                BlockExpression eB;
                GenerateMapperExpression(reader, ignoreTypeCastWarning, out eReader, out eB);
                _mapperFunc = (Func<IDataRecord, T>)Expression.Lambda(eB, eReader).Compile();
            }

            /// <summary>
            /// Смапить текущую строку на объект
            /// </summary>
            /// <param name="record"></param>
            /// <returns></returns>
            public T GetObject(IDataRecord record)
            {
                return _mapperFunc(record);
            }

            /// <summary>
            /// Смапить весь ридер в коллекцию объектов
            /// </summary>
            /// <param name="reader"></param>
            /// <returns></returns>
            public List<T> GetCollection(IDataReader reader)
            {
                var res = new List<T>();
                while (reader.Read())
                {
                    var dbo = _mapperFunc(reader);
                    res.Add(dbo);
                }
                return res;
            }

            private static MemberInfoEx InnerPropertyFinder(string name)
            {
                MemberInfoEx memberInfo = null;
                // На основе имени колонки ищется свойство
                var propertyInfo = typeof(T).GetProperty(name);
                if (null == propertyInfo || !propertyInfo.CanWrite)
                {
                    // Свойство не доступно ищется поле
                    var fieldInfo = typeof(T).GetField(name);
                    if (null != fieldInfo)
                    {
                        memberInfo = new MemberInfoEx
                        {
                            MemberInfo = fieldInfo,
                            MemberType = fieldInfo.FieldType
                        };
                    }
                }
                else
                {
                    memberInfo = new MemberInfoEx
                    {
                        MemberInfo = propertyInfo,
                        MemberType = propertyInfo.PropertyType
                    };
                }
                return memberInfo;
            }

            private void GenerateMapperExpression(IDataRecord reader, bool ignoreTypeCastWarning
                , out ParameterExpression eReader, out BlockExpression eB)
            {
                var expressions = new List<Expression>();
                var eVar = Expression.Variable(typeof(T));
                eReader = Expression.Parameter(typeof(IDataRecord));

                var constructorInfo = typeof(T).GetConstructor(new Type[0]);
                if (null == constructorInfo)
                {
                    throw new NotSupportedException("Default constructor not found");
                }
                expressions.Add(Expression.Assign(eVar, Expression.New(constructorInfo)));

                for (var i = 0; i < reader.FieldCount; i++)
                {
                    MemberInfo memberInfo = null;
                    Type memberType = null;

                    var name = reader.GetName(i);


                    // На основе имени колонки ищется свойство
                    var res = PropertyFinder(name);
                    if (res != null)
                    {
                        memberInfo = res.MemberInfo;
                        memberType = res.MemberType;
                    }

                    // Нашли куда мапить
                    if (null != memberInfo)
                    {
                        if (!ignoreTypeCastWarning)
                        {
                            var mType = memberType;
                            if (memberType.IsGenericType && !memberType.IsGenericTypeDefinition
                                && (typeof(Nullable<>) == memberType.GetGenericTypeDefinition()))
                            {
                                mType = memberType.GetGenericArguments()[0];
                            }

                            if (mType != reader.GetFieldType(i))
                            {
                                // NOTE пропустим если мепинг идет на enum
                                if (!(mType.IsEnum && reader.GetFieldType(i).IsValueType && IsInteger(reader.GetFieldType(i))))
                                {
                                    // Проверка соответствия типов колонки в ридере и в "свойстве"
                                    throw new InvalidCastException(
                                        $"Field: {name}, {reader.GetFieldType(i)} to member: {memberInfo.Name}, {memberType}");
                                }
                            }
                        }
                        // Создане мапинга для текущей колонки
                        CreateSetExpression(eVar, memberInfo, memberType, eReader, i, ref expressions);
                    }
                }

                expressions.Add(eVar);

                eB = Expression.Block(new[] { eVar }, expressions);
            }

            public static bool IsInteger(Type value)
            {
                return (value == typeof(SByte) || value == typeof(Int16) || value == typeof(Int32)
                        || value == typeof(Int64) || value == typeof(Byte) || value == typeof(UInt16)
                        || value == typeof(UInt32) || value == typeof(UInt64));
            }

            private void CreateSetExpression(ParameterExpression eVar, MemberInfo memberInfo, Type memberType
                , ParameterExpression eReader, int index, ref List<Expression> expressions)
            {
                var recordType = typeof(IDataRecord);
                var eMa = Expression.MakeMemberAccess(eVar, memberInfo);
                var eIndexConst = Expression.Constant(index, typeof(int));
                var eIsNotNull = Expression.Not(Expression.Call(eReader, recordType.GetMethod("IsDBNull", new[] { typeof(int) }), eIndexConst));

                if (memberType.IsEnum)
                {
                    Expression eGetter = GetterSwitcher.Switch(memberType, d =>
                    {
                        var func = (Func<ParameterExpression, Type, Expression, Expression>)d;
                        return func(eReader, recordType, eIndexConst);
                    }, true);
                    //Enum.Parse(memberInfo.GetType(), );
                    eGetter = Expression.Convert(eGetter, memberType);
                    expressions.Add(Expression.IfThen(eIsNotNull, Expression.Assign(eMa, eGetter)));
                }
                else
                {
                    var eGetter = GetterSwitcher.Switch(memberType, d =>
                    {
                        var func = (Func<ParameterExpression, Type, Expression, Expression>)d;
                        return func(eReader, recordType, eIndexConst);
                    }, true);

                    expressions.Add(Expression.IfThen(eIsNotNull, Expression.Assign(eMa, eGetter)));
                }
            }
        }

        private class GetterMapperSwitcher : BaseDataReaderTypeSwitcher
        {
            private static readonly Func<ParameterExpression, Type, Expression, Expression> Int16Func = (eReader, recordType, eIndexConst)
                => Expression.Call(eReader, recordType.GetMethod("GetInt16", new[] { typeof(int) }), eIndexConst);
            private static readonly Func<ParameterExpression, Type, Expression, Expression> Int16NullableFunc = (eReader, recordType, eIndexConst)
                => Expression.Convert(Int16Func(eReader, recordType, eIndexConst), typeof(Int16?));
            private static readonly Func<ParameterExpression, Type, Expression, Expression> Int32Func = (eReader, recordType, eIndexConst)
                => Expression.Call(eReader, recordType.GetMethod("GetInt32", new[] { typeof(int) }), eIndexConst);
            private static readonly Func<ParameterExpression, Type, Expression, Expression> Int32NullableFunc = (eReader, recordType, eIndexConst)
                => Expression.Convert(Int32Func(eReader, recordType, eIndexConst), typeof(Int32?));
            private static readonly Func<ParameterExpression, Type, Expression, Expression> Int64Func = (eReader, recordType, eIndexConst)
                => Expression.Call(eReader, recordType.GetMethod("GetInt64", new[] { typeof(int) }), eIndexConst);
            private static readonly Func<ParameterExpression, Type, Expression, Expression> Int64NullableFunc = (eReader, recordType, eIndexConst)
                => Expression.Convert(Int64Func(eReader, recordType, eIndexConst), typeof(Int64?));
            private static readonly Func<ParameterExpression, Type, Expression, Expression> ByteFunc = (eReader, recordType, eIndexConst)
                => Expression.Call(eReader, recordType.GetMethod("GetByte", new[] { typeof(int) }), eIndexConst);
            private static readonly Func<ParameterExpression, Type, Expression, Expression> ByteNullableFunc = (eReader, recordType, eIndexConst)
                => Expression.Convert(ByteFunc(eReader, recordType, eIndexConst), typeof(byte?));
            private static readonly Func<ParameterExpression, Type, Expression, Expression> StringFunc = (eReader, recordType, eIndexConst)
                => Expression.Call(eReader, recordType.GetMethod("GetString", new[] { typeof(int) }), eIndexConst);
            private static readonly Func<ParameterExpression, Type, Expression, Expression> DateTimeFunc = (eReader, recordType, eIndexConst)
                => Expression.Call(eReader, recordType.GetMethod("GetDateTime", new[] { typeof(int) }), eIndexConst);
            private static readonly Func<ParameterExpression, Type, Expression, Expression> DateTimeNullableFunc = (eReader, recordType, eIndexConst)
                => Expression.Convert(DateTimeFunc(eReader, recordType, eIndexConst), typeof(DateTime?));
            private static readonly Func<ParameterExpression, Type, Expression, Expression> GuidFunc = (eReader, recordType, eIndexConst)
                => Expression.Call(eReader, recordType.GetMethod("GetGuid", new[] { typeof(int) }), eIndexConst);
            private static readonly Func<ParameterExpression, Type, Expression, Expression> GuidNullableFunc = (eReader, recordType, eIndexConst)
                => Expression.Convert(GuidFunc(eReader, recordType, eIndexConst), typeof(Guid?));
            private static readonly Func<ParameterExpression, Type, Expression, Expression> BooleanFunc = (eReader, recordType, eIndexConst)
                => Expression.Call(eReader, recordType.GetMethod("GetBoolean", new[] { typeof(int) }), eIndexConst);
            private static readonly Func<ParameterExpression, Type, Expression, Expression> BooleanNullableFunc = (eReader, recordType, eIndexConst)
                => Expression.Convert(BooleanFunc(eReader, recordType, eIndexConst), typeof(bool?));
            private static readonly Func<ParameterExpression, Type, Expression, Expression> CharFunc = (eReader, recordType, eIndexConst)
                => Expression.Call(eReader, recordType.GetMethod("GetChar", new[] { typeof(int) }), eIndexConst);
            private static readonly Func<ParameterExpression, Type, Expression, Expression> CharNullableFunc = (eReader, recordType, eIndexConst)
                => Expression.Convert(CharFunc(eReader, recordType, eIndexConst), typeof(char?));
            private static readonly Func<ParameterExpression, Type, Expression, Expression> FloatFunc = (eReader, recordType, eIndexConst)
                => Expression.Call(eReader, recordType.GetMethod("GetFloat", new[] { typeof(int) }), eIndexConst);
            private static readonly Func<ParameterExpression, Type, Expression, Expression> FloatNullableFunc = (eReader, recordType, eIndexConst)
                => Expression.Convert(FloatFunc(eReader, recordType, eIndexConst), typeof(float?));
            private static readonly Func<ParameterExpression, Type, Expression, Expression> DoubleFunc = (eReader, recordType, eIndexConst)
                => Expression.Call(eReader, recordType.GetMethod("GetDouble", new[] { typeof(int) }), eIndexConst);
            private static readonly Func<ParameterExpression, Type, Expression, Expression> DoubleNullableFunc = (eReader, recordType, eIndexConst)
                => Expression.Convert(DoubleFunc(eReader, recordType, eIndexConst), typeof(double?));
            private static readonly Func<ParameterExpression, Type, Expression, Expression> DecimalFunc = (eReader, recordType, eIndexConst)
                => Expression.Call(eReader, recordType.GetMethod("GetDecimal", new[] { typeof(int) }), eIndexConst);
            private static readonly Func<ParameterExpression, Type, Expression, Expression> DecimalNullableFunc = (eReader, recordType, eIndexConst)
                => Expression.Convert(DecimalFunc(eReader, recordType, eIndexConst), typeof(decimal?));
            private static readonly Func<ParameterExpression, Type, Expression, Expression> ByteArrayFunc = (eReader, recordType, eIndexConst)
                => Expression.Convert(Expression.Call(eReader, recordType.GetMethod("get_Item", new[] { typeof(int) }), eIndexConst), typeof(byte[]));

            protected override Delegate ActionForByteArray(bool isNullable)
            {
                return ByteArrayFunc;
            }

            protected override Delegate ActionForInt16(bool isNullable)
            {
                return isNullable ? Int16NullableFunc : Int16Func;
            }

            protected override Delegate ActionForInt64(bool isNullable)
            {
                return isNullable ? Int64NullableFunc : Int64Func;
            }

            protected override Delegate ActionForChar(bool isNullable)
            {
                return isNullable ? CharNullableFunc : CharFunc;
            }

            protected override Delegate ActionForFloat(bool isNullable)
            {
                return isNullable ? FloatNullableFunc : FloatFunc;
            }

            protected override Delegate ActionForDouble(bool isNullable)
            {
                return isNullable ? DoubleNullableFunc : DoubleFunc;
            }

            protected override Delegate ActionForDecimal(bool isNullable)
            {
                return isNullable ? DecimalNullableFunc : DecimalFunc;
            }

            protected override Delegate ActionForGuid(bool isNullable)
            {
                return isNullable ? GuidNullableFunc : GuidFunc;
            }

            protected override Delegate ActionForBoolean(bool isNullable)
            {
                return isNullable ? BooleanNullableFunc : BooleanFunc;
            }

            protected override Delegate ActionForString(bool isNullable)
            {
                return StringFunc;
            }

            protected override Delegate ActionForDateTime(bool isNullable)
            {
                return isNullable ? DateTimeNullableFunc : DateTimeFunc;
            }

            protected override Delegate ActionForByte(bool isNullable)
            {
                return isNullable ? ByteNullableFunc : ByteFunc;
            }

            protected override Delegate ActionForInt32(bool isNullable)
            {
                return isNullable ? Int32NullableFunc : Int32Func;
            }
        }
    }

    public class MemberInfoEx
    {
        public MemberInfo MemberInfo { get; set; }
        public Type MemberType { get; set; }
    }

    /// <summary>
    /// Очень простой и быстрый мапер IDataReader в объект
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataReaderToObjectMapper<T>
    {
        /// <summary>
        /// Смапить текущую строку на объект
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        T GetObject(IDataRecord record);
        /// <summary>
        /// Смапить весь ридер в коллекцию объектов
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        List<T> GetCollection(IDataReader reader);
    }
}
