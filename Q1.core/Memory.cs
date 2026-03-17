namespace Q1.core;

public class Memory : IAddressable
{
    private readonly u8[] _memory;
    
    public Memory(int size)
    {
        _memory = new u8[size];
    }
    
    public u8 Read(u16 address) => _memory[address];
    public void Write(u16 address, u8 value) => _memory[address] = value;
    public void Clock() { /* No timing behavior for simple memory */ }
}