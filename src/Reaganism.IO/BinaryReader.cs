using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace Reaganism.IO;

/// <summary>
///     Reads primitive data types and contiguous blocks of memory from an
///     arbitrary data source.
/// </summary>
public interface IBinaryReader : IDisposable
{
    /// <summary>
    ///     The position of the reader within the data source.
    /// </summary>
    long Position { get; set; }

    /// <summary>
    ///     The length of the data source.
    /// </summary>
    long Length { get; }

#region Primitive types
    /// <summary>
    ///     Reads a signed 8-bit integer.
    /// </summary>
    sbyte S8();

    /// <summary>
    ///     Reads an unsigned 8-bit integer.
    /// </summary>
    byte U8();

    /// <summary>
    ///     Reads a signed 16-bit integer.
    /// </summary>
    short S16();

    /// <summary>
    ///     Reads an unsigned 16-bit integer.
    /// </summary>
    ushort U16();

    /// <summary>
    ///     Reads a signed 32-bit integer.
    /// </summary>
    int S32();

    /// <summary>
    ///     Reads an unsigned 32-bit integer.
    /// </summary>
    uint U32();

    /// <summary>
    ///     Reads a signed 64-bit integer.
    /// </summary>
    long S64();

    /// <summary>
    ///     Reads an unsigned 64-bit integer.
    /// </summary>
    ulong U64();

    /// <summary>
    ///     Reads a single-precision floating-point number.
    /// </summary>
    float F32();

    /// <summary>
    ///     Reads a double-precision floating-point number.
    /// </summary>
    double F64();
#endregion

#region Contiguous memory
    /// <summary>
    ///     Reads a span of bytes from the data source.
    /// </summary>
    /// <param name="span">The span to write to.</param>
    void Span(ref Span<byte> span);

    /// <summary>
    ///     Reads an array of bytes from the data source.
    /// </summary>
    /// <param name="length">The length of data to read.</param>
    byte[] Array(int length);
#endregion
}

