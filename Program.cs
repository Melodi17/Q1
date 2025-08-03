using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Xna.Framework;
using Q1Emu.Chip;
using Q1Emu;

internal class Program
{
    public static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            string filePath = args[0];
            if (filePath.EndsWith(".q1"))
            {
                Run(filePath);
            }
            else
            {
                Program.AssembleAndRun(filePath);
            }
        }
        else
        {
            Console.WriteLine("Usage: Q1Emu <file.q1|file.asm>");
            Console.WriteLine("Example: Q1Emu game.q1");
            Console.WriteLine("Example: Q1Emu game.asm");
        }
    }
    private static void AssembleAndRun(string filePath)
    {
        
    }
    private static void Run(string filePath)
    {
        Q1Cpu cpu = new();
        Bus bus = new(u16.MinValue, u16.MaxValue);
        RamDevice ram = new(0x1FFF, 1024 * 8);
        File.ReadAllBytes(filePath).CopyTo(ram.Memory, 0);

        cpu.PC = ram.AddressableStart;
        
        DisplayDevice display = new(16, 64, 32, 0x3FFF);
        display.SetPalette([
            Color.Red, Color.Green, Color.Blue, Color.Yellow, 
            Color.Cyan, Color.Magenta, Color.White, Color.Black, 
            Color.Gray, Color.Orange, Color.Purple, Color.Brown, 
            Color.LightGray, Color.DarkGray, Color.LightBlue, Color.LightGreen]);
        
        cpu.Bus = bus;
        ram.AttachToBus(bus);

        int frameRate = 600;
        Thread t = new(() =>
        {
            while (true)
            {
                cpu.Clock();
                display.Clock();
                Thread.Sleep(1000 / frameRate);
            }
        });
        t.IsBackground = true;
        t.Start();

        using DisplayEngine game = new(64, 32, 10, 
            tex =>
            {
                display.OutputTexture = tex;
                Console.WriteLine("Display texture initialized.");
            });
        game.Run();
    }
}