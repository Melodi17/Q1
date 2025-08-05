namespace Q1Emu.Assembler;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Chip;

public class Assembler
{
    public readonly BinaryWriter Writer;
    public readonly StringReader Reader;

    private Dictionary<string, u16> _labels         = new();
    private Dictionary<u16, string> _unloadedLabels = new();
    private u16                     _addressOffset  = 0;

    public static Dictionary<u16, string> RegisterLookup = new()
    {
        { 0x40, "AX" },
        { 0x41, "AH" },
        { 0x42, "AL" },
        { 0x43, "DX" },
        { 0x44, "DH" },
        { 0x45, "DL" },
        { 0x46, "LX" },
        { 0x50, "PC" },
        { 0x51, "SP" },
    };

    public Assembler(StringReader reader, BinaryWriter writer)
    {
        this.Reader = reader;
        this.Writer = writer;
    }

    public void Assemble()
    {
        int lineNumber = 0;
        while (this.Reader.ReadLine() is { } line)
        {
            lineNumber++;
            string trimmedLine = line.Split(";")[0].Trim(); // Remove comments
            if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(';'))
                continue;

            if (trimmedLine.StartsWith(".org"))
            {
                // Handle .org directive
                string orgValue = trimmedLine[5..].Trim();
                this._addressOffset = this.GetConstant(orgValue);
            }

            else if (trimmedLine.StartsWith(".ascii"))
            {
                string asciiValue = trimmedLine[7..].Trim()
                    [1..^1]; // Remove quotes

                for (int i = 0; i < asciiValue.Length; i++)
                {
                    char c = asciiValue[i];
                    if (c == '\\')
                        c = asciiValue[++i] switch
                        {
                            'n'  => '\n',
                            't'  => '\t',
                            '\\' => '\\',
                            '\'' => '\'',
                            '"'  => '"',
                            _    => throw new AssemblerException($"Unknown escape sequence: \\{asciiValue[i]}"),
                        };

                    this.Write((u8) c);
                }
            }

            else if (trimmedLine.StartsWith(".raw"))
            {
                string[] values = trimmedLine[5..].Split(',');
                foreach (string value in values)
                    this.Write(GetConstant(value.Trim()));
            }
            
            else if (trimmedLine.StartsWith(".include"))
            {
                string includeFile = trimmedLine[9..].Trim()[1..^1];
                if (!File.Exists(includeFile))
                {
                    WriteError(line, lineNumber, $"Include file '{includeFile}' not found");
                    return;
                }

                foreach (byte b in File.ReadAllBytes(includeFile))
                    this.Writer.Write(b);
            }

            else if (trimmedLine.EndsWith(":"))
                this._labels[trimmedLine[..^1]] = (u16) (this.Writer.BaseStream.Position + this._addressOffset);

            else
            {
                try
                {
                    this.ParseInstruction(trimmedLine);
                }
                catch (AssemblerException ex)
                {
                    WriteError(line, lineNumber, ex.Message);
                    return;
                }
            }
        }

