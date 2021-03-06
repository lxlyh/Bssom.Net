﻿//using System.Runtime.CompilerServices;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
namespace Bssom.Serializer.Internal
{
    internal static class Array1ElementWriterFactory<T>
    {
        private static readonly IArray1ElementWriter<T> _controller;

        static Array1ElementWriterFactory()
        {
            Type t = typeof(T);
            if (t == typeof(object) || t == typeof(BssomValue) || t == typeof(BssomChar) || t == typeof(BssomBoolean) || t == typeof(BssomDateTime) || t == typeof(BssomDecimal) || t == typeof(BssomFloat) || t == typeof(BssomGuid) || t == typeof(BssomNumber))
            {
                _controller = (IArray1ElementWriter<T>)(object)ObjectArray1ElementWriter.Instance;
            }
            else
            {
                _controller = (IArray1ElementWriter<T>)Array1ElementWriterContainers.GetArray1ElementWriter(t);
            }
        }

        public static void WriteElement(ref BssomWriter writer, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo, T value)
        {
            _controller.WriteElement(ref writer, option, offsetInfo, value);
        }

        public static T ReadElement(ref BssomReader reader, BssomSerializerOptions option, BssomFieldOffsetInfo offsetInfo)
        {
            return _controller.ReadElement(ref reader, option, offsetInfo);
        }
    }

    internal static class Array1ElementWriterContainers
    {
        private static readonly Dictionary<Type, IArray1ElementWriter> containers = new Dictionary<Type, IArray1ElementWriter>()
        {
            {typeof(Int16),Int16Array1ElementWriter.Instance },
            {typeof(Int32), Int32Array1ElementWriter.Instance },
            {typeof(Int64), Int64Array1ElementWriter.Instance },
            {typeof(UInt16), UInt16Array1ElementWriter.Instance },
            {typeof(UInt32), UInt32Array1ElementWriter.Instance },
            {typeof(UInt64), UInt64Array1ElementWriter.Instance },
            {typeof(Byte),UInt8Array1ElementWriter.Instance },
            {typeof(SByte), Int8Array1ElementWriter.Instance },
            {typeof(Single), Float32Array1ElementWriter.Instance },
            {typeof(Double), Float64Array1ElementWriter.Instance },
            {typeof(Boolean), BooleanArray1ElementWriter.Instance },

            {typeof(Char),CharArray1ElementWriter.Instance },
            {typeof(Guid),GuidArray1ElementWriter.Instance },
            {typeof(DateTime), DateTimeArray1ElementWriter.Instance },
            {typeof(Decimal), DecimalArray1ElementWriter.Instance },
        };
        private static readonly IArray1ElementWriter[] nativeTypeContainers = new IArray1ElementWriter[] {
            CharArray1ElementWriter.Instance,
            GuidArray1ElementWriter.Instance,
            DecimalArray1ElementWriter.Instance,
            DateTimeArray1ElementWriter.Instance
        };
        private static readonly IArray1ElementWriter[] buildInTypeContainers = new IArray1ElementWriter[] {
            Int8Array1ElementWriter.Instance,
            Int16Array1ElementWriter.Instance,
            Int32Array1ElementWriter.Instance,
            Int64Array1ElementWriter.Instance,
            UInt8Array1ElementWriter.Instance,
            UInt16Array1ElementWriter.Instance,
            UInt32Array1ElementWriter.Instance,
            UInt64Array1ElementWriter.Instance,
            Float32Array1ElementWriter.Instance,
            Float64Array1ElementWriter.Instance,
            BooleanArray1ElementWriter.Instance,
            DateTimeArray1ElementWriter.Instance,
        };

        static Array1ElementWriterContainers()
        {
            nativeTypeContainers = new IArray1ElementWriter[NativeBssomType.MaxCode + 1];
            nativeTypeContainers[NativeBssomType.CharCode] = CharArray1ElementWriter.Instance;
            nativeTypeContainers[NativeBssomType.GuidCode] = GuidArray1ElementWriter.Instance;
            nativeTypeContainers[NativeBssomType.DecimalCode] = DecimalArray1ElementWriter.Instance;
            nativeTypeContainers[NativeBssomType.DateTimeCode] = DateTimeArray1ElementWriter.Instance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IArray1ElementWriter GetArray1ElementWriter(Type type)
        {
            if (containers.TryGetValue(type, out IArray1ElementWriter array1ElementWriter))
            {
                return array1ElementWriter;
            }

            return BssomSerializationArgumentException.InvalidOffsetInfoFormat<IArray1ElementWriter>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IArray1ElementWriter GetArray1ElementWriter(bool isNativeType, byte type)
        {
            if (!isNativeType)
            {
                if (type > BssomType.MinFixLenTypeCode && type <= BssomType.TimestampCode)
                {
                    return buildInTypeContainers[type - BssomType.MinFixLenTypeCode - 1];
                }
            }
            else
            {
                if (type <= NativeBssomType.MaxCode)
                {
                    IArray1ElementWriter writer = nativeTypeContainers[type];
                    if (writer != null)
                    {
                        return writer;
                    }
                }
            }
            return BssomSerializationArgumentException.InvalidOffsetInfoFormat<IArray1ElementWriter>();
        }
    }
}
