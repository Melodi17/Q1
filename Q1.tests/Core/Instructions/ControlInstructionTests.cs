namespace Q1.tests.Core.Instructions;

using core;
using core.Arch;
using core.Arch.Constants;
using core.Arch.Lookups;
using core.Components;

public class ControlInstructionTests : InstructionTestBase
{
    [Test]
    public void JumpInstruction_JumpsToAddress()
    {
        // Arrange
        var chip = CreateChip();

        var instruction = InstructionEncoding
            .Encode(
                InstructionSet.GetOpcode("JMP"),
                AddressingModes.GetAddressingMode("Immediate"),
                0,
                true);

        u16 jumpAddress = 0x1234;

        chip.Bus.WriteWord(chip.Pc, instruction);
        chip.Bus.WriteWord((u16) (chip.Pc + 2), jumpAddress);

        // Act
        chip.Clock();

        // Assert
        Assert.That(
            chip.Pc,
            Is.EqualTo(jumpAddress),
            "PC should jump to the specified address");
    }

    [Test]
    public void HaltInstruction_HaltsExecution()
    {
        // Arrange
        var chip = CreateChip();

        var instruction = InstructionEncoding
            .Encode(
                InstructionSet.GetOpcode("HALT"),
                0,
                1,
                true);

        chip.Bus.WriteWord(chip.Pc, instruction);

        // Act
        var clock = () => chip.Clock();

        // Assert
        Assert.That(clock, Throws.TypeOf<HaltException>());
    }


    [Test]
    public void ReturnInstruction_ReturnsFromSubroutine()
    {
        // Arrange
        var chip = CreateChip();

        // Simulate a call by pushing a return address onto the stack
        u16 returnAddress = 0x5678;
        chip.Push(returnAddress);

        var instruction = InstructionEncoding
            .Encode(
                InstructionSet.GetOpcode("RET"),
                0,
                0,
                true);

        chip.Bus.WriteWord(chip.Pc, instruction);

        // Act
        chip.Clock();

        // Assert
        Assert.That(
            chip.Pc,
            Is.EqualTo(returnAddress),
            "PC should return to the address on top of the stack");
    }


