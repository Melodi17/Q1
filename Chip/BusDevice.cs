namespace Q1Emu.Chip;

using System.Runtime.CompilerServices;

public abstract class BusDevice
{
    public BusDevice(u16 addressableStart, u16 addressableEnd)
    {
        this.AddressableStart = addressableStart;
        this.AddressableEnd   = addressableEnd;
    }
    
    public Bus Bus { get; set; }
    public abstract u8 Read(u16 address);
    public abstract void Write(u16 address, u8 value);
    
    public abstract ref8 ReadSeq(u16 address, u16 length);
    public abstract void WriteSeq(u16 address, u16 length, ref8 value);
    
    public u16 ReadWord(u16 address)
    {
        ref8 data = this.ReadSeq(address, 2);
        u16 value = Make.u16(data);
        return value;
    }
    
    public void WriteWord(u16 address, u16 value)
    {
        ref8 data = Make.ref8(value);
        this.WriteSeq(address, 2, data);
    }

    public virtual void Clock() { }
    public virtual void Reset() { }
    
    public void AttachToBus(Bus bus)
    {
        this.Bus = bus;
        bus.Devices.Add(this);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected u16 GetRelativeAddress(u16 address)
    {
        return (u16) (address - this.AddressableStart);
    }

    public u16 AddressableStart { get; }
    public u16 AddressableEnd   { get; }
}