        foreach ((u16 key, string value) in this._unloadedLabels)
        {
            if (this._labels.TryGetValue(value, out u16 address))
            {
                Console.WriteLine($"Resolving label '{value}' at address {address:X4} for key {key:X4}");
                this.Writer.BaseStream.Seek(key, SeekOrigin.Begin);
                this.Write(address);
            }
            else
            {
                WriteError($"", 0, "Label not defined");
            }
        }
    }

    private void WriteError(string line, int lineNumber, string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Error on line {lineNumber}: {message}");
        Console.WriteLine($" --> {line}");
        Console.ResetColor();
    }

    private void ParseInstruction(string instruction)
    {
        string[] parts = instruction.Split(' ', 2);
        string opcode = parts[0].ToUpperInvariant();

        parts = parts.Length > 1 ? parts[1].Split(',') : [];
        if (parts.Length > 2)
            throw new AssemblerException($"Invalid instruction format");

        u8 opcodeValue = this.ParseOpcode(opcode, parts.Length, out u8 addressingIndex);

        u8 m1, m2;
        switch (addressingIndex)
        {
            case 0: // Implicit
                if (parts.Length > 0)
                    throw new AssemblerException($"Implicit instruction should not have operands");

                m1 = 0x00;
                m2 = 0x00;
                break;

            case 1: // Simple addressing mode
                if (parts.Length > 2)
                    throw new AssemblerException($"Simple addressing mode should have at most one operand");
                if (parts.Length == 0)
                    throw new AssemblerException($"Simple addressing mode requires an operand");

                m1 = this.ParseMode(parts[0]);
                m2 = 0x00;
                break;

            case 2: // Extended implicit
                if (parts.Length > 0)
                    throw new AssemblerException($"Extended implicit instruction should not have operands");

                m1 = 0x00;
                m2 = 0x01;
                break;

            case 3: // Dual addressing mode
                if (parts.Length != 2)
                    throw new AssemblerException($"Dual addressing mode requires exactly two operands");

                m1 = this.ParseMode(parts[0]);
                m2 = this.ParseMode(parts[1]);
                break;

            default:
                throw new UnreachableException("Invalid addressing index: " + addressingIndex);
        }

        this.WriteInstruction(opcodeValue, m1, m2);

        if (parts.Length > 0)
            this.WriteMode(m1, parts[0]);
        if (parts.Length > 1)
            this.WriteMode(m2, parts[1]);
    }

    private u8 ParseOpcode(string opcode, int parts, out u8 addressingIndex)
    {
        List<int> addressingIndices = new();
        u8? opCode = null;
        foreach ((u8 key, string[] value) in Q1Cpu.InstructionLookupStrings)
        {
            if (value.Contains(opcode))
            {
                addressingIndices.AddRange(value.AllIndexesOf(opcode));
                if (opCode is not null && opCode != key)
                    throw new AssemblerException($"Ambiguous opcode '{opcode}' matches multiple instructions: {opCode:X4} and {key:X4}");

                opCode = key;
            }
        }

        if (opCode is null)
            throw new AssemblerException($"Unknown opcode '{opcode}'");

        int Parts(int index)
            => index switch
            {
                0 => 0, // Implicit
                1 => 1, // Simple addressing mode
                2 => 0, // Extended implicit
                3 => 2, // Dual addressing mode
                _ => throw new UnreachableException("Invalid addressing index: " + index)
            };

        if (addressingIndices.All(x => Parts(x) != parts))
            throw new AssemblerException($"Opcode '{opcode}' does not match the expected number of operands: {parts}");

        addressingIndex = (u8) addressingIndices.First(x => Parts(x) == parts);
        return opCode.Value;
    }

    private u8 ParseMode(string mode)
    {
        mode = mode.Trim();

        bool IsRegister(string text)
        {
            if (text.Length > 1 && text[0] == 'V')
                return true;

            return Assembler.RegisterLookup.ContainsValue(text);
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

            case 0x02:                                         // Direct
                u16 directAddress = this.GetConstant(operand); // Remove brackets
                this.Write(directAddress);
                break;

            case 0x03:                                           // Indirect
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

            case 0x06:                                                // Register Indirect
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
    private u8 GetRegisterIndex(string reg)
    {
        reg = this.Clean(reg);

        if (reg.StartsWith("V"))
            if (u8.TryParse(reg[1..], out u8 index) && index < 0x3F)
                return index;

        if (Assembler.RegisterLookup.ContainsValue(reg))
        {
            // Find the register index by value
            return (u8) Assembler.RegisterLookup.FirstOrDefault(x => x.Value == reg).Key;
        }

        throw new AssemblerException($"Invalid register: {reg}");
    }
    private u16 GetConstant(string value)
    {
        value = this.Clean(value);

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
            this._unloadedLabels[(u16) this.Writer.BaseStream.Position] = value;
            return 0;
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
            this._unloadedLabels[(u16) this.Writer.BaseStream.Position] = value;
            return 0;
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

public class AssemblerException(string message)
    : Exception(message);