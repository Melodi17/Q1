namespace Q1.Emulator;

using System;
using System.Buffers.Binary;

public static class Make
{
    public static u16 u16(u8 high, u8 low)
    {
        return (u16) ((high << 8) | low);
    }
    
    public static u16 u16(ref8 data)
    {
        if (data.Length < 2)
            throw new ArgumentException("Data must contain at least 2 bytes to form a u16.");
        
        return Make.u16(data[0], data[1]);
    }
    
    public static u32 u32(u8 b1, u8 b2, u8 b3, u8 b4)
    {
        return (u32) ((b1 << 24) | (b2 << 16) | (b3 << 8) | b4);
    }
    
    public static u32 u32(ref8 data)
    {
        if (data.Length < 4)
            throw new ArgumentException("Data must contain at least 4 bytes to form a u32.");
        
        return Make.u32(data[0], data[1], data[2], data[3]);
    }
    
    public static ref8 ref8(u16 value)
    {
        u8 high = (u8) (value >> 8);
        u8 low = (u8) (value & 0xFF);
        return new ref8([high, low], 0, 2);
    }
    
    public static ref8 ref8(u32 value)
    {
        u8 b1 = (u8) (value >> 24);
        u8 b2 = (u8) (value >> 16);
        u8 b3 = (u8) (value >> 8);
        u8 b4 = (u8) (value & 0xFF);
        return new ref8([b1, b2, b3, b4], 0, 4);
    }
    public static i16 i16(ref8 value)
    {
        return BinaryPrimitives.ReadInt16BigEndian(value);
    }
    
    public static i16 i16(u8 high, u8 low)
    {
        return BinaryPrimitives.ReadInt16BigEndian(new ReadOnlySpan<u8>([high, low]));
    }
}