    [Test]
    public void CallInstruction_CallsSubroutine()
    {
        // Arrange
        var chip = CreateChip();

        u16 pc = chip.Pc;
        u16 subroutineAddress = 0x9ABC;

        var instruction = InstructionEncoding
            .Encode(
                InstructionSet.GetOpcode("CALL"),
                AddressingModes.GetAddressingMode("Immediate"),
                0,
                true);

        chip.Bus.WriteWord(pc, instruction);
        chip.Bus.WriteWord((u16) (pc + 2), subroutineAddress);

        u16 expectedReturnAddress = (u16) (pc + 4);

        // Act
        chip.Clock();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(
                chip.Pc,
                Is.EqualTo(subroutineAddress),
                "PC should jump to the subroutine address");

            Assert.That(
                chip.Bus.ReadWord(ChipLayout.STACK_START),
                Is.EqualTo(expectedReturnAddress),
                "Return address should be pushed onto the stack");
        });
    }


    [Test]
    public void SuspendInstruction_Fails()
    {
        // Arrange
        var chip = CreateChip();

        var instruction = InstructionEncoding
            .Encode(
                InstructionSet.GetOpcode("SUS"),
                0,
                1,
                true);

        chip.Bus.WriteWord(chip.Pc, instruction);

        // Act
        var clock = () => chip.Clock();

        // Assert
        Assert.That(clock, Throws.TypeOf<NotImplementedException>());
    }


    [TestCase(0, true)]
    [TestCase(1, false)]
    [TestCase(5, false)]
    public void BranchIfZeroLx_BranchesCorrectly(i32 lxValue, bool shouldBranch)
    {
        // Arrange
        var chip = CreateChip();

        u16 pc = chip.Pc;

        var instruction = InstructionEncoding
            .Encode(
                InstructionSet.GetOpcode("BZ(LX)"),
                0,
                0,
                true);

        chip.Bus.WriteWord(pc, instruction);
        chip.Lx = (u16) lxValue;

        u16 nextInstructionAddress = (u16) (pc            + 2); // Address of the next instruction
        u16 branchAddress = (u16) (nextInstructionAddress + 2); // Finish current instruction and skip next one

        // Act
        chip.Clock();

        // Assert
        if (shouldBranch)
        {
            Assert.That(
                chip.Pc,
                Is.EqualTo(branchAddress),
                "PC should skip the next instruction when Lx is zero");
        }
        else
        {
            Assert.That(
                chip.Pc,
                Is.EqualTo(nextInstructionAddress),
                "PC should proceed to the next instruction when Lx is non-zero");
        }
    }

    [TestCase(0, true)]
    [TestCase(1, false)]
    [TestCase(5, false)]
    public void BranchIfZero_BranchesCorrectly(int value, bool shouldBranch)
    {
        // Arrange
        var chip = CreateChip();

        u16 pc = chip.Pc;

        var instruction = InstructionEncoding
            .Encode(
                InstructionSet.GetOpcode("BZ"),
                AddressingModes.GetAddressingMode("Immediate"),
                0,
                true);

        chip.Bus.WriteWord(pc, instruction);
        chip.Bus.WriteWord((u16) (pc + 2), (u16) value);

        u16 nextInstructionAddress = (u16) (pc            + 4); // Address of the instruction after the next one
        u16 branchAddress = (u16) (nextInstructionAddress + 2); // Finish current instruction and skip next one

        // Act
        chip.Clock();

        // Assert
        if (shouldBranch)
        {
            Assert.That(
                chip.Pc,
                Is.EqualTo(branchAddress),
                "PC should skip the next instruction when the value is zero");
        }
        else
        {
            Assert.That(
                chip.Pc,
                Is.EqualTo(nextInstructionAddress),
                "PC should proceed to the next instruction when the value is non-zero");
        }
    }

    [Test]
    public void Branch_SkipsNextInstruction()
    {
        // Arrange
        var chip = CreateChip();

        u16 pc = chip.Pc;

        var instruction = InstructionEncoding
            .Encode(
                InstructionSet.GetOpcode("BR"),
                0,
                0,
                true);

        chip.Bus.WriteWord(pc, instruction);

        u16 nextInstructionAddress = (u16) (pc            + 2); // Address of the next instruction
        u16 branchAddress = (u16) (nextInstructionAddress + 2); // Finish current instruction and skip next one

        // Act
        chip.Clock();

        // Assert
        Assert.That(
            chip.Pc,
            Is.EqualTo(branchAddress),
            "PC should skip the next instruction unconditionally");
    }

    [Test]
    public void PushInstruction_PushesValueOntoStack()
    {
        // Arrange
        var chip = CreateChip();

        u16 pc = chip.Pc;
        u16 valueToPush = 0xABCD;

        var instruction = InstructionEncoding
            .Encode(
                InstructionSet.GetOpcode("PUSH"),
                AddressingModes.GetAddressingMode("Immediate"),
                0,
                true);

        chip.Bus.WriteWord(pc, instruction);
        chip.Bus.WriteWord((u16) (pc + 2), valueToPush);

        // Act
        chip.Clock();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(
                chip.Bus.ReadWord(ChipLayout.STACK_START),
                Is.EqualTo(valueToPush),
                "Value should be pushed onto the stack");

            Assert.That(
                chip.Sp,
                Is.EqualTo((u16) (ChipLayout.STACK_START + 2)),
                "Stack pointer should move up by 2 bytes after push");
        });
    }

    [Test]
    public void PopInstruction_PopsValueFromStack()
    {
        // Arrange
        var chip = CreateChip();

        u16 pc = chip.Pc;
        u16 valueToPop = 0xABCD;

        // Simulate a push by writing a value to the stack and moving the stack pointer
        chip.Bus.WriteWord(ChipLayout.STACK_START, valueToPop);
        chip.Sp = ChipLayout.STACK_START + 2;

        var instruction = InstructionEncoding
            .Encode(
                InstructionSet.GetOpcode("POP"),
                AddressingModes.GetAddressingMode("Register"),
                0,
                true);

        chip.Bus.WriteWord(pc, instruction);
        chip.Bus.Write((u16) (pc + 2), ChipRegisters.DX);

        // Act
        chip.Clock();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(
                chip.Dx,
                Is.EqualTo(valueToPop),
                "Value should be popped from the stack into the register");

            Assert.That(
                chip.Sp,
                Is.EqualTo(ChipLayout.STACK_START),
                "Stack pointer should move down by 2 bytes after pop");
        });
    }

    [Test]
    public void PushPopSequence_WorksCorrectly()
    {
        // Arrange
        var chip = CreateChip();

        u16[] valuesToPush = [0x1111, 0x2222, 0x3333];

        void PushValue(u16 value)
        {
            var instruction = InstructionEncoding
                .Encode(
                    InstructionSet.GetOpcode("PUSH"),
                    AddressingModes.GetAddressingMode("Immediate"),
                    0,
                    true);

            chip.Bus.WriteWord(chip.Pc, instruction);
            chip.Bus.WriteWord((u16) (chip.Pc + 2), value);
            chip.Clock();
        }

        u16 PopValue()
        {
            var instruction = InstructionEncoding
                .Encode(
                    InstructionSet.GetOpcode("POP"),
                    AddressingModes.GetAddressingMode("Register"),
                    0,
                    true);

            chip.Bus.WriteWord(chip.Pc, instruction);
            chip.Bus.Write((u16) (chip.Pc + 2), ChipRegisters.DX);

            chip.Clock();

            return chip.Dx;
        }

        // Act & Assert
        // Push 1 item, then pop it, then push 2 items, then pop them
        Assert.Multiple(() =>
        {
            PushValue(valuesToPush[0]);
            Assert.That(
                PopValue(),
                Is.EqualTo(valuesToPush[0]),
                "First popped value should match the first pushed value");

            PushValue(valuesToPush[1]);
            PushValue(valuesToPush[2]);
            Assert.That(
                PopValue(),
                Is.EqualTo(valuesToPush[2]),
                "Second popped value should match the second pushed value");
            Assert.That(
                PopValue(),
                Is.EqualTo(valuesToPush[1]),
                "Third popped value should match the first pushed value");
        });
    }

    [Test]
    public void InterruptInstruction_TriggersInterrupt()
    {
        // Arrange
        var chip = CreateChip();

        var instruction = InstructionEncoding
            .Encode(
                InstructionSet.GetOpcode("INT"),
                AddressingModes.GetAddressingMode("Immediate"),
                0,
                true);

        u8 interruptNumber = 8;

        chip.Bus.WriteWord(chip.Pc, instruction);
        chip.Bus.WriteWord((u16) (chip.Pc + 2), interruptNumber);
        
        u16 expectedReturnPc = (u16) (chip.Pc + 4); // After the INT instruction and its operand
        u16 handlerAddress = 0x2345;
        
        chip.RegisterInterruptHandler(interruptNumber, handlerAddress);

        // Act
        chip.Clock();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(
                chip.Pc,
                Is.EqualTo(handlerAddress),
                "PC should jump to the interrupt handler address");

            Assert.That(
                chip.Bus.ReadWord(ChipLayout.STACK_START),
                Is.EqualTo(expectedReturnPc),
                "PC should be pushed onto the stack during interrupt");
        });
    }
}