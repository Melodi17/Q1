namespace Q1.core.Arch;

public record struct AddressingMode
{
    public string Name;
    public u8 SizeB;
    public u8 SizeW;
    
    public Func<Chip, u8> LoadByte;
    public Func<Chip, u16> LoadWord;
    public Action<Chip, u8> StoreByte;
    public Action<Chip, u16> StoreWord;
}