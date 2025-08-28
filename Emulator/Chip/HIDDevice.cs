namespace Q1.Emulator.Chip;

using System;

public class HIDDevice : RamDevice
{
    public HIDDevice(u16 start) : base(start, HIDDevice.CalculateSize()) { }

    public static u16 CalculateSize()
    {
        return 4 + 256;
    }

    public void SetMouse(u16 positionalIndex, bool leftDown, bool rightDown, bool middleDown)
    {
        this.WriteWord(this.AddressableStart, positionalIndex);
        u16 buttons = (u16) ((leftDown ? 1 : 0)
                             | ((rightDown ? 1 : 0)  << 1)
                             | ((middleDown ? 1 : 0) << 2));
        this.WriteWord((u16) (this.AddressableStart + 2), buttons);
    }

    public void SetKeyboard(u8 scanCode, bool pressed)
    {
        int index = scanCode >> 3; // keyCode / 8
        int bit = scanCode & 7;    // keyCode % 8

        ushort addr = (ushort) (4 + index);
        u8 value = this.Read((u16) (this.AddressableStart + addr));

        if (pressed)
            value |= (u8) (1 << bit);
        else
            value &= (u8) ~(1 << bit);

        this.Write((u16) (this.AddressableStart + addr), value);
    }
    
    public bool GetKeyboard(int scanCode)
    {
        int index = scanCode >> 3;
        int bit   = scanCode & 7;

        ushort addr = (ushort) (4 + index);
        byte value  = this.Read((u16) (this.AddressableStart + addr));

        return (value & (1 << bit)) != 0;
    }
}