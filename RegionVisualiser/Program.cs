namespace RegionVisualiser;

using Q1.Emulator;

class Program
{
    static void Main(string[] args)
    {
        Region[] regions =
        [
            new Region
            {
                Name = "ROM",
                Info = $"ProgramStart: 0x{Q1Layout.ProgramStart:X4}",
                Start = Q1Layout.Rom.start,
                Size = Q1Layout.Rom.size
            },
            new Region
            {
                Name = "Memory",
                Info =
                    $"Stack: 0x{Q1Layout.StackStart:X4}-0x{Q1Layout.StackStart + Q1Layout.StackLength:X4}"
                    + $" ({Q1Layout.StackLength}b)\n"
                    + $"Free : 0x{Q1Layout.FreeMemoryStart:X4}",
                Start = Q1Layout.Memory.start,
                Size = Q1Layout.Memory.size
            },
            new Region
            {
                Name = "Display",
                Info = $"Palette   : {Q1Layout.DisplayPaletteSize} colors\n"
                       + $"Offset    : 0x{Q1Layout.Display.start + Q1Layout.DisplayPaletteSize * 3:X4}\n"
                       + $"Dimensions: {Q1Layout.DisplayWidth}x{Q1Layout.DisplayHeight}",
                Start = Q1Layout.Display.start,
                Size = Q1Layout.Display.size
            },
            new Region
            {
                Name = "HID Module",
                Info = $"Mouse IDX  : 0x{Q1Layout.Hid.start:X4}\n"
                       + $"Mouse state: 0x{Q1Layout.Hid.start + 2:X4}\n"
                       + $"Keyboard SK: 0x{Q1Layout.Hid.start + 4:X4}",
                Start = Q1Layout.Hid.start,
                Size = Q1Layout.Hid.size
            },
        ];

        regions = regions.OrderBy(x => x.Start).ToArray();

        int lastRegionEnd = -1;
        for (int i = 0; i < regions.Length; i++)
        {
            Region r = regions[i];
            if (r.Start != lastRegionEnd + 1)
            {
                Console.WriteLine($" 0x{lastRegionEnd + 1:X4} ────" + new string('─', 50));
                Console.WriteLine($"           [ Unmapped / Reserved ({SizeFormatter.HumanReadable(r.Start - lastRegionEnd)}) ]");
            }
            BoxDrawer.DrawBox(
                $"{r.Name} ({SizeFormatter.HumanReadable(r.Size)})",
                r.Info,
                width: 50,
                head: $" 0x{r.Start:X4} ── ");

            lastRegionEnd = r.End;
        }
        
        if (lastRegionEnd + 1 != UInt16.MaxValue)
        {
            Console.WriteLine($" 0x{lastRegionEnd + 1:X4} ────" + new string('─', 50));
            Console.WriteLine($"           [ Unmapped / Reserved ({SizeFormatter.HumanReadable(UInt16.MaxValue - lastRegionEnd)}) ]");
        }
    }
}