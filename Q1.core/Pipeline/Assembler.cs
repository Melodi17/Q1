namespace Q1.core.Pipeline;

using Q1.core.Arch;
using Q1.core.Arch.Constants;
using Q1.core.Arch.Lookups;

public class Assembler
{
    private Dictionary<string, AssemblerBlock> _blocks = new();
    private string _currentBlock = "";
    private u16 _addressOffset = 0;
    
    public MemoryStream Assemble(TextReader source)
    {
        this._blocks[this._currentBlock] = new AssemblerBlock();
        
        while (source.ReadLine() is { } line)
        {
            line = this.StripComments(line);
            if (string.IsNullOrEmpty(line))
                continue;

            if (line.StartsWith('.'))
                this.HandleDirective(line[1..]);
            else if (line.EndsWith(':'))
                this.HandleLabel(line[..^1]);
            else
                this.HandleInstruction(line);
        }
        
        return this.CreateFinalBinary();
    }

    private MemoryStream CreateFinalBinary()
    {
        Dictionary<string, u16> labelAddresses = new();
        u16 currentAddress = this._addressOffset;
        foreach ((string key, AssemblerBlock value) in this._blocks)
        {
            labelAddresses[key] = currentAddress;
            currentAddress += (u16) value.Size;
        }
        
        MemoryStream finalBinary = new();
        foreach ((_, AssemblerBlock value) in this._blocks)
        {
            foreach ((i64 offset, (bool word, string constant)) in value.DeferredConstants)
            {
                if (!labelAddresses.TryGetValue(constant, out u16 address))
                    throw new Exception($"Undefined label: {constant}");

                if (word)
                    value.ResolveWordConstant((i32) offset, address);
                else
                    value.ResolveByteConstant((i32) offset, (u8) address);
            }
            
            value.WriteToStream(finalBinary);
        }

        return finalBinary;
    }

    private void HandleDirective(string directive)
    {
        string[] parts = directive
            .Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.ToLower())
            .ToArray();
        
        string directiveName = parts[0];
        string[] args = parts[1..];
        
