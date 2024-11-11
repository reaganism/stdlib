using System;

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

    /// <summary>
    ///     Writes a span of bytes to the data source.
    /// </summary>
    void Span(Span<byte> value);

    /// <summary>
    ///     Writes an array of bytes to the data source.
    /// </summary>
    void Array(byte[] bytes);

    /// <summary>
    ///     Flushes the writer.
    /// </summary>
    void Flush();
}

public readonly unsafe partial struct BinaryWriter { }