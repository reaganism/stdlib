using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

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

#region Primitive types
    /// <summary>
    ///     Writes a signed 8-bit integer.
    /// </summary>
    void S8(sbyte value);

    /// <summary>
    ///     Writes an unsigned 8-bit integer.
    /// </summary>
    void U8(byte value);

    /// <summary>
    ///     Writes a signed 16-bit integer.
    /// </summary>
    void S16(short value);

    /// <summary>
    ///     Writes an unsigned 16-bit integer.
    /// </summary>
    void U16(ushort value);

    /// <summary>
    ///     Writes a signed 32-bit integer.
    /// </summary>
    void S32(int value);

    /// <summary>
    ///     Writes an unsigned 32-bit integer.
    /// </summary>
    void U32(uint value);

    /// <summary>
    ///     Writes a signed 64-bit integer.
    /// </summary>
    void S64(long value);

    /// <summary>
    ///     Writes an unsigned 64-bit integer.
    /// </summary>
    void U64(ulong value);

    /// <summary>
    ///     Writes a single-precision floating-point number.
    /// </summary>
    void F32(float value);

    /// <summary>
    ///     Writes a double-precision floating-point number.
    /// </summary>
    void F64(double value);
#endregion

#region Contiguous memory
    /// <summary>
    ///     Writes a span of bytes to the data source.
    /// </summary>
    void Span(Span<byte> value);

    /// <summary>
    ///     Writes an array of bytes to the data source.
    /// </summary>
    void Array(byte[] bytes);
#endregion

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
///     Uses system endianness by default; use <see cref="BigEndian"/> and
///     <see cref="LittleEndian"/> for explicit endianness.
/// </remarks>
public readonly unsafe partial struct BinaryWriter(IBinaryWriter impl) : IBinaryWriter
{
    public long Position
    {
        get => impl.Position;
        set => impl.Position = value;
    }

    public long Length => impl.Length;

#region Primitive types
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void S8(sbyte value)
    {
        impl.S8(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void U8(byte value)
    {
        impl.U8(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void S16(short value)
    {
        impl.S16(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void U16(ushort value)
    {
        impl.U16(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void S32(int value)
    {
        impl.S32(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void U32(uint value)
    {
        impl.U32(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void S64(long value)
    {
        impl.S64(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void U64(ulong value)
    {
        impl.U64(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void F32(float value)
    {
        impl.F32(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void F64(double value)
    {
        impl.F64(value);
    }
#endregion

#region Contiguous memory
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Span(Span<byte> value)
    {
        impl.Span(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Array(byte[] value)
    {
        impl.Array(value);
    }
#endregion

#region Disposal
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Flush()
    {
        impl.Flush();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        impl.Dispose();
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

#region Primitive types
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void S8(sbyte value)
    {
        writer.S8(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void U8(byte value)
    {
        writer.U8(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void S16(short value)
    {
        writer.S16(reverse_bytes ? BinaryPrimitives.ReverseEndianness(value) : value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void U16(ushort value)
    {
        writer.U16(reverse_bytes ? BinaryPrimitives.ReverseEndianness(value) : value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void S32(int value)
    {
        writer.S32(reverse_bytes ? BinaryPrimitives.ReverseEndianness(value) : value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void U32(uint value)
    {
        writer.U32(reverse_bytes ? BinaryPrimitives.ReverseEndianness(value) : value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void S64(long value)
    {
        writer.S64(reverse_bytes ? BinaryPrimitives.ReverseEndianness(value) : value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void U64(ulong value)
    {
        writer.U64(reverse_bytes ? BinaryPrimitives.ReverseEndianness(value) : value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void F32(float value)
    {
        writer.F32(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void F64(double value)
    {
        writer.F64(value);
    }
#endregion

#region Contiguous memory
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
#endregion

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
    public static void BooleanWide(this IBinaryWriter @this, bool value)
    {
        @this.U32(value ? 1u : 0u);
    }

    /// <summary>
    ///     Writes a narrow (8-bit) boolean value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void BooleanNarrow(this IBinaryWriter @this, bool value)
    {
        @this.U8(value ? (byte)1 : (byte)0);
    }
#endregion
}