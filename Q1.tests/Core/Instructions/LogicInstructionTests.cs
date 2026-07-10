namespace Q1.tests.Core.Instructions;

using core.Arch;
using core.Arch.Lookups;
using core.Components;

[TestFixture]
public class LogicInstructionTests : InstructionTestBase
{
    [TestCase(0x0000, 0x01)]
    [TestCase(0x0001, 0x00)]
    [TestCase(0x0005, 0x00)]
    public void NotLxInstruction_PerformsNot(i32 input, i32 output)
    {
        // Arrange
        var chip = this.CreateChip();
        chip.Lx = (u16) input;

        var instruction = InstructionEncoding
            .Encode(
                InstructionSet.GetOpcode("NOT(LX)"),
                0,
                0,
                true);

        chip.Bus.WriteWord(chip.Pc, instruction);

        // Act
        chip.Clock();

        // Assert
        u16 expected = (u16) output;
        Assert.That(chip.Lx, Is.EqualTo(expected));
    }
    
    [TestCase(0x0000, 0x01)]
    [TestCase(0x0001, 0x00)]
    [TestCase(0x0005, 0x00)]
    public void NotInstruction_PerformsNot(i32 input, i32 output)
    {
        // Arrange
        var chip = this.CreateChip();

        var instruction = InstructionEncoding
            .Encode(
                InstructionSet.GetOpcode("NOT"),
                AddressingModes.GetAddressingMode("Immediate"),
                0,
                true);

        chip.Bus.WriteWord(chip.Pc, instruction);
        chip.Bus.WriteWord((u16) (chip.Pc + 2), (u16) input);

        // Act
        chip.Clock();

        // Assert
        u16 expected = (u16) output;
        Assert.That(chip.Lx, Is.EqualTo(expected));
    }
    
    [TestCase(0x0000, 0x0000, 0x01)]
    [TestCase(0x0000, 0x0001, 0x00)]
    [TestCase(0x0001, 0x0000, 0x00)]
    [TestCase(0x0001, 0x0001, 0x01)]
    [TestCase(0x1234, 0x5678, 0x00)]
    [TestCase(0x8342, 0x8342, 0x01)]
    public void CompareInstruction_ComparesCorrectly(i32 a, i32 b, i32 expected)
    {
        // Arrange
        var chip = this.CreateChip();

        var instruction = InstructionEncoding
            .Encode(
                InstructionSet.GetOpcode("CMP"),
                AddressingModes.GetAddressingMode("Immediate"),
                AddressingModes.GetAddressingMode("Immediate"),
                true);

        chip.Bus.WriteWord(chip.Pc, instruction);
        chip.Bus.WriteWord((u16) (chip.Pc + 2), (u16) a);
        chip.Bus.WriteWord((u16) (chip.Pc + 4), (u16) b);

        // Act
        chip.Clock();

        // Assert
        u16 expected16 = (u16) expected;
        Assert.That(chip.Lx, Is.EqualTo(expected16));
    }
    
    [TestCase(0x0000, 0x0000, 0x00)]
    [TestCase(0x0000, 0x0001, 0x01)]
    [TestCase(0x0001, 0x0000, 0x00)]
    [TestCase(0x0001, 0x0001, 0x00)]
    [TestCase(0x1234, 0x5678, 0x01)]
    [TestCase(0x8342, 0x1234, 0x00)]
    public void LessThanInstruction_ComparesCorrectly(i32 a, i32 b, i32 expected)
    {
        // Arrange
        var chip = this.CreateChip();

        var instruction = InstructionEncoding
            .Encode(
                InstructionSet.GetOpcode("LT"),
                AddressingModes.GetAddressingMode("Immediate"),
                AddressingModes.GetAddressingMode("Immediate"),
                true);

        chip.Bus.WriteWord(chip.Pc, instruction);
        chip.Bus.WriteWord((u16) (chip.Pc + 2), (u16) a);
        chip.Bus.WriteWord((u16) (chip.Pc + 4), (u16) b);

        // Act
        chip.Clock();

        // Assert
        u16 expected16 = (u16) expected;
        Assert.That(chip.Lx, Is.EqualTo(expected16));
    }
    
    [TestCase(0x0000, 0x0000, 0x00)]
    [TestCase(0x0000, 0x0001, 0x00)]
    [TestCase(0x0001, 0x0000, 0x01)]
    [TestCase(0x0001, 0x0001, 0x00)]
    [TestCase(0x5678, 0x1234, 0x01)]
    [TestCase(0x1234, 0x8342, 0x00)]
    public void GreaterThanInstruction_ComparesCorrectly(i32 a, i32 b, i32 expected)
    {
        // Arrange
        var chip = this.CreateChip();

        var instruction = InstructionEncoding
            .Encode(
                InstructionSet.GetOpcode("GT"),
                AddressingModes.GetAddressingMode("Immediate"),
                AddressingModes.GetAddressingMode("Immediate"),
                true);

        chip.Bus.WriteWord(chip.Pc, instruction);
        chip.Bus.WriteWord((u16) (chip.Pc + 2), (u16) a);
        chip.Bus.WriteWord((u16) (chip.Pc + 4), (u16) b);

        // Act
        chip.Clock();

        // Assert
        u16 expected16 = (u16) expected;
        Assert.That(chip.Lx, Is.EqualTo(expected16));
    }
}