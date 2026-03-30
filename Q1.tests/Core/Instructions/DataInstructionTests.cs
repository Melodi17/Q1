namespace Q1.tests.Core.Instructions;

using core.Arch;
using core.Arch.Constants;
using core.Arch.Lookups;
using core.Components;

[TestFixture]
public class DataInstructionTests : InstructionTestBase
{
    [TestCase(0x0000)]
    [TestCase(0xFFFF)]
    [TestCase(0x1234)]
    [TestCase(0xABCD)]
    [TestCase(0x5555)]
    [TestCase(0b1010101010101010)]
    public void InverseDxInstruction_ShouldCorrectlyInvertBits(int value)
    {
        // Arrange
        u16 value16 = (u16) value;

        var chip = this.CreateChip();
        chip.Dx = value16;

        var instruction = InstructionEncoding
            .Encode(
                InstructionSet.GetOpcode("INV(DX)"),
                0,
                0,
                true);

        chip.Bus.WriteWord(chip.Pc, instruction);

        // Act
        chip.Clock();

        // Assert
        u16 expected = (u16) ~value16;
        Assert.That(chip.Dx, Is.EqualTo(expected));
    }

    [TestCase(0x0000)]
    [TestCase(0xFFFF)]
    [TestCase(0x1234)]
    [TestCase(0xABCD)]
    [TestCase(0x5555)]
    [TestCase(0b1010101010101010)]
    public void InverseInstruction_ShouldCorrectlyInvertBits(int value)
    {
        // Arrange
        u16 value16 = (u16) value;

        var chip = this.CreateChip();

        var instruction = InstructionEncoding
            .Encode(
                InstructionSet.GetOpcode("INV"),
                AddressingModes.GetAddressingMode("Immediate"),
                0,
                true);

        chip.Bus.WriteWord(chip.Pc, instruction);
        chip.Bus.WriteWord((u16) (chip.Pc + 2), value16);

        // Act
        chip.Clock();

        // Assert
        u16 expected = (u16) ~value16;
        Assert.That(chip.Dx, Is.EqualTo(expected));
    }

    [TestCase(0b01, 0b10)]
    [TestCase(0b1111, 0b0000)]
    [TestCase(0b1010, 0b0101)]
    [TestCase(0x1234, 0x5678)]
    public void AndInstruction_ShouldCorrectlyAndBits(int value1, int value2)
    {
        // Arrange
        u16 value16_1 = (u16) value1;
        u16 value16_2 = (u16) value2;

        var chip = this.CreateChip();

        var instruction = InstructionEncoding
            .Encode(
                InstructionSet.GetOpcode("AND"),
                AddressingModes.GetAddressingMode("Immediate"),
                AddressingModes.GetAddressingMode("Immediate"),
                true);

        chip.Bus.WriteWord(chip.Pc, instruction);
        chip.Bus.WriteWord((u16) (chip.Pc + 2), value16_1);
        chip.Bus.WriteWord((u16) (chip.Pc + 4), value16_2);

        // Act
        chip.Clock();

        // Assert
        u16 expected = (u16) (value16_1 & value16_2);
        Assert.That(chip.Dx, Is.EqualTo(expected));
    }

    [TestCase(0b01, 0b10)]
    [TestCase(0b1111, 0b0000)]
    [TestCase(0b1010, 0b0101)]
    [TestCase(0x1234, 0x5678)]
    public void OrInstruction_ShouldCorrectlyOrBits(int value1, int value2)
    {
        // Arrange
        u16 value16_1 = (u16) value1;
        u16 value16_2 = (u16) value2;

        var chip = this.CreateChip();

        var instruction = InstructionEncoding
            .Encode(
                InstructionSet.GetOpcode("OR"),
                AddressingModes.GetAddressingMode("Immediate"),
                AddressingModes.GetAddressingMode("Immediate"),
                true);

        chip.Bus.WriteWord(chip.Pc, instruction);
        chip.Bus.WriteWord((u16) (chip.Pc + 2), value16_1);
        chip.Bus.WriteWord((u16) (chip.Pc + 4), value16_2);

        // Act
        chip.Clock();

        // Assert
        u16 expected = (u16) (value16_1 | value16_2);
        Assert.That(chip.Dx, Is.EqualTo(expected));
    }

    [TestCase(0b01, 0b10)]
    [TestCase(0b1111, 0b0000)]
    [TestCase(0b1010, 0b0101)]
    [TestCase(0x1234, 0x5678)]
    public void XorInstruction_ShouldCorrectlyXorBits(int value1, int value2)
    {
        // Arrange
        u16 value16_1 = (u16) value1;
        u16 value16_2 = (u16) value2;

        var chip = this.CreateChip();

        var instruction = InstructionEncoding
            .Encode(
                InstructionSet.GetOpcode("XOR"),
                AddressingModes.GetAddressingMode("Immediate"),
                AddressingModes.GetAddressingMode("Immediate"),
                true);

        chip.Bus.WriteWord(chip.Pc, instruction);
        chip.Bus.WriteWord((u16) (chip.Pc + 2), value16_1);
        chip.Bus.WriteWord((u16) (chip.Pc + 4), value16_2);

        // Act
        chip.Clock();

        // Assert
        u16 expected = (u16) (value16_1 ^ value16_2);
        Assert.That(chip.Dx, Is.EqualTo(expected));
    }

