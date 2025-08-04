namespace Q1Emu.Chip;

using System.Collections.Generic;
using System.Linq;

public class Bus : BusDevice
{
    public List<BusDevice> Devices;

    public Bus(u16 addressableStart, u16 addressableEnd) : base(addressableStart, addressableEnd)
    {
        this.Devices = [];
    }
    private BusDevice? GetDevice(u16 address)
    {
        BusDevice? device = this.Devices.FirstOrDefault(x => address >= x.AddressableStart && address < x.AddressableEnd);
        return device;
    }

    public override u8 Read(u16 address)
    {
        BusDevice? device = GetDevice(address);
        if (device == null)
            return 0;

        return device.Read(address);
    }

    public override void Write(u16 address, u8 value)
    {
        BusDevice? device = GetDevice(address);
        if (device == null)
            return;

        device.Write(address, value);
    }

    public override ref8 ReadSeq(u16 address, u16 length)
    {
        BusDevice? device = this.GetDevice(address);
        if (device == null)
            return new u8[length];

        return device.ReadSeq(address, length);
    }

    public override void WriteSeq(u16 address, u16 length, ref8 value)
    {
        BusDevice? device = this.GetDevice(address);
        if (device == null)
            return;

        device.WriteSeq(address, length, value);
    }
}