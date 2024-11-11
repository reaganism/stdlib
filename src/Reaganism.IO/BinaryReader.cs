using System;
using System.Buffers.Binary;
using System.Diagnostics;
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
    /// <returns>
    ///     The amount of data filled; may not be the entire span.
    /// </returns>
    int Span(Span<byte> span);

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
public readonly unsafe partial struct BinaryReader(IBinaryReader impl) : IBinaryReader
{
    public long Position
    {
        get => impl.Position;
        set => impl.Position = value;
    }

    public long Length => impl.Length;

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
    public int Span(Span<byte> span)
    {
        AssertSize(span.Length);
        {
            return impl.Span(span);
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
    public int Span(Span<byte> span)
    {
        return reader.Span(span);
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