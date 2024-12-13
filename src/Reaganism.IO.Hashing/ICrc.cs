namespace Reaganism.IO.Hashing;

/// <summary>
///     Represents a 32-bit Cyclic Redundancy Check (CRC) algorithm.
/// </summary>
public interface ICrc32
{
    void SetupTable();

    uint Hash(uint crc, byte[] data);
}