/// <summary>
///     Reads primitive data types and contiguous blocks of memory from an
///     arbitrary data source.
/// </summary>
/// <remarks>
///     Wraps an <see cref="IBinaryReader"/> implementation.
///     <br />
///     Uses system endianness by default; use <see cref="BigEndian"/> and
///     <see cref="LittleEndian"/> for explicit endianness.
/// </remarks>
public readonly unsafe struct BinaryReader(IBinaryReader impl) : IBinaryReader
{
    private sealed class Contiguous(byte[] data) : IBinaryReader
    {
        public long Position { get; set; }

        long IBinaryReader.Length => data.Length;

        private T Read<T>() where T : unmanaged
        {
            var value = Unsafe.As<byte, T>(ref data[Position]);
            Position += sizeof(T);
            return value;
        }

#region Primitive types
        sbyte IBinaryReader.S8()
        {
            return (sbyte)data[Position++];
        }

        byte IBinaryReader.U8()
        {
            return data[Position++];
        }

        short IBinaryReader.S16()
        {
            return Read<short>();
        }

        ushort IBinaryReader.U16()
        {
            return Read<ushort>();
        }

        int IBinaryReader.S32()
        {
            return Read<int>();
        }

        uint IBinaryReader.U32()
        {
            return Read<uint>();
        }

        long IBinaryReader.S64()
        {
            return Read<long>();
        }

        ulong IBinaryReader.U64()
        {
            return Read<ulong>();
        }

        float IBinaryReader.F32()
        {
            return Read<float>();
        }

        double IBinaryReader.F64()
        {
            return Read<double>();
        }
#endregion

#region Contiguous memory
        void IBinaryReader.Span(ref Span<byte> span)
        {
            // TODO(large-files): We assert that the ending position is within the
            //                    bounds of a 32-bit signed integer.
            Debug.Assert(Position + span.Length <= int.MaxValue);

            span     =  new Span<byte>(data, (int)Position, span.Length);
            Position += span.Length;
        }

        byte[] IBinaryReader.Array(int length)
        {
            // TODO(large-files): We assert that the ending position is within the
            //                    bounds of a 32-bit signed integer.
            Debug.Assert(Position + length <= int.MaxValue);

            var array = new byte[length];
            Buffer.BlockCopy(data, (int)Position, array, 0, length);
            Position += length;
            return array;
        }
#endregion

#region Disposal
        void IDisposable.Dispose()
        {
            // No implementation necessary, there is no data to dispose of.
        }
#endregion

#region Construction
        /// <summary>
        ///     Constructs a binary reader from a byte array.
        /// </summary>
        /// <param name="bytes">The byte array to read from.</param>
        /// <returns>The binary reader.</returns>
        public static BinaryReader FromByteArray(byte[] bytes)
        {
            return new BinaryReader(new Contiguous(bytes));
        }

        /// <summary>
        ///     Constructs a binary reader from a stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="disposeStream">
        ///     Whether to dispose of the stream when the reader is disposed.
        /// </param>
        /// <returns>The binary reader.</returns>
        public static BinaryReader FromStream(Stream stream, bool disposeStream)
        {
            return new BinaryReader(new Streamed(stream, disposeStream));
        }
#endregion
    }

    private sealed class Streamed(Stream stream, bool disposeStream) : IBinaryReader
    {
        public long Position { get; set; }

        long IBinaryReader.Length => stream.Length;

        private T Read<T>() where T : unmanaged
        {
            // TODO(perf): This isn't the most efficient approach.  We are
            //             indeed taking advantage of streaming, but it may
            //             prove more useful to read in chunks and interpret
            //             that data.

            var buffer = (Span<byte>)stackalloc byte[sizeof(T)];
            {
                var read = stream.Read(buffer);
                {
                    Debug.Assert(read == sizeof(T));
                }
            }

            Position += sizeof(T);
            return Unsafe.As<byte, T>(ref buffer[0]);
        }

#region Primitive types
        sbyte IBinaryReader.S8()
        {
            Position++;
            return (sbyte)stream.ReadByte();
        }

        byte IBinaryReader.U8()
        {
            Position++;
            return (byte)stream.ReadByte();
        }

        short IBinaryReader.S16()
        {
            return Read<short>();
        }

        ushort IBinaryReader.U16()
        {
            return Read<ushort>();
        }

        int IBinaryReader.S32()
        {
            return Read<int>();
        }

        uint IBinaryReader.U32()
        {
            return Read<uint>();
        }

        long IBinaryReader.S64()
        {
            return Read<long>();
        }

        ulong IBinaryReader.U64()
        {
            return Read<ulong>();
        }

        float IBinaryReader.F32()
        {
            return Read<float>();
        }

        double IBinaryReader.F64()
        {
            return Read<double>();
        }
#endregion

#region Contiguous memory
        void IBinaryReader.Span(ref Span<byte> span)
        {
            var read = stream.Read(span);
            {
                Debug.Assert(read == span.Length);
            }
        }

        byte[] IBinaryReader.Array(int length)
        {
            var buffer = new byte[length];
            {
                var read = stream.Read(buffer);
                {
                    Debug.Assert(read == length);
                }
            }

            return buffer;
        }
#endregion

#region Disposal
        void IDisposable.Dispose()
        {
            if (disposeStream)
            {
                stream.Dispose();
            }
        }
#endregion
    }

    public long Position
    {
        get => impl.Position;
        set => impl.Position = value;
    }

    public long Length => impl.Length;

    /// <summary>
    ///     Reads data in big-endian byte order.
    /// </summary>
    public BinaryReader<BigEndian, SystemEndian> BigEndian => new(this);

    /// <summary>
    ///     Reads data in little-endian byte order.
    /// </summary>
    public BinaryReader<LittleEndian, SystemEndian> LittleEndian => new(this);

#region Primitive types
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sbyte S8()
    {
        AssertSize<sbyte>();
        {
            return impl.S8();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte U8()
    {
        AssertSize<byte>();
        {
            return impl.U8();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short S16()
    {
        AssertSize<short>();
        {
            return impl.S16();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort U16()
    {
        AssertSize<ushort>();
        {
            return impl.U16();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int S32()
    {
        AssertSize<int>();
        {
            return impl.S32();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint U32()
    {
        AssertSize<uint>();
        {
            return impl.U32();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long S64()
    {
        AssertSize<long>();
        {
            return impl.S64();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong U64()
    {
        AssertSize<ulong>();
        {
            return impl.U64();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float F32()
    {
        AssertSize<float>();
        {
            return impl.F32();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double F64()
    {
        AssertSize<double>();
        {
            return impl.F64();
        }
    }
#endregion

#region Contiguous memory
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Span(ref Span<byte> span)
    {
        AssertSize(span.Length);
        {
            impl.Span(ref span);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] Array(int length)
    {
        AssertSize(length);
        {
            return impl.Array(length);
        }
    }
#endregion

#region Disposal
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        impl.Dispose();
    }
#endregion

#region Assertions
    /// <summary>
    ///     Asserts that there is enough available data remaining to read the
    ///     requested data type.
    /// </summary>
    /// <typeparam name="T">The requested data type.</typeparam>
    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AssertSize<T>() where T : unmanaged
    {
        AssertSize(sizeof(T));
    }

    /// <summary>
    ///     Asserts that there is enough available data remaining to read the
    ///     specified number of bytes.
    /// </summary>
    /// <param name="size">The number of bytes.</param>
    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AssertSize(int size)
    {
        Debug.Assert(Position >= 0 && Position + size <= Length);
    }
#endregion
}

/// <summary>
///     Endian-aware wrapper around <see cref="BinaryReader"/>.
/// </summary>
public readonly ref struct BinaryReader<TFromEndian, TToEndian>(BinaryReader reader) : IBinaryReader
    where TFromEndian : IEndianProvider
    where TToEndian : IEndianProvider
{
    // ReSharper disable once StaticMemberInGenericType - Intentional.
    /// <summary>
    ///     Whether to reverse the byte order when reading data.
    /// </summary>
    /// <remarks>
    ///     This is designed to be inlined by the JIT compiler at runtime so our
    ///     APIs may collapse to a single code path.
    /// </remarks>
    private static readonly bool reverse_bytes = TFromEndian.Endianness != TToEndian.Endianness;

    public long Position
    {
        get => reader.Position;
        set => reader.Position = value;
    }

    public long Length => reader.Length;

#region Primitive types
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sbyte S8()
    {
        return reader.S8();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte U8()
    {
        return reader.U8();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short S16()
    {
        var value = reader.S16();
        {
            return reverse_bytes ? BinaryPrimitives.ReverseEndianness(value) : value;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort U16()
    {
        var value = reader.U16();
        {
            return reverse_bytes ? BinaryPrimitives.ReverseEndianness(value) : value;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int S32()
    {
        var value = reader.S32();
        {
            return reverse_bytes ? BinaryPrimitives.ReverseEndianness(value) : value;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint U32()
    {
        var value = reader.U32();
        {
            return reverse_bytes ? BinaryPrimitives.ReverseEndianness(value) : value;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long S64()
    {
        var value = reader.S64();
        {
            return reverse_bytes ? BinaryPrimitives.ReverseEndianness(value) : value;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong U64()
    {
        var value = reader.U64();
        {
            return reverse_bytes ? BinaryPrimitives.ReverseEndianness(value) : value;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float F32()
    {
        return reader.F32();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double F64()
    {
        return reader.F64();
    }
#endregion

#region Contiguous memory
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Span(ref Span<byte> span)
    {
        reader.Span(ref span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] Array(int length)
    {
        return reader.Array(length);
    }
#endregion

#region Disposal
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IDisposable.Dispose()
    {
        throw new InvalidOperationException("Use the parent BinaryReader to dispose");
    }
#endregion
}

/// <summary>
///     Provides extension methods for <see cref="IBinaryReader"/>.
/// </summary>
public static class BinaryReaderExtensions
{
#region Boolean reading
    /// <summary>
    ///     Reads a wide (32-bit) boolean value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool BooleanWide(this IBinaryReader @this)
    {
        var value = @this.U32();
        {
            // Boolean values should be '0' or '1'.  Technically, the only
            // requirement is that we treat zero as falsy and non-zero as
            // truthy.  Regardless, we can typically reliably deduce that our
            // reading has gone astray if we happen to expect a boolean value
            // and we encounter a value that is not '0' or '1'.
            Debug.Assert(value is 0 or 1);
        }

        return value != 0;
    }

    /// <summary>
    ///     Reads a narrow (8-bit) boolean value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool BooleanNarrow(this IBinaryReader @this)
    {
        var value = @this.U8();
        {
            // Boolean values should be '0' or '1'.  Technically, the only
            // requirement is that we treat zero as falsy and non-zero as
            // truthy.  Regardless, we can typically reliably deduce that our
            // reading has gone astray if we happen to expect a boolean value
            // and we encounter a value that is not '0' or '1'.
            Debug.Assert(value is 0 or 1);
        }

        return value != 0;
    }
#endregion
}