namespace Q1.tests.Core;

using core.Arch;

[TestFixture]
public class InstructionEncodingTests
{
    [Test]
    public void EncodeDecode_ShouldRoundTrip()
    {
        u8 opcode = 0x01;
        u8 m1 = 0x5;
        u8 m2 = 0xC;
        bool word = true;

        u16 instruction = InstructionEncoding.Encode(opcode, m1, m2, word);

        InstructionEncoding.Decode(instruction, out var op2, out var m1_2, out var m2_2, out var word2);

        Assert.Multiple(() =>
        {
            Assert.That(op2, Is.EqualTo(opcode));
            Assert.That(m1_2, Is.EqualTo(m1));
            Assert.That(m2_2, Is.EqualTo(m2));
            Assert.That(word2, Is.EqualTo(word));
        });
    }
}