namespace Q1.Assembler;

public static class Extensions
{
    
    public static u8 High(this u16 value)
    {
        return (u8)(value >> 8);
    }
    
    public static u8 Low(this u16 value)
    {
        return (u8)(value & 0xFF);
    }
}