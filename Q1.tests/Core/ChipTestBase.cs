namespace Q1.tests.Core;

using Q1.core.Arch;
using Q1.core.Arch.Constants;
using Q1.core.Components;

public abstract class ChipTestBase
{
    protected Chip CreateChip()
    {
        var chip = new Chip();
        
        var memory = new Memory(ChipLayout.MEMORY_START, ChipLayout.MEMORY_SIZE);
        var bus = new Bus(0x0, u16.MaxValue);
        bus.MountDevice(memory);
        
        bus.AttachChip(chip);
        return chip;
    }
}