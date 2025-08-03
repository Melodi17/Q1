namespace Q1Emu.Assembler;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Assembler
{
    public readonly BinaryWriter Writer;
    public readonly StringReader Reader;

    public Assembler(StringReader reader, BinaryWriter writer)
    {
        this.Reader = reader;
        this.Writer = writer;
    }

    public void Assemble()
    {
        while (this.Reader.ReadLine() is { } line)
        {
            string trimmedLine = line.Trim();
            if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(';'))
                continue;

            this.ParseInstruction(trimmedLine);
        }
    }

    private void ParseInstruction(string instruction)
    {
        string[] parts = instruction.Split(' ', 2);
        string opcode = parts[0].ToUpperInvariant();

        parts = parts.Length > 1 ? parts[1].Split(',') : [];
        if (parts.Length > 2)
            throw new ArgumentException($"Invalid instruction format: {instruction}");

        u8 m1 = 0, m2 = 0;
        if (parts.Length > 0)
        {
            m1 = this.ParseOperand(parts[0]);
            if (parts.Length > 1)
                m2 = this.ParseOperand(parts[1]);
        }

        switch (opcode)
        {
            case "NOP":
                this.Writer.Write((u8) 0x00);
                break;

            default:
                throw new NotSupportedException($"Unsupported opcode: {opcode}");
        }
    }

    private u8 ParseOperand(string operand)
    {
        operand = operand.Trim();

        bool IsRegister(string text)
        {
            if (text.Length > 1 && text[0] == 'V')
                return true;

            string[] hardcodedRegisters =
            [
                "AX",
                "AH",
                "AL",
                "DX",
                "DH",
                "DL",
                "LX",
                "PC",
                "SP",
            ];

            return hardcodedRegisters.Contains(text);
        }

        if (operand.StartsWith("@"))
        {
            return 0x03; // Indirect
        }

        if (operand.StartsWith("[") && operand.EndsWith("]"))
        {
            string inner = operand[1..^1].Trim();
            if (inner.Contains("+"))
                return 0x07; // Register indirect with offset
            
            if (IsRegister(inner))
                return 0x06; // Register indirect

            return 0x02; // Direct
        }

        if (IsRegister(operand))
            return 0x05; // Register

        return 0x01; // Immediate
    }
}