using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Reaganism.Internal;

public static class MemoryStreamAccess
{
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public static byte[] GetBuffer(MemoryStream stream)
    // {
    //     return IL.MemoryStreamAccess.GetBuffer(stream);
    // }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<byte> InternalReadSpan(MemoryStream ms, int count)
    {
        return IL.MemoryStreamAccess.InternalReadSpan(ms, count);
    }
}