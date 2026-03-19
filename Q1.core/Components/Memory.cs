namespace Q1.core.Components;

public class Memory : IBusDevice
{
    private readonly u8[] _memory;
    
    public u16 AddressableStart { get; }
    public u16 AddressableEnd { get; }
    
    public Memory(u16 start, u16 size)
    {
        this.AddressableStart = start;
        this.AddressableEnd = (u16) (start + size);
        
        this._memory = new u8[size];
    }
    
    public u8 Read(u16 address) => this._memory[address];
    public void Write(u16 address, u8 value) => this._memory[address] = value;
    public void Clock(Bus bus) { /* No timing behavior for simple memory */ }
}