using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
    void Span(Span<byte> span);

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
///     Uses system endianness by default; use <see cref="Be"/> and
///     <see cref="Le"/> for explicit endianness.
/// </remarks>
public readonly unsafe struct BinaryReader(Stream stream, bool disposeStream) : IBinaryReader
{
    public long Position
    {
        get => stream.Position;
        set => stream.Position = value;
    }

    public long Length => stream.Length;

    /// <summary>
    ///     Reads data in big-endian byte order.
    /// </summary>
    public BinaryReader<BigEndian, SystemEndian> Be => new(this);

    /// <summary>
    ///     Reads data in little-endian byte order.
    /// </summary>
    public BinaryReader<LittleEndian, SystemEndian> Le => new(this);

    private readonly bool isMemoryStream = stream is MemoryStream;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Read<T>() where T : unmanaged
    {
        AssertSize<T>();
        {
            if (typeof(T) == typeof(byte) || typeof(T) == typeof(sbyte))
            {
                var b = stream.ReadByte();
                {
                    Debug.Assert(b != -1);
                }

                return Unsafe.As<int, T>(ref b);
            }

            return MemoryMarshal.Read<T>(InternalRead(stackalloc byte[sizeof(T)]));
        }
    }

    public void Span(Span<byte> span)
    {
        AssertSize(span.Length);
        {
            stream.ReadExactly(span);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] Array(int length)
    {
        AssertSize(length);
        {
            var buffer = new byte[length];
            stream.ReadExactly(buffer, 0, length);
            return buffer;
        }
    }

#region Disposal
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        if (disposeStream)
        {
            stream.Dispose();
        }
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

    private ReadOnlySpan<byte> InternalRead(Span<byte> buffer)
    {
        if (isMemoryStream)
        {
            return Internal.MemoryStreamAccess.InternalReadSpan(Unsafe.As<MemoryStream>(stream), buffer.Length);
        }

        stream.ReadExactly(buffer);
        return buffer;
    }
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
    public void Span(Span<byte> span)
    {
        reader.Span(span);
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
        where TReader : struct, IBinaryReader, allows ref struct
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
        where TReader : struct, IBinaryReader, allows ref struct
    {
        var value = @this.Read<byte>();
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