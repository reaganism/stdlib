# stdlib

**stdlib** (`Reaganism.Standard`, "The Library") is a collection of standard utilities used by reaganism projects and some others.

## io

The Library includes utilities for handling input/output.

### binary reading and writing

Centralized `BinaryReader` and `BinaryWriter` types exist that implement their respective `IBinaryReader/Writer` interfaces.
These types have `Be` and `Le` properties that return temporary instances on the stack that can specifically read and write data
in big and little endian form.

Main primitives are read/written with single `Read<T>()`/`Write<T>(T value)` APIs. Contiguous memory may be handled with `Array`
and `Span` APIs.

These types attempt to maximize performance and minimize virtualization while still allowing for an arbitrary data source.
Implementations of `IBinaryReader/Writer` are implemented with minimal safety checks (bounds, etc.) as they're handled by their
wrapping types. The big and little endian APIs are done efficiently by using the JIT to inline the correct code paths and remove
any branching.

