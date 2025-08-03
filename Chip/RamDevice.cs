namespace Q1Emu.Chip;

using System;
using System.Runtime.CompilerServices;

public class RamDevice : BusDevice
{
    public u8[] Memory;

    public RamDevice(u16 start, u16 size) : base(start, (u16) (start + size))
    {
        this.Memory           = new u8[size];
    }
    
    public override void Reset()
    {
        this.Memory = new u8[this.AddressableEnd - this.AddressableStart];
    }
    
    public override void Clock()
    {
        // No clocking needed for RAM
    }

    public override u8 Read(u16 address)
    {
        u16 relativeAddress = this.GetRelativeAddress(address);
        return this.Memory[relativeAddress];
    }

    public override void Write(u16 address, u8 value)
    {
        u16 relativeAddress = this.GetRelativeAddress(address);
        this.Memory[relativeAddress] = value;
    }

    public override ref8 ReadSeq(u16 address, u16 length)
    {
        u16 relativeAddress = this.GetRelativeAddress(address);
        // Console.WriteLine($"{address:X4} {relativeAddress:X4} {length:X4}");
        return new ref8(this.Memory, relativeAddress, length);
    }
    
    public override void WriteSeq(u16 address, u16 length, ref8 value)
    {
        ref8 destination = this.ReadSeq(address, length);
        value.CopyTo(destination);
    }
}