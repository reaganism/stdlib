using System.IO;

using Reaganism.IO;

using BinaryReader = Reaganism.IO.BinaryReader;
using BinaryWriter = Reaganism.IO.BinaryWriter;

namespace Reaganism.Tests.IO;

[TestFixture]
public static class BinaryReaderWriterTests
{
    [Test]
    public static void TestBasicRead()
    {
        var testData = CreateTestData();

        using var reader = BinaryReader.FromByteArray(testData);

        Assert.Multiple(
            () =>
            {
                Assert.That(reader.Be.Read<uint>(), Is.EqualTo(0x01020304u));
                Assert.That(reader.Le.Read<uint>(), Is.EqualTo(0x08070605u));
            }
        );
    }

    [Test]
    public static void TestBasicWrite()
    {
        using var memStream = new MemoryStream();
        using var writer    = BinaryWriter.FromStream(memStream, disposeStream: true);

        writer.Be.Write(0x11223344u);
        writer.Le.Write(0x55667788u);

        var result = memStream.ToArray();

        Assert.That(
            result,
            Is.EquivalentTo(
                new byte[]
                {
                    0x11, 0x22, 0x33, 0x44,
                    0x88, 0x77, 0x66, 0x55,
                }
            )
        );
    }

    [Test]
    public static void TestEndianness()
    {
        var testData = new byte[] { 0x12, 0x34, 0x56, 0x78 };

        using var reader = BinaryReader.FromByteArray(testData);

        Assert.That(reader.Le.Read<uint>(), Is.EqualTo(0x78563412u));

        reader.Position = 0;
        Assert.That(reader.Be.Read<uint>(), Is.EqualTo(0x12345678u));
    }

    [Test]
    public static void TestBooleanReading()
    {
        var testData = new byte[]
        {
            // LE
            0x01, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,
            0x01,
            0x00,

            // BE
            0x00, 0x00, 0x00, 0x01,
            0x00, 0x00, 0x00, 0x00,
            0x01,
            0x00,
        };

        using var reader = BinaryReader.FromByteArray(testData);

        Assert.Multiple(
            () =>
            {
                Assert.That(reader.Le.BooleanWide(),   Is.True);
                Assert.That(reader.Le.BooleanWide(),   Is.False);
                Assert.That(reader.Le.BooleanNarrow(), Is.True);
                Assert.That(reader.Le.BooleanNarrow(), Is.False);
                Assert.That(reader.Be.BooleanWide(),   Is.True);
                Assert.That(reader.Be.BooleanWide(),   Is.False);
                Assert.That(reader.Be.BooleanNarrow(), Is.True);
                Assert.That(reader.Be.BooleanNarrow(), Is.False);
            }
        );
    }

    [Test]
    public static void TestBooleanWriting()
    {
        using var memStream = new MemoryStream();
        using var writer    = BinaryWriter.FromStream(memStream, disposeStream: true);

        writer.Le.BooleanWide(true);
        writer.Le.BooleanWide(false);
        writer.Le.BooleanNarrow(true);
        writer.Le.BooleanNarrow(false);
        writer.Be.BooleanWide(true);
        writer.Be.BooleanWide(false);
        writer.Be.BooleanNarrow(true);
        writer.Be.BooleanNarrow(false);

        var result = memStream.ToArray();

        Assert.That(
            result,
            Is.EquivalentTo(
                new byte[]
                {
                    // LE
                    0x01, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00,
                    0x01,
                    0x00,

                    // BE
                    0x00, 0x00, 0x00, 0x01,
                    0x00, 0x00, 0x00, 0x00,
                    0x01,
                    0x00,
                }
            )
        );
    }

    [Test]
    public static void TestSpanReading()
    {
        var       testData = CreateTestData();
        using var reader   = BinaryReader.FromByteArray(testData);

        var buffer = new byte[4];
        reader.Span(buffer);

        Assert.That(buffer, Is.EquivalentTo(new byte[] { 0x01, 0x02, 0x03, 0x04 }));
    }

    [Test]
    public static void TestSpanWriting()
    {
        using var memStream = new MemoryStream();
        using var writer    = BinaryWriter.FromStream(memStream, disposeStream: true);

        var data = new byte[] { 0xAA, 0xBB, 0xCC, 0xDD };
        writer.Span(data);

        Assert.That(memStream.ToArray(), Is.EquivalentTo(data));
    }

    [Test]
    public static void TestPositionAndLength()
    {
        var       testData = CreateTestData();
        using var reader   = BinaryReader.FromByteArray(testData);

        Assert.Multiple(
            () =>
            {
                Assert.That(reader.Position, Is.Zero);
                Assert.That(reader.Length,   Is.EqualTo(testData.Length));

                reader.Position = 4;
                Assert.That(reader.Position, Is.EqualTo(4));
            }
        );
    }

    private static byte[] CreateTestData()
    {
        return
        [
            0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
            0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10,
        ];
    }
}