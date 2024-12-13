.assembly extern System.Runtime
{
    .publickeytoken = ( B0 3F 5F 7F 11 D5 0A 3A )
    .ver 9:0:0:0
}

// .assembly extern System.Private.CoreLib
// {
//     .publickeytoken = ( 7C EC 85 D7 BE A7 79 8E )
//     .ver 4:0:0:0
// }

.assembly Reaganism.Internal.IL
{
    .custom instance void System.Runtime.CompilerServices.IgnoresAccessChecksToAttribute::.ctor(string) = (
        // System.Runtime
        01 00 0e 53 79 73 74 65 6d 2e 52 75 6e 74 69 6d
        65 00 00
    )
    
    .custom instance void System.Runtime.CompilerServices.IgnoresAccessChecksToAttribute::.ctor(string) = (
        // System.Private.CoreLib
        01 00 16 53 79 73 74 65 6d 2e 50 72 69 76 61 74
        65 2e 43 6f 72 65 4c 69 62 00 00
    )

    .ver 1:0:0:0
}

.namespace System.Runtime.CompilerServices
{
    .class public auto ansi sealed beforefieldinit IgnoresAccessChecksToAttribute
        extends [System.Runtime]System.Attribute
    {
        .field private string assemblyName

        .method public hidebysig specialname rtspecialname 
            instance void .ctor (
                string assemblyName
            ) cil managed
        {
            .maxstack 8

            ldarg.0
            ldarg.1
            stfld string System.Runtime.CompilerServices.IgnoresAccessChecksToAttribute::assemblyName
            ret
        }

        .property instance string AssemblyName()
        {
            .get instance string System.Runtime.CompilerServices.IgnoresAccessChecksToAttribute::get_AssemblyName()
        }

        .method public hidebysig specialname instance string get_AssemblyName () cil managed
        {
            .maxstack 8

            ldarg.0
            ldfld string System.Runtime.CompilerServices.IgnoresAccessChecksToAttribute::assemblyName
            ret
        }
    }
}
