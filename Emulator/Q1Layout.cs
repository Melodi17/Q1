namespace Q1.Emulator;

using Chip;

public static class Q1Layout
{
    public static readonly u16 DisplayWidth = 128;
    public static readonly u16 DisplayHeight = 64;
    public static readonly u16 DisplayPaletteSize = 16;
 
    // Bus locations
    public static readonly (u16 start, u16 size) Rom = (0x1FFF, 1024    * 8);
    public static readonly (u16 start, u16 size) Hid = (0x602F, HIDDevice.CalculateSize());
    public static readonly (u16 start, u16 size) Memory = (0x6FFF, 1024 * 16);
    public static readonly (u16 start, u16 size) Display = (0x3FFF, 
        DisplayDevice.DetermineSize(Q1Layout.DisplayPaletteSize, Q1Layout.DisplayWidth, Q1Layout.DisplayHeight));


    public static readonly u16 StackStart = Q1Layout.Memory.start;
    public static readonly u16 StackLength = 256 * 2; // 256 16-bit values
    public static readonly u16 FreeMemoryStart = (u16) (Q1Layout.StackStart + Q1Layout.StackLength);
    public static readonly u16 ProgramStart = Q1Layout.Rom.start;
}