namespace Q1.core.Components;

using Q1.core.Arch;

public class Bus : IAddressable
{
    private Chip? _chip;
    private readonly List<IBusDevice> _devices;

    public u16 AddressableStart { get; }
    public u16 AddressableEnd { get; }

    public Bus(u16 addressableStart, u16 addressableEnd)
    {
        this.AddressableStart = addressableStart;
        this.AddressableEnd = addressableEnd;
        this._devices = new();
    }

    public void SendInterrupt(u16 code)
    {
        if (this._chip == null)
            throw new InvalidOperationException("Chip is not attached to bus");
        
        this._chip.Interrupt(code);
    }
    
    public void AttachChip(Chip chip)
    {
        if (this._chip != null)
            throw new InvalidOperationException("Bus already has a chip attached.");

        this._chip = chip;
        chip.Bus = this;
    }

    public void MountDevice(IBusDevice device)
    {
        if (device.AddressableStart >= device.AddressableEnd)
            throw new ArgumentException("Addressable range is invalid.", nameof(device));

        int index = 0;
        while (index < this._devices.Count && this._devices[index].AddressableStart < device.AddressableStart)
            index++;

        if (index > 0)
        {
            IBusDevice prev = this._devices[index - 1];
            if (device.AddressableStart < prev.AddressableEnd)
                throw new InvalidOperationException("Address range overlaps with previous device.");
        }

        if (index < this._devices.Count)
        {
            IBusDevice next = this._devices[index];
            if (device.AddressableEnd > next.AddressableStart)
                throw new InvalidOperationException("Address range overlaps with next device.");
        }

        this._devices.Insert(index, device);
    }

    private IBusDevice? GetDevice(u16 address)
    {
        foreach (IBusDevice device in this._devices)
        {
            if (address < device.AddressableStart)
                break;

            if (address < device.AddressableEnd)
                return device;
        }

        return null;
    }

    public u8 Read(u16 address)
    {
        IBusDevice? device = this.GetDevice(address);
        if (device == null)
            return 0;

        return device.Read((u16) (address - device.AddressableStart));
    }

    public void Write(u16 address, u8 value)
    {
        IBusDevice? device = this.GetDevice(address);
        if (device == null)
            return;

        device.Write((u16) (address - device.AddressableStart), value);
    }

    public void Clock()
    {
        foreach (IBusDevice addressable in this._devices)
            addressable.Clock(this);
    }
}