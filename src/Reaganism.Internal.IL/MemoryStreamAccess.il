.module Reaganism.Internal.IL.dll

.namespace Reaganism.Internal.IL
{
    .class public auto ansi abstract sealed beforefieldinit MemoryStreamAccess
        extends [System.Runtime]System.Object
    {
        .method public hidebysig static
            uint8[] GetBuffer (
                class [System.Runtime]System.IO.MemoryStream ms
            ) cil managed aggressiveinlining
        {
            .maxstack 8

            ldarg.0
            ldfld uint8[] [System.Runtime]System.IO.MemoryStream::_buffer
            ret
        }
        
        .method public hidebysig static valuetype [System.Runtime]System.ReadOnlySpan`1<unsigned int8>
            InternalReadSpan(
                class [System.Runtime]System.IO.MemoryStream ms,
                int32 count
            ) cil managed aggressiveinlining
        {
            .maxstack 3
            .locals (
                [0] int32 origPos,
                [1] int32 newPos
            )

        IL_0000: ldarg.0      // this
        IL_0001: call         instance void [System.Runtime]System.IO.MemoryStream::EnsureNotClosed()

        IL_0006: ldarg.0      // this
        IL_0007: ldfld        int32 [System.Runtime]System.IO.MemoryStream::_position
        IL_000c: stloc.0      // origPos

        IL_000d: ldloc.0      // origPos
        IL_000e: ldarg.1      // count
        IL_000f: add
        IL_0010: stloc.1      // newPos

        IL_0011: ldloc.1      // newPos
        IL_0012: ldarg.0      // this
        IL_0013: ldfld        int32 [System.Runtime]System.IO.MemoryStream::_length
        IL_0018: ble.un.s     IL_002b

        IL_001a: ldarg.0      // this
        IL_001b: ldarg.0      // this
        IL_001c: ldfld        int32 [System.Runtime]System.IO.MemoryStream::_length
        IL_0021: stfld        int32 [System.Runtime]System.IO.MemoryStream::_position

        // IL_0026: call         void [System.Private.CoreLib]System.ThrowHelper::ThrowEndOfFileException()
        IL_0026: newobj       instance void [System.Runtime]System.IO.EndOfStreamException::.ctor()
                 throw

        IL_002b: ldarg.0      // this
        IL_002c: ldfld        unsigned int8[] [System.Runtime]System.IO.MemoryStream::_buffer
        IL_0031: ldloc.0      // origPos
        IL_0032: ldarg.1      // count
        IL_0033: newobj       instance void valuetype [System.Runtime]System.ReadOnlySpan`1<unsigned int8>::.ctor(!0/*unsigned int8*/[], int32, int32)

        IL_0038: ldarg.0      // this
        IL_0039: ldloc.1      // newPos
        IL_003a: stfld        int32 [System.Runtime]System.IO.MemoryStream::_position

        IL_003f: ret

        }
    }
}