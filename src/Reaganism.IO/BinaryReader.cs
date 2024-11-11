using System;

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

    /// <summary>
    ///     Reads a span of bytes from the data source.
    /// </summary>
    /// <param name="span">The span to write to.</param>
    /// <returns>
    ///     The amount of data filled; may not be the entire span.
    /// </returns>
    int Span(Span<byte> span);

    /// <summary>
    ///     Reads an array of bytes from the data source.
    /// </summary>
    /// <param name="length">The length of data to read.</param>
    byte[] Array(int length);
}

public readonly unsafe partial struct BinaryReader { }