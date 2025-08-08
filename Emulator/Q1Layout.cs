namespace Q1.Emulator;

public static class Q1Layout
{
    // Bus locations
    public static readonly (u16 start, u16 size) Rom     = (0x1FFF, 1024 * 8);
    public static readonly (u16 start, u16 size) Memory  = (0x4FFF, 1024 * 16);
    public static readonly u16                   Display = 0x3FFF;
    
    
    public static readonly u16 StackStart      = Q1Layout.Memory.start;
    public static readonly u16 StackLength     = 256 * 2; // 256 16-bit values
    public static readonly u16 FreeMemoryStart = (u16) (Q1Layout.StackStart + Q1Layout.StackLength);
    public static readonly u16 ProgramStart = Q1Layout.Rom.start;
}