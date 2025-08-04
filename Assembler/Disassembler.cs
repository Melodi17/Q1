namespace Q1Emu.Assembler;

using System;
using System.Collections.Generic;

public class Disassembler
{
    public static Dictionary<u8, string[]> InstructionLookup = new()
    {
        { 0x00, ["NOP", "JMP", "HALT", "MOV"] },
        { 0x01, ["RET", "CALL", "SUS", "CMP"] },
        { 0x02, ["BR", "BZ", "BZ_LX", "LT"] },
        { 0x03, ["NOP", "NOP", "NOP", "GT"] },

        { 0x04, ["NOT_LX", "NOT", "NOP", "AND"] },
        { 0x05, ["INV_DX", "INV", "NOP", "OR"] },
        { 0x06, ["SHL_DX", "SHL", "NOP", "XOR"] },
        { 0x07, ["SHR_DX", "SHR", "NOP", "NOP"] },

        { 0x08, ["INC_AX", "INC", "NOP", "ADD"] },
        { 0x09, ["DEC_AX", "DEC", "NOP", "SUB"] },
        { 0x0A, ["NOP", "NOP", "NOP", "DIV"] },
        { 0x0B, ["NOP", "NOP", "NOP", "MUL"] },
        { 0x0C, ["NOP", "NOP", "NOP", "MOD"] },
    };
    
    public static Dictionary<u8, string> ModeLookup = new()
    {
        { 0x00, "" },
        { 0x01, "c" },
        { 0x02, "[a]" },
        { 0x03, "@[a]" },
        { 0x04, "pc + c" },
        { 0x05, "r" },
        { 0x06, "[r]" },
        { 0x07, "[r + c]" },
    };
    
    public static string ParseInstruction(u16 instruction)
    {
        u8 op = (u8) ((instruction >> 8) & 0xFF); // 256 opcodes
        u8 m1 = (u8) ((instruction >> 4) & 0x0F); // 16 modes
        u8 m2 = (u8) (instruction        & 0x0F); // 16 modes

        u8 i = (m1, m2) switch
        {
            (0x00, 0x00) => 0, // Implicit
            (0x00, _)    => 2, // Extended implicit
            (_, 0x00)    => 1, // Simple addressing mode
            _            => 3  // Extended addressing mode
        };
        
        string opcodeName = GetOpcodeName(op, i);
        string mode1 = GetModeName(m1);
        string mode2 = GetModeName(m2);

        if (i == 0)
            return opcodeName;
        
        if (i == 2)
            return $"{opcodeName}";
        
        if (i == 1)
            return $"{opcodeName} {mode1}";
        
        if (i == 3)
            return $"{opcodeName} {mode1}, {mode2}";
        
        throw new InvalidOperationException($"Unknown instruction format for opcode {op:X2} with modes {m1:X2}, {m2:X2}");
    }
    
    private static string GetOpcodeName(u8 opcode, u8 i)
    {
        if (!InstructionLookup.ContainsKey(opcode) || i >= InstructionLookup[opcode].Length)
            return $"{opcode:X2}";
        
        return InstructionLookup[opcode][i];
    }
    
    private static string GetModeName(u8 mode)
    {
        if (ModeLookup.TryGetValue(mode, out string modeName))
            return modeName;
        
        return $"[{mode:X2}]"; // Fallback for unknown modes
    }
}