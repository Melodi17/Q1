namespace Q1.Emulator;

using System;
using System.IO;
using System.Threading;
using Chip;
using CommandLine;
using Microsoft.Xna.Framework;

internal class Program
{
    public static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args).WithParsed(Program.Main);
    }

    static void Main(Options options)
    {
        Q1Cpu cpu = new();
        Bus bus = new(u16.MinValue, u16.MaxValue);
        RamDevice rom = new(Q1Layout.Rom.start, Q1Layout.Rom.size);
        File.ReadAllBytes(options.InputFile).CopyTo(rom.Memory, 0);
        
        DisplayDevice display = new(16, 64, 32, Q1Layout.Display);
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
        
        RamDevice memory = new(Q1Layout.Memory.start, Q1Layout.Memory.size);
        
        cpu.Bus = bus;
        rom.AttachToBus(bus);
        display.AttachToBus(bus);
        memory.AttachToBus(bus);

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

        using DisplayEngine game = new(64, 32, 10,
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