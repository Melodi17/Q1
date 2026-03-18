namespace Q1.tests.Core;

using core;
using core.Arch;
using core.Arch.Lookups;

[TestFixture]
public class InstructionShapeTests
{

    [Test]
    public void LookupShouldHaveInstructions()
    {
        var populatedSets = InstructionSet.Lookup
            .Where(x => x != default)
            .ToArray();
        
        Assert.That(populatedSets, Has.Length.EqualTo(19), 
            "Expected 19 instruction groups to be defined");
        
        Assert.That(InstructionSet.Lookup.Length, Is.EqualTo(128),
            "Expected instruction lookup to have 128 entries");
    }
    
    [TestCase(0x00, "NOP", "JMP", "HALT", "MOV")]
    [TestCase(0x01, "RET", "CALL", "SUS", "NOP")]
    [TestCase(0x02, "BZ(LX)", "BZ", "BR", "NOP")]
    [TestCase(0x03, "NOP", "PUSH", "NOP", "NOP")]
    [TestCase(0x04, "NOP", "POP", "NOP", "NOP")]
    [TestCase(0x05, "NOP", "INT", "NOP", "NOP")]
    
    [TestCase(0x10, "INV(DX)", "INV", "NOP", "AND")]
    [TestCase(0x11, "NOP", "NOP", "NOP", "OR")]
    [TestCase(0x12, "NOP", "NOP", "NOP", "XOR")]
    [TestCase(0x13, "SHPL(DX)", "SHPL", "NOP", "SHL")]
    [TestCase(0x14, "SHPR(DX)", "SHPR", "NOP", "SHR")]
    
    [TestCase(0x20, "NOT(LX)", "NOT", "NOP", "CMP")]
    [TestCase(0x21, "NOP", "NOP", "NOP", "LT")]
    [TestCase(0x22, "NOP", "NOP", "NOP", "GT")]
    
    [TestCase(0x30, "INC(AX)", "INC", "NOP", "ADD")]
    [TestCase(0x31, "DEC(AX)", "DEC", "NOP", "SUB")]
    [TestCase(0x32, "NOP", "NOP", "NOP", "DIV")]
    [TestCase(0x33, "NOP", "NOP", "NOP", "MUL")]
    [TestCase(0x34, "NOP", "NOP", "NOP", "MOD")]
    public void GroupLooksLike(i32 opcode, string impl, string simple, string extendedImpl, string extended)
    {
        var group = InstructionSet.Lookup[opcode];
        
        Assert.That(group.Implicit.Name, Is.EqualTo(impl),
            $"Expected implicit instruction for opcode {opcode:X2} to be {impl}");
        
        Assert.That(group.SimpleAddressing.Name, Is.EqualTo(simple),
            $"Expected simple addressing instruction for opcode {opcode:X2} to be {simple}");
        
        Assert.That(group.ExtendedImplicit.Name, Is.EqualTo(extendedImpl),
            $"Expected extended implicit instruction for opcode {opcode:X2} to be {extendedImpl}");
        
        Assert.That(group.ExtendedAddressing.Name, Is.EqualTo(extended),
            $"Expected extended addressing instruction for opcode {opcode:X2} to be {extended}");
    }
}