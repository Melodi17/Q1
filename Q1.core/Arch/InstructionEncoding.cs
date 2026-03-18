namespace Q1.core.Arch;

public static class InstructionEncoding
{
    public static void Decode(u16 instruction, out u8 opcode, out u8 m1, out u8 m2, out bool word)
    {
        word   = ((instruction     >> 15) & 0x1) != 0; // top bit
        opcode = (u8)((instruction >> 8)  & 0x7F);     // 7 clean bits

        m1 = (u8)((instruction >> 4) & 0x0F);
        m2 = (u8)(instruction        & 0x0F);
    }
    
    public static u16 Encode(u8 opcode, u8 m1, u8 m2, bool word)
    {
        u16 instruction = 0;

        instruction |= (u16)((opcode & 0x7F) << 8);  // 7-bit opcode
        instruction |= (u16)((word ? 1 : 0)  << 15); // top bit flag

        instruction |= (u16)((m1 & 0x0F) << 4);
        instruction |= (u16)(m2 & 0x0F);

        return instruction;
    }
}