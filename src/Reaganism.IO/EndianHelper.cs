using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Reaganism.IO;

internal static class EndianHelper
{
    /// <summary>
    ///     Reverses a primitive value by performing an endianness swap of the
    ///     specified <typeparamref name="T"/> value.
    /// </summary>
    /// <param name="value">The value to reverse.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>The reversed value.</returns>
    /// <remarks>
    ///     If the specified <typeparamref name="T"/> is not a primitive type
    ///     (specifically, not implemented by <see cref="BinaryPrimitives"/>),
    ///     the value will be returned as-is.  This is intentional, and is
    ///     designed to allow methods to unconditionally call this method
    ///     without having to check if the type is a supported primitive type.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ReverseEndianness<T>(T value) where T : unmanaged
    {
        if (typeof(T) == typeof(ushort))
        {
            return (T)(object)BinaryPrimitives.ReverseEndianness((ushort)(object)value);
        }

        if (typeof(T) == typeof(short))
        {
            return (T)(object)BinaryPrimitives.ReverseEndianness((short)(object)value);
        }

        if (typeof(T) == typeof(uint))
        {
            return (T)(object)BinaryPrimitives.ReverseEndianness((uint)(object)value);
        }

        if (typeof(T) == typeof(int))
        {
            return (T)(object)BinaryPrimitives.ReverseEndianness((int)(object)value);
        }

        if (typeof(T) == typeof(ulong))
        {
            return (T)(object)BinaryPrimitives.ReverseEndianness((ulong)(object)value);
        }

        if (typeof(T) == typeof(long))
        {
            return (T)(object)BinaryPrimitives.ReverseEndianness((long)(object)value);
        }

        if (typeof(T) == typeof(UInt128))
        {
            return (T)(object)BinaryPrimitives.ReverseEndianness((UInt128)(object)value);
        }

        if (typeof(T) == typeof(Int128))
        {
            return (T)(object)BinaryPrimitives.ReverseEndianness((Int128)(object)value);
        }

        // We could do a slow path that reverses the bytes manually, but instead
        // we'll opt to not reverse at all.  This allows us to unconditionally
        // transfer control to this method even if a user-defined type, a float,
        // or something similar is passed.
        return value;
    }
}