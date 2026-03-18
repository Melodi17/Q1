namespace Q1.tests.Core;

using core.Arch;
using core.Arch.Constants;

[TestFixture]
public class ChipTests : ChipTestBase
{
    [TestCase(ChipRegisters.AX)]
    [TestCase(ChipRegisters.DX)]
    [TestCase(ChipRegisters.LX)]
    public void Register_ShouldRoundTrip(u8 register)
    {
        var chip = this.CreateChip();

        u16 value = (u16) (register * 0x1111); // unique value for each register
        chip.SetRegister(register, value);
        u16 readValue = chip.GetRegister(register);

        Assert.That(readValue, Is.EqualTo(value), $"Register {register:X2} should round trip");
    }

    [Test]
    public void PushPop_ShouldRoundTrip()
    {
        var chip = this.CreateChip();

        u16 value = 0xABCD;
        chip.Push(value);
        u16 poppedValue = chip.Pop();

        Assert.That(poppedValue, Is.EqualTo(value), "Popped value should match pushed value");
    }

    [Test]
    public void StackOverflow_ShouldThrow()
    {
        var chip = this.CreateChip();

        // Fill the stack to its limit
        for (int i = 0; i < ChipLayout.STACK_COUNT; i++)
            chip.Push((u16) i);

        // Next push should cause overflow
        Assert.Throws<InvalidOperationException>(() => chip.Push(0xFFFF), "Expected stack overflow to throw");
    }

    [Test]
    public void StackUnderflow_ShouldThrow()
    {
        var chip = this.CreateChip();
        
        // Attempt to pop from an empty stack
        Assert.Throws<InvalidOperationException>(() => chip.Pop(), "Expected stack underflow to throw");
    }
    
    [Test]
    public void InvalidRegisterAccess_ShouldThrow()
    {
        var chip = this.CreateChip();
        
        // Attempt to access an invalid register index
        Assert.Throws<InvalidOperationException>(() => chip.GetRegister(0xFF), "Expected invalid register access to throw");
        Assert.Throws<InvalidOperationException>(() => chip.SetRegister(0xFF, 0x1234), "Expected invalid register access to throw");
    }
    
    [Test]
    public void InterruptHandling_ShouldJumpToInterruptVector()
    {
        var chip = this.CreateChip();

        var originalPc = chip.Pc;
        
        // Set up an interrupt vector at 0xFFFE
        u16 interruptHandlerAddress = 0x1234;
        chip.RegisterInterruptHandler(8, interruptHandlerAddress); // Register handler for interrupt 8
        
        // Trigger the interrupt
        chip.Interrupt(8);
        
        Assert.Multiple(() =>
        {
            Assert.That(chip.Pc, Is.EqualTo(interruptHandlerAddress), "PC should jump to interrupt handler address");
            Assert.That(chip.Pop(), Is.EqualTo(originalPc), "PC should be pushed onto the stack during interrupt");
        });
    }
}