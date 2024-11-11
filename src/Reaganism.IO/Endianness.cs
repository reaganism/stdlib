using System;

namespace Reaganism.IO;

/// <summary>
///     Denotes byte order.
/// </summary>
public enum Endianness
{
    /// <summary>
    ///     Big-endian byte order; most significant byte first.
    /// </summary>
    Big,

    /// <summary>
    ///     Little-endian byte order; least significant byte first.
    /// </summary>
    Little,
}

// The following provider is intended to be used in cases where generics are
// replicating C++-like templates.  Useful if you want to shake should-be-dead
// code paths (the JIT should be able to handle this):
//     void Example<TEndianness>() where TEndianness : IEndianProvider {
//         if (TEndianness.Endianness == Endianness.Big) {
//             // 1
//         }
//         else
//         {
//             // 2
//         }
//     }
//
//     // Should ideally get JIT-ed to only have code path 1 or 2 depending on
//     // the system.  I've yet to confirm...
//     Example<SystemEndian>();

/// <summary>
///     Provides a static property for determining endianness.
/// </summary>
public interface IEndianProvider
{
    /// <summary>
    ///     The endianness of the provider.
    /// </summary>
    static abstract Endianness Endianness { get; }
}

/// <summary>
///     Big-endian provider.
/// </summary>
public readonly struct BigEndian : IEndianProvider
{
    static Endianness IEndianProvider.Endianness => Endianness.Big;
}

/// <summary>
///     Little-endian provider.
/// </summary>
public readonly struct LittleEndian : IEndianProvider
{
    static Endianness IEndianProvider.Endianness => Endianness.Little;
}

/// <summary>
///     System endianness provider.  Uses the default, system-dependent
///     endianness.
/// </summary>
public readonly struct SystemEndian : IEndianProvider
{
    static Endianness IEndianProvider.Endianness { get; } = BitConverter.IsLittleEndian ? Endianness.Little : Endianness.Big;
}