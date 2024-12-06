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

    /// <summary>
    ///     Reads an unmanaged type <typeparamref name="T"/> from the data
    ///     source.
    /// </summary>
    /// <typeparam name="T">The type to read.</typeparam>
    /// <returns>The value read from the data source.</returns>
    T Read<T>() where T : unmanaged;

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

        T IBinaryReader.Read<T>()
        {
            if (typeof(T) == typeof(sbyte) || typeof(T) == typeof(byte))
            {
                return Unsafe.As<byte, T>(ref data[Position++]);
            }

            var value = Unsafe.As<byte, T>(ref data[Position]);
            Position += sizeof(T);
            return value;
        }

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

#region Disposal
        void IDisposable.Dispose()
        {
            // No implementation necessary, there is no data to dispose of.
        }
#endregion
    }

    private sealed class Streamed(Stream stream, bool disposeStream) : IBinaryReader
    {
        public long Position { get; set; }

        long IBinaryReader.Length => stream.Length;

        T IBinaryReader.Read<T>()
        {
            if (typeof(T) == typeof(sbyte) || typeof(T) == typeof(byte))
            {
                var b = stream.ReadByte();
                {
                    Debug.Assert(b != -1);
                    b = (byte)b;
                }

                Position++;
                return Unsafe.As<int, T>(ref b);
            }

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Read<T>() where T : unmanaged
    {
        AssertSize<T>();
        {
            return impl.Read<T>();
        }
    }

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Read<T>() where T : unmanaged
    {
        return reverse_bytes ? EndianHelper.ReverseEndianness(reader.Read<T>()) : reader.Read<T>();
    }

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
    public static bool BooleanWide<TReader>(this TReader @this)
        where TReader : struct, IBinaryReader
    {
        var value = @this.Read<uint>();
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
    public static bool BooleanNarrow<TReader>(this TReader @this)
        where TReader : struct, IBinaryReader
    {
        var value = @this.Read<uint>();
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