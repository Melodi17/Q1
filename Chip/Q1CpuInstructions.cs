namespace Q1Emu.Chip;

using System;
using System.Collections.Generic;

public partial class Q1Cpu
{
    public static Dictionary<u8, string[]> InstructionLookupStrings = new()
    {
        { 0x00, ["NOP", "JMP", "HALT", "MOV"] },
        { 0x01, ["RET", "CALL", "SUS", "MOVB"] },
        { 0x02, ["BZ", "BZ", "BR", "NOP"] },

        { 0x10, ["INV", "INV", "NOP", "AND"] },
        { 0x11, ["SHL", "SHL", "NOP", "OR"] },
        { 0x12, ["SHR", "SHR", "NOP", "XOR"] },

        { 0x20, ["NOT", "NOT", "NOP", "CMP"] },
        { 0x21, ["NOP", "NOP", "NOP", "LT"] },
        { 0x22, ["NOP", "NOP", "NOP", "GT"] },

        { 0x30, ["INC_AX", "INC", "NOP", "ADD"] },
        { 0x31, ["DEC_AX", "DEC", "NOP", "SUB"] },
        { 0x32, ["NOP", "NOP", "NOP", "DIV"] },
        { 0x33, ["NOP", "NOP", "NOP", "MUL"] },
        { 0x34, ["NOP", "NOP", "NOP", "MOD"] },
    };
    
    public Dictionary<u8, (Action, Action<u8>, Action, Action<u8, u8>)> InstructionLookup;
    private void LoadInstructionLookup()
    {
        this.InstructionLookup = new Dictionary<u8, (Action, Action<u8>, Action, Action<u8, u8>)>
        {
            { 0x00, (this.Instruction_NOP, this.Instruction_JMP, this.Instruction_HALT, this.Instruction_MOV) },
            { 0x01, (this.Instruction_RET, this.Instruction_CALL, this.Instruction_SUS, this.Instruction_MOVB) },
            { 0x02, (this.Instruction_BZ_LX, this.Instruction_BZ, this.Instruction_BR, this.Instruction_NOP) },

            { 0x10, (this.Instruction_INV_DX, this.Instruction_INV, this.Instruction_NOP, this.Instruction_AND) },
            { 0x11, (this.Instruction_SHL_DX, this.Instruction_SHL, this.Instruction_NOP, this.Instruction_OR) },
            { 0x12, (this.Instruction_SHR_DX, this.Instruction_SHR, this.Instruction_NOP, this.Instruction_XOR) },
            
            { 0x20, (this.Instruction_NOT_LX, this.Instruction_NOT, this.Instruction_NOP, this.Instruction_CMP) },
            { 0x21, (this.Instruction_NOP, this.Instruction_NOP, this.Instruction_NOP, this.Instruction_LT) },
            { 0x22, (this.Instruction_NOP, this.Instruction_NOP, this.Instruction_NOP, this.Instruction_GT) },

            { 0x30, (this.Instruction_INC_AX, this.Instruction_INC, this.Instruction_NOP, this.Instruction_ADD) },
            { 0x31, (this.Instruction_DEC_AX, this.Instruction_DEC, this.Instruction_NOP, this.Instruction_SUB) },
            { 0x32, (this.Instruction_NOP, this.Instruction_NOP, this.Instruction_NOP, this.Instruction_DIV) },
            { 0x33, (this.Instruction_NOP, this.Instruction_NOP, this.Instruction_NOP, this.Instruction_MUL) },
            { 0x34, (this.Instruction_NOP, this.Instruction_NOP, this.Instruction_NOP, this.Instruction_MOD) },
        };
    }
    private void Instruction_ADD(u8 m1, u8 m2)
    {
        u16 v1 = this.Address(m1).read();
        u16 v2 = this.Address(m2).read();
        
        u16 result = (u16) (v1 + v2);
        this.AX = result;
    }
    private void Instruction_SUB(u8 m1, u8 m2)
    {
        u16 v1 = this.Address(m1).read();
        u16 v2 = this.Address(m2).read();
        
        u16 result = (u16)(v1 - v2);
        this.AX = result;
    }
    private void Instruction_MUL(u8 m1, u8 m2)
    {
        u16 v1 = this.Address(m1).read();
        u16 v2 = this.Address(m2).read();
        
        u16 result = (u16) (v1 * v2);
        this.AX = result;
    }
    private void Instruction_DIV(u8 m1, u8 m2)
    {
        u16 v1 = this.Address(m1).read();
        u16 v2 = this.Address(m2).read();
        
        if (v2 == 0)
            throw new DivideByZeroException("Cannot perform division by zero.");
        u16 result = (u16)(v1 / v2);
        this.AX = result;
    }
    private void Instruction_MOD(u8 m1, u8 m2)
    {
        u16 v1 = this.Address(m1).read();
        u16 v2 = this.Address(m2).read();
        
        if (v2 == 0)
            throw new DivideByZeroException("Cannot perform modulo operation with divisor zero.");
        u16 result = (u16)(v1 % v2);
        this.AX = result;
    }
    private void Instruction_INC(u8 obj)
    {
        var address = this.Address(obj);
        u16 value = address.read();
        address.write((u16)(value + 1));
    }
    private void Instruction_DEC(u8 obj)
    {
        var address = this.Address(obj);
        u16 value = address.read();
        address.write((u16)(value - 1));
    }
    private void Instruction_INC_AX()
    {
        this.AX = (u16)(this.AX + 1);
    }
    private void Instruction_DEC_AX()
    {
        this.AX = (u16)(this.AX - 1);
    }
    private void Instruction_SHL(u8 obj)
    {
        var address = this.Address(obj);
        u16 value = address.read();
        u16 result = (u16)(value << 1);
        address.write(result);
    }
    private void Instruction_SHR(u8 obj)
    {
        var address = this.Address(obj);
        u16 value = address.read();
        u16 result = (u16)(value >> 1);
        address.write(result);
    }
    private void Instruction_SHL_DX()
    {
        this.DX = (u16)(this.DX << 1);
    }
    private void Instruction_SHR_DX()
    {
        this.DX = (u16)(this.DX >> 1);
    }
    private void Instruction_XOR(u8 m1, u8 m2)
    {
        u16 v1 = this.Address(m1).read();
        u16 v2 = this.Address(m2).read();
        
        u16 result = (u16)(v1 ^ v2);
        this.DX = result;
    }
    private void Instruction_OR(u8 m1, u8 m2)
    {
        u16 v1 = this.Address(m1).read();
        u16 v2 = this.Address(m2).read();
        
        u16 result = (u16)(v1 | v2);
        this.DX = result;
    }
    private void Instruction_INV(u8 obj)
    {
        var address = this.Address(obj);
        u16 value = address.read();
        u16 result = (u16)~value;
        address.write(result);
    }
    private void Instruction_INV_DX()
    {
        this.DX = (u16)~this.DX;
    }
    private void Instruction_AND(u8 m1, u8 m2)
    {
        u16 v1 = this.Address(m1).read();
        u16 v2 = this.Address(m2).read();
        
        u16 result = (u16)(v1 & v2);
        this.DX = result;
    }
    private void Instruction_NOT(u8 obj)
    {
        u16 value = this.Address(obj).read();
        u16 result = value == 0 ? (u16)1 : (u16)0;
        this.LX = result;
    }
    private void Instruction_NOT_LX()
    {
        this.LX = this.LX == 0 ? (u16)1 : (u16)0;
    }
    private void Instruction_GT(u8 m1, u8 m2)
    {
        u16 v1 = this.Address(m1).read();
        u16 v2 = this.Address(m2).read();
        
        this.LX = (u16)(v1 > v2 ? 1 : 0);
    }
    private void Instruction_LT(u8 m1, u8 m2)
    {
        u16 v1 = this.Address(m1).read();
        u16 v2 = this.Address(m2).read();
        
        this.LX = (u16)(v1 < v2 ? 1 : 0);
    }
    