    [Test]
    public void ShiftLeftInPlaceDxInstruction_ShouldCorrectlyShiftBits()
    {
        // Arrange
        u16 value16 = 0b1010101010101010;

        var chip = this.CreateChip();
        chip.Dx = value16;

        var instruction = InstructionEncoding
            .Encode(
                InstructionSet.GetOpcode("SHPL(DX)"),
                0,
                0,
                true);
        chip.Bus.WriteWord(chip.Pc, instruction);

        // Act
        chip.Clock();

        // Assert
        u16 expected = (u16) ((value16 << 1) | (value16 >> 15));
        Assert.That(chip.Dx, Is.EqualTo(expected));
    }
    
    [Test]
    public void ShiftLeftInPlaceInstruction_ShouldCorrectlyShiftBits()
    {
        // Arrange
        u16 value16 = 0b1010101010101010;

        var chip = this.CreateChip();

        var instruction = InstructionEncoding
            .Encode(
                InstructionSet.GetOpcode("SHPL"),
                AddressingModes.GetAddressingMode("Immediate"),
                0,
                true);

        chip.Bus.WriteWord(chip.Pc, instruction);
        chip.Bus.WriteWord((u16) (chip.Pc + 2), value16);

        // Act
        chip.Clock();

        // Assert
        u16 expected = (u16) ((value16 << 1) | (value16 >> 15));
        Assert.That(chip.Dx, Is.EqualTo(expected));
    }
    
    [Test]
    public void ShiftLeftInstruction_ShouldCorrectlyShiftBits()
    {
        // Arrange
        u16 value16 = 0b1010101010101010;
        u16 shiftAmount = 3;

        var chip = this.CreateChip();

        var instruction = InstructionEncoding
            .Encode(
                InstructionSet.GetOpcode("SHL"),
                AddressingModes.GetAddressingMode("Immediate"),
                AddressingModes.GetAddressingMode("Immediate"),
                true);

        chip.Bus.WriteWord(chip.Pc, instruction);
        chip.Bus.WriteWord((u16) (chip.Pc + 2), value16);
        chip.Bus.WriteWord((u16) (chip.Pc + 4), shiftAmount);

        // Act
        chip.Clock();

        // Assert
        u16 expected = (u16) (value16 << shiftAmount);
        Assert.That(chip.Dx, Is.EqualTo(expected));
    }
    
    [Test]
    public void ShiftRightInPlaceDxInstruction_ShouldCorrectlyShiftBits()
    {
        // Arrange
        u16 value16 = 0b1010101010101010;

        var chip = this.CreateChip();
        chip.Dx = value16;

        var instruction = InstructionEncoding
            .Encode(
                InstructionSet.GetOpcode("SHPR(DX)"),
                0,
                0,
                true);

        chip.Bus.WriteWord(chip.Pc, instruction);

        // Act
        chip.Clock();

        // Assert
        u16 expected = (u16) (value16 >> 1);
        Assert.That(chip.Dx, Is.EqualTo(expected));
    }
    
    [Test]
    public void ShiftRightInPlaceInstruction_ShouldCorrectlyShiftBits()
    {
        // Arrange
        u16 value16 = 0b1010101010101010;

        var chip = this.CreateChip();

        var instruction = InstructionEncoding
            .Encode(
                InstructionSet.GetOpcode("SHPL"),
                AddressingModes.GetAddressingMode("Immediate"),
                0,
                true);

        chip.Bus.WriteWord(chip.Pc, instruction);
        chip.Bus.WriteWord((u16) (chip.Pc + 2), value16);

        // Act
        chip.Clock();

        // Assert
        u16 expected = (u16) ((value16 >> 1) | (value16 << 15));
        Assert.That(chip.Dx, Is.EqualTo(expected));
    }
    
    [Test]
    public void ShiftRightInstruction_ShouldCorrectlyShiftBits()
    {
        // Arrange
        u16 value16 = 0b1010101010101010;
        u16 shiftAmount = 3;

        var chip = this.CreateChip();

        var instruction = InstructionEncoding
            .Encode(
                InstructionSet.GetOpcode("SHR"),
                AddressingModes.GetAddressingMode("Immediate"),
                AddressingModes.GetAddressingMode("Immediate"),
                true);

        chip.Bus.WriteWord(chip.Pc, instruction);
        chip.Bus.WriteWord((u16) (chip.Pc + 2), value16);
        chip.Bus.WriteWord((u16) (chip.Pc + 4), shiftAmount);

        // Act
        chip.Clock();

        // Assert
        u16 expected = (u16) (value16 >> shiftAmount);
        Assert.That(chip.Dx, Is.EqualTo(expected));
    }
}