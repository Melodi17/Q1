namespace Q1.Assembler;

using System.Diagnostics;
using Emulator;
using Emulator.Chip;

public class Assembler
{
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
        { 0x51, "SP" }
    };
    private readonly string[] InputLines;

    private readonly Dictionary<string, u16> _labels = new();

    public Assembler(string[] inputLines)
    {
        this.InputLines = inputLines;
    }

    private void ResolveLabels()
    {
        int offset = Q1Layout.ProgramStart;
        int pos = 0;
        foreach (string line in this.InputLines)
        {
            string trimmedLine = line.Split(";")[0].Trim();
            if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(';'))
                continue;

            if (trimmedLine.StartsWith(".org"))
            {
                string orgValue = trimmedLine[5..].Trim();
                offset = this.GetConstant(orgValue);
            }

            else if (trimmedLine.StartsWith(".ascii"))
            {
                string asciiValue = trimmedLine[7..].Trim()
                    [1..^1]; // Remove quotes

                for (int i = 0; i < asciiValue.Length; i++)
                {
                    char c = asciiValue[i];
                    if (c == '\\')
                    {
                        c = asciiValue[++i] switch
                        {
                            'n'  => '\n',
                            't'  => '\t',
                            '\\' => '\\',
                            '\'' => '\'',
                            '"'  => '"',
                            _    => throw new AssemblerException($"Unknown escape sequence: \\{asciiValue[i]}")
                        };
                    }

                    pos++;
                }
            }

            else if (trimmedLine.StartsWith(".raw"))
            {
                string[] values = trimmedLine[5..].Split(',');
                pos += values.Length * 2; // Each value is 2 bytes
            }

            else if (trimmedLine.StartsWith(".include"))
            {
                string includeFile = trimmedLine[9..].Trim()[1..^1];
                if (!File.Exists(includeFile))
                    throw new AssemblerException($"Include file '{includeFile}' not found");

                pos += File.ReadAllBytes(includeFile).Length;
            }

            else if (trimmedLine.EndsWith(":"))
            {
                string label = trimmedLine[..^1].Trim();
                if (this._labels.ContainsKey(label))
                    throw new AssemblerException($"Label '{label}' already defined");
                this._labels[label] = (u16) (pos + offset);
            }

            else
                pos += this.GetInstructionLength(trimmedLine);
        }
    }

    public IEnumerable<byte> Assemble()
    {
        this.ResolveLabels();

        foreach (string line in this.InputLines)
        {
            string trimmedLine = line.Split(";")[0].Trim(); // Remove comments
            if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(';'))
                continue;

            if (trimmedLine.StartsWith(".org"))
            {
                // Already handled in ResolveLabels
            }

            else if (trimmedLine.StartsWith(".ascii"))
            {
                string asciiValue = trimmedLine[7..].Trim()
                    [1..^1]; // Remove quotes

                for (int i = 0; i < asciiValue.Length; i++)
                {
                    char c = asciiValue[i];
                    if (c == '\\')
                    {
                        c = asciiValue[++i] switch
                        {
                            'n'  => '\n',
                            't'  => '\t',
                            '\\' => '\\',
                            '\'' => '\'',
                            '"'  => '"',
                            _    => throw new AssemblerException($"Unknown escape sequence: \\{asciiValue[i]}")
                        };
                    }

                    yield return (byte) c;
                }
            }

            else if (trimmedLine.StartsWith(".raw"))
            {
                string[] values = trimmedLine[5..].Split(',');
                foreach (string value in values)
                {
                    u16 c = this.GetConstant(value.Trim());
                    
                    yield return c.High();
                    yield return c.Low();
                }
            }

            else if (trimmedLine.StartsWith(".include"))
            {
                string includeFile = trimmedLine[9..].Trim()[1..^1];
                if (!File.Exists(includeFile))
                    throw new AssemblerException($"Include file '{includeFile}' not found");

                foreach (byte b in File.ReadAllBytes(includeFile))
                    yield return b;
            }

            else if (trimmedLine.EndsWith(":"))
            {
                // Handled in ResolveLabels
            }

            else
                foreach (byte b in this.GetInstructionBytes(trimmedLine))
                    yield return b;
        }
    }

    private int GetInstructionLength(string instruction)
    {
        string[] parts = instruction.Split(' ', 2);
        string opcode = parts[0].ToUpperInvariant();

        parts = parts.Length > 1 ? parts[1].Split(',') : [];
        if (parts.Length > 2)
            throw new AssemblerException("Invalid instruction format");

        u8 opcodeValue = this.ResolveOpcode(opcode, parts.Length, out u8 addressingIndex);
        (u8 m1, u8 m2) = this.ResolveAddressingModes(addressingIndex, parts);

        int length = 2; // Opcode + mode byte

        if (parts.Length > 0)
            length += this.GetModeLength(m1);

        if (parts.Length > 1)
            length += this.GetModeLength(m2);

        return length;
    }

    private IEnumerable<byte> GetInstructionBytes(string instruction)
    {
        string[] parts = instruction.Split(' ', 2);
        string opcode = parts[0].ToUpperInvariant();

        parts = parts.Length > 1 ? parts[1].Split(',') : [];
        if (parts.Length > 2)
            throw new AssemblerException("Invalid instruction format");

        u8 opcodeValue = this.ResolveOpcode(opcode, parts.Length, out u8 addressingIndex);
        (u8 m1, u8 m2) = this.ResolveAddressingModes(addressingIndex, parts);

        yield return opcodeValue;
        yield return (u8) ((m1 << 4) | m2); // Combine m1 and m2 into a single byte

        if (parts.Length > 0)
        {
            foreach (byte b in this.GetModeBytes(m1, parts[0]))
                yield return b;
        }

        if (parts.Length > 1)
        {
            foreach (byte b in this.GetModeBytes(m2, parts[1]))
                yield return b;
        }
    }
    private (u8 m1, u8 m2) ResolveAddressingModes(u8 addressingIndex, string[] parts)
    {
        u8 m1, m2;
        switch (addressingIndex)
        {
            case 0: // Implicit
                if (parts.Length > 0)
                    throw new AssemblerException("Implicit instruction should not have operands");

                m1 = 0x00;
                m2 = 0x00;
                break;

            case 1: // Simple addressing mode
                if (parts.Length > 2)
                    throw new AssemblerException("Simple addressing mode should have at most one operand");
                if (parts.Length == 0)
                    throw new AssemblerException("Simple addressing mode requires an operand");

                m1 = this.ResolveMode(parts[0]);
                m2 = 0x00;
                break;

            case 2: // Extended implicit
                if (parts.Length > 0)
                    throw new AssemblerException("Extended implicit instruction should not have operands");

                m1 = 0x00;
                m2 = 0x01;
                break;

            case 3: // Dual addressing mode
                if (parts.Length != 2)
                    throw new AssemblerException("Dual addressing mode requires exactly two operands");

                m1 = this.ResolveMode(parts[0]);
                m2 = this.ResolveMode(parts[1]);
                break;

            default:
                throw new UnreachableException("Invalid addressing index: " + addressingIndex);
        }
        return (m1, m2);
    }

    private u8 ResolveOpcode(string opcode, int parts, out u8 addressingIndex)
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

    private u8 ResolveMode(string mode)
    {
        mode = mode.Trim();

        bool IsRegister(string text)
        {
            if (text.Length > 1 && text[0] == 'V')
                return true;

            return Assembler.RegisterLookup.ContainsValue(text);
        }

        if (this._labels.ContainsKey(mode))
            return 0x01; // Immediate

        if (mode.StartsWith("@"))
            return 0x03; // Indirect

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

    private byte[] GetModeBytes(u8 mode, string operand)
    {
        operand = operand.Trim();

        switch (mode)
        {
            case 0x00: // Implicit
                // No additional data needed
                return [];

            case 0x01: // Immediate
                u16 immediateValue = this.GetConstant(operand);
                return [immediateValue.High(), immediateValue.Low()];

            case 0x02:                                         // Direct
                u16 directAddress = this.GetConstant(operand); // Remove brackets
                return [directAddress.High(), directAddress.Low()];

            case 0x03:                                           // Indirect
                u16 indirectAddress = this.GetConstant(operand); // Remove brackets and @
                return [indirectAddress.High(), indirectAddress.Low()];

            case 0x04: // Relative
                u16 relativeOffset = this.GetConstant(operand);
                return [relativeOffset.High(), relativeOffset.Low()];

            case 0x05: // Register
                u8 regIndex = this.GetRegisterIndex(operand);
                return [regIndex];

            case 0x06:                                                // Register Indirect
                u8 indirectRegIndex = this.GetRegisterIndex(operand); // Remove brackets
                return [indirectRegIndex];

            case 0x07: // Register Indirect with Offset
                u8 offsetRegIndex = this.GetRegisterIndex(operand[1..^1].Split('+')[0].Trim());
                u16 offsetValue = this.GetConstant(operand[1..^1].Split('+')[1].Trim());
                return [offsetRegIndex, offsetValue.High(), offsetValue.Low()];

            default:
                throw new AssemblerException($"Unknown addressing mode: {mode:X2}");
        }
    }

    private int GetModeLength(u8 mode)
    {
        return mode switch
        {
            0x00 => 0, // Implicit
            0x01 => 2, // Immediate
            0x02 => 2, // Direct
            0x03 => 2, // Indirect
            0x04 => 2, // Relative
            0x05 => 1, // Register
            0x06 => 1, // Register indirect
            0x07 => 3, // Register indirect with offset
            _    => throw new AssemblerException($"Unknown addressing mode: {mode:X2}")
        };
    }
    private u8 GetRegisterIndex(string reg)
    {
        reg = this.Clean(reg);

        if (reg.StartsWith("V"))
        {
            if (u8.TryParse(reg[1..], out u8 index) && index < 0x3F)
                return index;
        }

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
        if (u16.TryParse(value, out u16 result))
            // Decimal constant
            return result;
        
        if (this._labels.TryGetValue(value, out u16 labelAddress))
            // Label reference
            return labelAddress;
        
        return 0;
    }

    private string Clean(string value)
    {
        if (value.StartsWith("@"))
            value = value[1..]; // Remove @ for indirect addressing

        if (value.StartsWith("[") && value.EndsWith("]"))
            value = value[1..^1].Trim(); // Remove brackets for direct addressing

        return value.Trim();
    }
}

public class AssemblerException(string message)
    : Exception(message);