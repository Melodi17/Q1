namespace Q1.Emulator;

using System;
using System.IO;
using System.Threading;
using Chip;
using CommandLine;
using Microsoft.Xna.Framework;

public class Program
{
    public static void Main(string[] args)
    {
        Parser.Default.ParseArguments<EmulatorOptions>(args).WithParsed(Program.Main);
    }

    public static void Main(EmulatorOptions emulatorOptions)
    {
        Q1Cpu cpu = new();
        Bus bus = new(u16.MinValue, u16.MaxValue);
        RamDevice rom = new(Q1Layout.Rom.start, Q1Layout.Rom.size);
        File.ReadAllBytes(emulatorOptions.InputFile).CopyTo(rom.Memory, 0);
        
        DisplayDevice display = new(Q1Layout.DisplayPaletteSize, Q1Layout.DisplayWidth, Q1Layout.DisplayHeight, Q1Layout.Display.start);
        display.SetPalette([
            Color.Red,
            Color.Green,
            Color.Blue,
            Color.Yellow,
            Color.Cyan,
            Color.Magenta,
            Color.White,
            Color.Black,
            Color.Gray,
            Color.Orange,
            Color.Purple,
            Color.Brown,
            Color.LightGray,
            Color.DarkGray,
            Color.LightBlue,
            Color.LightGreen
        ]);

        HIDDevice hid = new(Q1Layout.Hid.start);
        
        RamDevice memory = new(Q1Layout.Memory.start, Q1Layout.Memory.size);
        
        cpu.Bus = bus;
        rom.AttachToBus(bus);
        display.AttachToBus(bus);
        memory.AttachToBus(bus);
        hid.AttachToBus(bus);

        Thread t = new(() =>
        {
            cpu.Reset();
            while (!cpu.AbortRequested)
            {
                // for (int i = 0; i < 64; i++)
                //     cpu.Clock();
                //
                // Thread.Sleep(1);
                //
                cpu.Clock();
            }
            Console.WriteLine($"Exit code: 0x{cpu.V[0]:X4}");
        });
        t.IsBackground = true;
        t.Start();

        using DisplayEngine game = new(Q1Layout.DisplayWidth, Q1Layout.DisplayHeight, 5,
            hid,
            tex =>
            {
                display.OutputTexture = tex;
                Console.WriteLine("Display surface initialized.");
            }, () => display.Clock(), () => cpu.AbortRequested);
        game.Run();
        
        Console.WriteLine("Display surface closed. Waiting for CPU thread to finish...");
        t.Join();
    }
}