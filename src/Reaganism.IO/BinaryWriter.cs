using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Reaganism.IO;

/// <summary>
///     Writes primitive data types and contiguous blocks of memory to an
///     arbitrary data source.
/// </summary>
public interface IBinaryWriter : IDisposable
{
    /// <summary>
    ///     The position of the writer within the data source.
    /// </summary>
    long Position { get; set; }

    /// <summary>
    ///     The length of written data within the data source.
    /// </summary>
    long Length { get; }

    /// <summary>
    ///     Writes a value of type <typeparamref name="T"/> to the data source.
    /// </summary>
    /// <param name="value">The value to read.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    void Write<T>(T value) where T : unmanaged;

    /// <summary>
    ///     Writes a span of bytes to the data source.
    /// </summary>
    void Span(Span<byte> value);

    /// <summary>
    ///     Writes an array of bytes to the data source.
    /// </summary>
    void Array(byte[] bytes);

#region Disposal
    /// <summary>
    ///     Flushes the writer.
    /// </summary>
    void Flush();
#endregion
}

/// <summary>
///     Writes primitive data types and contiguous blocks of memory to an
///     arbitrary data source.
/// </summary>
/// <remarks>
///     Wraps an <see cref="IBinaryWriter"/> implementation.
///     <br />
///     Uses system endianness by default; use <see cref="Be"/> and
///     <see cref="Le"/> for explicit endianness.
/// </remarks>
public readonly struct BinaryWriter(Stream stream, bool disposeStream) : IBinaryWriter
{
    public long Position
    {
        get => stream.Position;
        set => stream.Position = value;
    }

    public long Length => stream.Length;

    /// <summary>
    ///     Writes data in big-endian byte order.
    /// </summary>
    public BinaryWriter<BigEndian, SystemEndian> Be => new(this);

    /// <summary>
    ///     Writes data in little-endian byte order.
    /// </summary>
    public BinaryWriter<LittleEndian, SystemEndian> Le => new(this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void Write<T>(T value) where T : unmanaged
    {
        // Fast paths: manually write bytes for primitive types of known
        // sizes.
        if (typeof(T) == typeof(byte))
        {
            stream.WriteByte((byte)(object)value);
            return;
        }

        if (typeof(T) == typeof(sbyte))
        {
            stream.WriteByte((byte)(sbyte)(object)value);
            return;
        }

        if (typeof(T) == typeof(short))
        {
            stream.WriteByte((byte)(short)(object)value);
            stream.WriteByte((byte)((short)(object)value >> 0x8));
            return;
        }

        if (typeof(T) == typeof(ushort))
        {
            stream.WriteByte((byte)(ushort)(object)value);
            stream.WriteByte((byte)((ushort)(object)value >> 0x8));
            return;
        }

        if (typeof(T) == typeof(int))
        {
            stream.WriteByte((byte)(int)(object)value);
            stream.WriteByte((byte)((int)(object)value >> 0x8));
            stream.WriteByte((byte)((int)(object)value >> 0x10));
            stream.WriteByte((byte)((int)(object)value >> 0x18));
            return;
        }

        if (typeof(T) == typeof(uint))
        {
            stream.WriteByte((byte)(uint)(object)value);
            stream.WriteByte((byte)((uint)(object)value >> 0x8));
            stream.WriteByte((byte)((uint)(object)value >> 0x10));
            stream.WriteByte((byte)((uint)(object)value >> 0x18));
            return;
        }

        if (typeof(T) == typeof(long))
        {
            stream.WriteByte((byte)(long)(object)value);
            stream.WriteByte((byte)((long)(object)value >> 0x8));
            stream.WriteByte((byte)((long)(object)value >> 0x10));
            stream.WriteByte((byte)((long)(object)value >> 0x18));
            stream.WriteByte((byte)((long)(object)value >> 0x20));
            stream.WriteByte((byte)((long)(object)value >> 0x28));
            stream.WriteByte((byte)((long)(object)value >> 0x30));
            stream.WriteByte((byte)((long)(object)value >> 0x38));
            return;
        }

        if (typeof(T) == typeof(ulong))
        {
            stream.WriteByte((byte)(ulong)(object)value);
            stream.WriteByte((byte)((ulong)(object)value >> 0x8));
            stream.WriteByte((byte)((ulong)(object)value >> 0x10));
            stream.WriteByte((byte)((ulong)(object)value >> 0x18));
            stream.WriteByte((byte)((ulong)(object)value >> 0x20));
            stream.WriteByte((byte)((ulong)(object)value >> 0x28));
            stream.WriteByte((byte)((ulong)(object)value >> 0x30));
            stream.WriteByte((byte)((ulong)(object)value >> 0x38));
            return;
        }

        // Slow path: write bytes for unknown types.  This is also the path
        // used for floating-point types.
        Span<byte> bytes = stackalloc byte[sizeof(T)];
        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(bytes), value);
        stream.Write(bytes);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Span(Span<byte> value)
    {
        stream.Write(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Array(byte[] value)
    {
        stream.Write(value);
    }

#region Disposal
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Flush()
    {
        stream.Flush();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        if (disposeStream)
        {
            stream.Dispose();
        }
    }
#endregion
}

/// <summary>
///     Endian-aware wrapper around <see cref="BinaryWriter"/>.
/// </summary>
public readonly ref struct BinaryWriter<TFromEndian, TToEndian>(BinaryWriter writer) : IBinaryWriter
    where TFromEndian : IEndianProvider
    where TToEndian : IEndianProvider
{
    // ReSharper disable once StaticMemberInGenericType - Intentional.
    /// <summary>
    ///     Whether to reverse the byte order when writing data.
    /// </summary>
    /// <remarks>
    ///     This is designed to be inlined by the JIT compiler at runtime so our
    ///     APIs may collapse to a single code path.
    /// </remarks>
    private static readonly bool reverse_bytes = TFromEndian.Endianness != TToEndian.Endianness;

    public long Position
    {
        get => writer.Position;
        set => writer.Position = value;
    }

    public long Length => writer.Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<T>(T value) where T : unmanaged
    {
        writer.Write(reverse_bytes ? EndianHelper.ReverseEndianness(value) : value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Span(Span<byte> value)
    {
        writer.Span(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Array(byte[] bytes)
    {
        writer.Array(bytes);
    }

#region Disposal
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IBinaryWriter.Flush()
    {
        throw new InvalidOperationException("Use the parent BinaryWriter to flush");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IDisposable.Dispose()
    {
        throw new InvalidOperationException("Use the parent BinaryWriter to dispose");
    }
#endregion
}

/// <summary>
///     Provides extensions methods for <see cref="IBinaryWriter"/>.
/// </summary>
public static class BinaryWriterExtensions
{
#region Boolean writing
    /// <summary>
    ///     Writes a wide (32-bit) boolean value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void BooleanWide<TWriter>(this TWriter @this, bool value)
        where TWriter : struct, IBinaryWriter, allows ref struct
    {
        @this.Write(value ? 1u : 0u);
    }

    /// <summary>
    ///     Writes a narrow (8-bit) boolean value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void BooleanNarrow<TWriter>(this TWriter @this, bool value)
        where TWriter : struct, IBinaryWriter, allows ref struct
    {
        @this.Write(value ? (byte)1 : (byte)0);
    }
#endregion
}