    private void Instruction_BR()
    {
        this.FetchInstruction(out _, out _, out _, out u8 length);
        this.PC += length;
    }
    private void Instruction_BZ_LX()
    {
        if (this.LX == 0)
            this.Instruction_BR();
    }
    private void Instruction_BZ(u8 obj)
    {
        u16 value = this.Address(obj).read();
        if (value == 0)
            this.Instruction_BR();
    }
    private void Instruction_CMP(u8 m1, u8 m2)
    {
        u16 v1 = this.Address(m1).read();
        u16 v2 = this.Address(m2).read();
        
        // Set LX based on comparison
        this.LX = (u16)(v1 == v2 ? 1 : 0);
    }
    private void Instruction_MOV(u8 m1, u8 m2)
    {
        u16 value = this.Address(m1).read();
        this.Address(m2).write(value);
    }
    
    private void Instruction_MOVB(u8 m1, u8 m2)
    {
        u16 value = this.Address(m1).read();
        u8 low = Make.ref8(value)[1];
        this.Address(m2).writeByte(low);
    }

    public void Instruction_NOP() { }
    public void Instruction_NOP(u8 m1) { }
    public void Instruction_NOP(u8 m1, u8 m2) { }

    public void Instruction_JMP(u8 m1)
    {
        this.PC = this.Address(m1).read();
    }

    public void Instruction_RET()
    {
        this.PC = this.PopStack();
    }

    public void Instruction_CALL(u8 m1)
    {
        u16 address = this.Address(m1).read();
        this.PushStack(this.PC);
        this.PC = address;
    }

    public void Instruction_HALT()
    {
        throw new NotImplementedException("Halt instruction not implemented yet.");
    }

    public void Instruction_SUS()
    {
        this.PC -= 2;
    }
}