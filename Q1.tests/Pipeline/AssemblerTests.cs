namespace Q1.tests.Pipeline;

using core.Arch;
using core.Arch.Constants;
using core.Arch.Lookups;
using core.Pipeline;

[TestFixture]
public class AssemblerTests
{
    private Assembler CreateAssembler()
    {
        return new Assembler();
    }
    
    private byte[] Assemble(string source)
    {
        var assembler = this.CreateAssembler();
        var reader = new StringReader(source);
        return assembler.Assemble(reader).ToArray();
    }
    
    [Test]
    public void Assembler_ShouldAssembleSimpleProgram()
    {
        // Arrange
        string source = """
                        mov 0x1234, ax
                        """;
        
        // Act
        var result = Assemble(source);
        
        // Assert
        MemoryStream expected = new MemoryStream();
        WriteWord(expected, InstructionEncoding.Encode(
            InstructionSet.GetOpcode("MOV"),
            AddressingModes.GetAddressingMode("Immediate"),
            AddressingModes.GetAddressingMode("Register"),
            true));
        WriteWord(expected, 0x1234);
        WriteByte(expected, ChipRegisters.AX);
        
        Assert.That(result, Is.EqualTo(expected.ToArray()));
    }
    
    private void WriteByte(MemoryStream stream, byte value)
    {
        stream.WriteByte(value);
    }
    
    private void WriteWord(MemoryStream stream, ushort value)
    {
        u8 high = (u8) (value       & 0xFF);
        u8 low = (u8) ((value >> 8) & 0xFF);
        stream.WriteByte(low);
        stream.WriteByte(high);
    }
}