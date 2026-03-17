namespace Q1.core.Arch;

public static class ChipLayout
{
    public const u16 MEMORY_START = 0x0000;
    public const u16 MEMORY_SIZE = 0x4000; // 16KB of memory (0x0000 to 0x3FFF)
    
    public const u16 IVT_START = ChipLayout.MEMORY_START;
    public const u16 IVT_SIZE = 256; // 256 bytes for 128 interrupt vectors (2 bytes each)
    
    public const u16 STACK_START = ChipLayout.IVT_START + ChipLayout.IVT_SIZE;
    public const u16 STACK_SIZE = 256 * 2; // 256 words (512 bytes)
    
    public const u16 PROGRAM_START = ChipLayout.MEMORY_START + ChipLayout.STACK_SIZE;
}