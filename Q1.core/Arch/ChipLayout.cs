namespace Q1.core.Arch;

public static class ChipLayout
{
    public const u16 MEMORY_START = 0x0000;
    public const u16 MEMORY_SIZE = 0xFF00; // 64KB - 256 bytes reserved for stack
    
    public const u16 STACK_START = ChipLayout.MEMORY_START;
    public const u16 STACK_SIZE = 256 * 2; // 256 words (512 bytes)
    
    public const u16 PROGRAM_START = ChipLayout.MEMORY_START + ChipLayout.STACK_SIZE;
}