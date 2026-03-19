namespace Q1.core.Components;

public interface IAddressable
{
    public u8 Read(u16 address);
    public void Write(u16 address, u8 value);
}

public interface IBusDevice : IAddressable
{
    public u16 AddressableStart { get; }
    public u16 AddressableEnd { get; }
    
    public void Clock(Bus bus);
}

public static class AddressableExtensions
{
    public static u16 ReadWord(this IAddressable bus, u16 address)
    {
        u8 high = bus.Read(address);
        u8 low = bus.Read((u16) (address + 1));
        return (u16) ((high << 8) | low);
    }

    public static void WriteWord(this IAddressable bus, u16 address, u16 value)
    {
        u8 high = (u8) (value       & 0xFF);
        u8 low = (u8) ((value >> 8) & 0xFF);
        bus.Write(address, low);
        bus.Write((u16) (address + 1), high);
    }
}