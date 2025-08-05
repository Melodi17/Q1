namespace Q1Emu.Assembler;

using System;
using System.Collections.Generic;
using System.IO;
using Chip;

public class Disassembler
{
    public readonly StringWriter Writer;
    public readonly BinaryReader Reader;
    
    public Disassembler(StringWriter writer, BinaryReader reader)
    {
        this.Writer = writer;
        this.Reader = reader;
    }
    
    private static Dictionary<u8, string> ModeLookup = new()
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
            (0x00, 0x01)    => 2, // Extended implicit
            (_, 0x00)    => 1, // Simple addressing mode
            _            => 3  // Extended addressing mode
        };

        string opcodeName = GetOpcodeName(op, i);

        if (i == 0 || i == 2)
            return opcodeName;

        if (i == 1)
            return $"{opcodeName} {GetModeName(m1)}";

        if (i == 3)
            return $"{opcodeName} {GetModeName(m1)}, {GetModeName(m2)}";

        throw new InvalidOperationException($"Unknown instruction format for opcode {op:X2} with modes {m1:X2}, {m2:X2}");
    }

    private static string GetOpcodeName(u8 opcode, u8 i)
    {
        if (!Q1Cpu.InstructionLookupStrings.TryGetValue(opcode, out string[]? opNames))
            return $"[{opcode:X2}]"; // Fallback for unknown opcodes

        return opNames[i];
    }

    private static string GetModeName(u8 mode)
    {
        if (ModeLookup.TryGetValue(mode, out string? modeName))
            return modeName;

        return $"[{mode:X2}]"; // Fallback for unknown modes
    }
}