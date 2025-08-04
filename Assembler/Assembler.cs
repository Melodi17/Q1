namespace Q1Emu.Assembler;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Assembler
{
    public readonly BinaryWriter Writer;
    public readonly StringReader Reader;

    private Dictionary<string, u16> _labels        = new();
    private u16                     _addressOffset = 0;

    public Assembler(StringReader reader, BinaryWriter writer)
    {
        this.Reader = reader;
        this.Writer = writer;
    }

    public void Assemble()
    {
        while (this.Reader.ReadLine() is { } line)
        {
            string trimmedLine = line.Split(";")[0].Trim(); // Remove comments
            if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(';'))
                continue;

            if (trimmedLine.StartsWith(".org"))
            {
                // Handle .org directive
                string orgValue = trimmedLine[5..].Trim();
                this._addressOffset = this.GetConstant(orgValue);
                Console.WriteLine($"Setting address offset to {this._addressOffset:X4}");
            }

            else if (trimmedLine.EndsWith(":"))
                this._labels[trimmedLine[..^1]] = (u16) (this.Writer.BaseStream.Position + this._addressOffset);
            
            else
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

        u8 opcodeValue = this.ParseOpcode(opcode);

        u8 m1 = 0, m2 = 0;
        if (parts.Length > 0)
            m1 = this.ParseMode(parts[0]);
        if (parts.Length > 1)
            m2 = this.ParseMode(parts[1]);

        Console.WriteLine($"Parsed instruction: {opcode} with modes {m1:X2}, {m2:X2} (at address {this.Writer.BaseStream.Position + this._addressOffset:X4})");
        this.WriteInstruction(opcodeValue, m1, m2);
        if (parts.Length > 0)
            this.WriteMode(m1, parts[0]);
        if (parts.Length > 1)
            this.WriteMode(m2, parts[1]);
    }

    private u8 ParseOpcode(string opcode)
    {
        return opcode switch
        {
            "NOP" => 0x00, "JMP"  => 0x00, "MOV" => 0x00,
            "RET" => 0x01, "CALL" => 0x01, "CMP" => 0x02,
            "BR"  => 0x02, "BZ"   => 0x02, "LT"  => 0x02,
            "GT"  => 0x03,
            "INC" => 0x08, "ADD" => 0x08,
            "DEC" => 0x09,
            "MUL" => 0x0B,
            _     => throw new ArgumentException($"Unknown opcode: {opcode}")
        };
    }

    private u8 ParseMode(string mode)
    {
        mode = mode.Trim();

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

        if (this._labels.ContainsKey(mode))
        {
            return 0x01; // Immediate
        }

        if (mode.StartsWith("@"))
        {
            return 0x03; // Indirect
        }

        if (mode.StartsWith("[") && mode.EndsWith("]"))
        {
            string inner = mode[1..^1].Trim();
            if (inner.Contains("+"))
                return 0x07; // Register indirect with offset

            if (IsRegister(inner))
                return 0x06; // Register indirect

            return 0x02; // Direct
        }

        if (IsRegister(mode))
            return 0x05; // Register

        return 0x01; // Immediate
    }

    private void WriteInstruction(u8 opcode, u8 m1, u8 m2)
    {
        Write(opcode);
        Write((u8) (m1 << 4 | m2)); // Combine m1 and m2 into a single byte
    }

    private void WriteMode(u8 mode, string operand)
    {
        operand = operand.Trim();

        switch (mode)
        {
            case 0x00: // Implicit
                // No additional data needed
                break;

            case 0x01: // Immediate
                u16 immediateValue = this.GetConstant(operand);
                this.Write(immediateValue);
                break;

            case 0x02:                                                // Direct
                u16 directAddress = this.GetConstant(operand); // Remove brackets
                this.Write(directAddress);
                break;

            case 0x03:                                                  // Indirect
                u16 indirectAddress = this.GetConstant(operand); // Remove brackets and @
                this.Write(indirectAddress);
                break;

            case 0x04: // Relative
                i16 relativeOffset = this.GetSignedConstant(operand);
                Write(relativeOffset);
                break;

            case 0x05: // Register
                u8 regIndex = this.GetRegisterIndex(operand);
                Write(regIndex);
                break;

            case 0x06:                                                       // Register Indirect
                u8 indirectRegIndex = this.GetRegisterIndex(operand); // Remove brackets
                Write(indirectRegIndex);
                break;

            case 0x07: // Register Indirect with Offset
                u8 offsetRegIndex = this.GetRegisterIndex(operand[1..^1].Split('+')[0].Trim());
                i16 offsetValue = this.GetSignedConstant(operand[1..^1].Split('+')[1].Trim());
                Write(offsetRegIndex);
                Write(offsetValue);
                break;
        }
    }
    private i16 GetSignedConstant(string value)
    {
        value = this.Clean(value);
        
        if (value.StartsWith("$"))
        {
            // Hexadecimal offset
            return Convert.ToInt16(value[1..], 16);
        }
        else if (i16.TryParse(value, out i16 result))
            // Decimal offset
            return result;
        else
        {
            throw new ArgumentException($"Invalid offset value: {value}");
        }
    }
    private u8 GetRegisterIndex(string reg)
    {
        reg = this.Clean(reg);
        
        if (reg.StartsWith("V"))
            if (u8.TryParse(reg[1..], out u8 index) && index < 0x3F)
                return index;

        return reg switch
        {
            "AX" => 0x40,
            "AH" => 0x41,
            "AL" => 0x42,
            "DX" => 0x43,
            "DH" => 0x44,
            "DL" => 0x45,
            "LX" => 0x46,
            "PC" => 0x50,
            "SP" => 0x51,
            _    => throw new ArgumentException($"Invalid register index: {reg}")
        };
    }
    private u16 GetConstant(string value)
    {
        value = this.Clean(value);
        
        if (this._labels.ContainsKey(value))
        {
            // Label reference
            return this._labels[value];
        }
        if (value.StartsWith("$"))
        {
            // Hexadecimal constant (backwards) $1FFF
            u16 x = Convert.ToUInt16(value[1..], 16);
            return x;
        }
        else if (u16.TryParse(value, out u16 result))
            // Decimal constant
            return result;
        else
        {
            throw new ArgumentException($"Invalid constant value: {value}");
        }
    }

    private string Clean(string value)
    {
        if (value.StartsWith("@"))
            value = value[1..]; // Remove @ for indirect addressing
        
        if (value.StartsWith("[") && value.EndsWith("]"))
            value = value[1..^1].Trim(); // Remove brackets for direct addressing
        
        return value.Trim();
    }
    
    private void Write(u16 value)
    {
        u16 swappedValue = (u16) ((value >> 8) | (value << 8));
        this.Writer.Write(swappedValue);
    }
    
    private void Write(u8 value)
    {
        this.Writer.Write(value);
    }
    
    private void Write(i16 value)
    {
        u16 swappedValue = (u16) ((value >> 8) | (value << 8));
        this.Writer.Write(swappedValue);
    }
}