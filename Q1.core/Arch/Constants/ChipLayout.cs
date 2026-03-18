namespace Q1.core.Arch.Constants;

public static class ChipLayout
{
    public const u16 MEMORY_START = 0x0000;
    public const u16 MEMORY_SIZE = 0x4000; // 16KB of memory (0x0000 to 0x3FFF)
    
    public const u16 IVT_START = ChipLayout.MEMORY_START;
    public const u16 IVT_COUNT = 128; // 128 interrupt vectors
    public const u16 IVT_SIZE = ChipLayout.IVT_COUNT * 2; // Each vector is 2 bytes (u16)
    
    public const u16 STACK_START = ChipLayout.IVT_START + ChipLayout.IVT_SIZE;
    public const u16 STACK_COUNT = 256;
    public const u16 STACK_SIZE = ChipLayout.STACK_COUNT * 2; // Each stack entry is 2 bytes (u16)
    
    public const u16 PROGRAM_START = ChipLayout.STACK_START + ChipLayout.STACK_SIZE;
}