        if (directiveName == "org")
        {
            if (args.Length != 1)
                throw new Exception($"Expected 1 argument for .org directive, got {args.Length}");

            if (args[0].StartsWith("0x"))
                this._addressOffset = Convert.ToUInt16(args[0][2..], 16);
            else if (args[0].StartsWith("0b"))
                this._addressOffset = Convert.ToUInt16(args[0][2..], 2);
            else if (i32.TryParse(args[0], out i32 decimalValue))
                this._addressOffset = (u16) decimalValue;
            else
                throw new Exception($"Invalid argument for .org directive: {args[0]}");
        }
        else
            throw new Exception($"Unknown directive: {directiveName}");
    }
    
    private void HandleLabel(string label)
    {
        this._currentBlock = label;
        if (this._blocks.ContainsKey(label))
            throw new Exception($"Duplicate label: {label}");
        this._blocks[label] = new AssemblerBlock();
    }
    
    private void HandleInstruction(string instruction)
    {
        string[] parts = instruction
            .Split([' ', '\t', ','], StringSplitOptions.RemoveEmptyEntries);
        
        string opcodeName = parts[0].ToUpper();
        bool isWordInstruction = !(parts.Length > 1 && parts[1].Equals("byte", StringComparison.InvariantCultureIgnoreCase));
        
        string[] args = isWordInstruction ? parts[1..] : parts[2..];
        
        var opcode = InstructionSet.GetOpcodeWithIndex(opcodeName, out u8 opcodeIndex);
        switch (opcodeIndex)
        {
            case 0: // Implicit
            {
                if (args.Length != 0)
                    throw new Exception($"Expected 0 arguments for opcode {opcodeName}, got {args.Length}");

                this.WriteInstruction(opcode, 0, 0, isWordInstruction);
                
                break;
            }
            
            case 1: // Simple Addressing
            {
                if (args.Length != 1)
                    throw new Exception($"Expected 1 argument for opcode {opcodeName}, got {args.Length}");

                u8 m1 = this.GetArgumentType(args[0]);
                this.WriteInstruction(opcode, m1, 0, isWordInstruction);
                this.WriteArgument(args[0], isWordInstruction);
                
                break;
            }
            
            case 2: // Extended Implicit
            {
                if (args.Length != 0)
                    throw new Exception($"Expected 0 arguments for opcode {opcodeName}, got {args.Length}");

                this.WriteInstruction(opcode, 0, 1, isWordInstruction);
                
                break;
            }
            
            case 3: // Extended Addressing
            {
                if (args.Length != 2)
                    throw new Exception($"Expected 2 arguments for opcode {opcodeName}, got {args.Length}");

                u8 m1 = this.GetArgumentType(args[0]);
                u8 m2 = this.GetArgumentType(args[1]);
                
                this.WriteInstruction(opcode, m1, m2, isWordInstruction);
                this.WriteArgument(args[0], isWordInstruction);
                this.WriteArgument(args[1], isWordInstruction);
                
                break;
            }
        }
    }
    
    private void WriteInstruction(u8 opcode, u8 m1, u8 m2, bool word)
    {
        u16 instruction = InstructionEncoding.Encode(opcode, m1, m2, word);
        this.WriteWordToBlock(instruction);
    }
    
    private u8 GetArgumentType(string arg)
    {
        if (arg.StartsWith('[') && arg.EndsWith(']'))
        {
            string inner = arg[1..^1];
            
            if (ChipRegisters.GetRegisterIndex(inner.ToUpper()) != null)
                return AddressingModes.GetAddressingMode("Register Indirect"); 
            
            if (inner.Contains('+'))
                return AddressingModes.GetAddressingMode("Register Indirect with Offset");
            
            return AddressingModes.GetAddressingMode("Direct");
        }
        
        if (arg.StartsWith('@'))
        {
            if (arg.StartsWith("@[") && arg.EndsWith(']'))
                return AddressingModes.GetAddressingMode("Indirect");
            else
                throw new Exception($"Invalid indirect argument format: {arg}");
        }
        
        if (ChipRegisters.GetRegisterIndex(arg.ToUpper()) != null)
            return AddressingModes.GetAddressingMode("Register");
        
        return AddressingModes.GetAddressingMode("Immediate");
    }

    private void WriteArgument(string arg, bool isWord)
    {
        if (arg.StartsWith('[') && arg.EndsWith(']'))
        {
            string inner = arg[1..^1];

            if (ChipRegisters.GetRegisterIndex(inner.ToUpper()) is { } registerIndex)
                this.WriteByteToBlock(registerIndex);

            else if (inner.Contains('+'))
            {
                string[] parts = inner.Split('+', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                    throw new Exception($"Invalid register indirect with offset format: {arg}");
                
                string registerPart = parts[0].Trim().ToUpper();
                string offsetPart = parts[1].Trim();
                
                if (ChipRegisters.GetRegisterIndex(registerPart) is not { } regIndex)
                    throw new Exception($"Invalid register in register indirect with offset: {registerPart}");
                
                this.WriteByteToBlock(regIndex);
                this.WriteConstant(offsetPart, isWord);
            }
            else
                this.WriteConstant(inner, isWord);
        }

        else if (arg.StartsWith('@'))
        {
            if (arg.StartsWith("@[") && arg.EndsWith(']'))
            {
                string inner = arg[2..^1];

                this.WriteConstant(inner, isWord);
            }
            else 
                throw new Exception($"Invalid indirect argument format: {arg}");
        }
        
        else if (ChipRegisters.GetRegisterIndex(arg.ToUpper()) is { } registerIndex)
            this.WriteByteToBlock(registerIndex);
        else
            this.WriteConstant(arg, isWord);
    }

    private void WriteConstant(string constant, bool isWord)
    {
        // hex 0x0-9a-fA-F
        if (constant.StartsWith("0X", StringComparison.InvariantCultureIgnoreCase))
        {
            string hexPart = constant[2..];
            if (isWord)
                this.WriteWordToBlock(Convert.ToUInt16(hexPart, 16));
            else
                this.WriteByteToBlock(Convert.ToByte(hexPart, 16));
        }
        // binary 0b0-1
        else if (constant.StartsWith("0B", StringComparison.InvariantCultureIgnoreCase))
        {
            string binaryPart = constant[2..];
            if (isWord)
                this.WriteWordToBlock(Convert.ToUInt16(binaryPart, 2));
            else
                this.WriteByteToBlock(Convert.ToByte(binaryPart, 2));
        }
        // char `a, etc.
        else if (constant.StartsWith('`') && constant.Length == 2)
        {
            char charValue = constant[1];
            if (isWord)
                this.WriteWordToBlock(charValue);
            else
                this.WriteByteToBlock((u8) charValue);
        }

        // decimal 0-9+
        else if (i32.TryParse(constant, out i32 decimalValue))
        {
            if (isWord)
                this.WriteWordToBlock((u16) decimalValue);
            else
                this.WriteByteToBlock((u8) decimalValue);
        }

        // label
        else
        {
            if (isWord)
                this.DeferWriteWordToBlock(constant);
            else
                this.DeferWriteByteToBlock(constant);
        }
    }

    private void WriteByteToBlock(u8 data)
    {
        this._blocks[this._currentBlock].WriteByte(data);
    }
    
    private void WriteWordToBlock(u16 data)
    {
        this._blocks[this._currentBlock].WriteWord(data);
    }
    
    private void DeferWriteByteToBlock(string constant)
    {
        this._blocks[this._currentBlock].DeferByteConstant(constant);
    }
    
    private void DeferWriteWordToBlock(string constant)
    {
        this._blocks[this._currentBlock].DeferWordConstant(constant);
    }

    private string StripComments(string line)
    {
        int commentIndex = line.IndexOf(';');
        if (commentIndex >= 0)
            return line[..commentIndex].Trim();
        
        return line.Trim();
    }
}