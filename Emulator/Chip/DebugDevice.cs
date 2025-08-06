namespace Q1.Emulator.Chip;

using System;

public class DebugDevice(u16 addressPoint)
    : BusDevice(addressPoint, (u16) (addressPoint + 2))
{
    public override u8 Read(u16 address) => throw new Exception("DebugDevice.Read not permitted");
    public override void Write(u16 address, u8 value)
    {
        Console.WriteLine($"DebugDevice.Write: x{value:X4} b{value:b8}");
    }
}