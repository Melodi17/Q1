namespace Q1.core.Arch.Lookups;

public static class InstructionSet
{
    public static readonly InstructionGroup[] Lookup = new InstructionGroup[128];
    
    public static u8 GetOpcode(string instructionName)
    {
        for (u8 opcode = 0; opcode < InstructionSet.Lookup.Length; opcode++)
        {
            var group = InstructionSet.Lookup[opcode];
            if (group.Implicit.Name == instructionName ||
                group.SimpleAddressing.Name == instructionName ||
                group.ExtendedImplicit.Name == instructionName ||
                group.ExtendedAddressing.Name == instructionName)
            {
                return opcode;
            }
        }
        throw new ArgumentException($"Instruction '{instructionName}' not found in the instruction set.");
    }
    
    static InstructionSet()
    {
        var nop = new ImplicitInstruction { Name = "NOP", Execute = InstructionSet.Nop };
        var nopSimple = new SimpleInstruction { Name = "NOP", Execute = InstructionSet.Nop };
        var nopExtended = new ExtendedInstruction { Name = "NOP", Execute = InstructionSet.Nop };

        #region Control Group

        InstructionSet.Lookup[0x00] = new InstructionGroup
        {
            Implicit = nop,
            SimpleAddressing = new SimpleInstruction { Name = "JMP", Execute = InstructionSet.Jump },
            ExtendedImplicit = new ImplicitInstruction { Name = "HALT", Execute = InstructionSet.Halt },
            ExtendedAddressing = new ExtendedInstruction { Name = "MOV", Execute = InstructionSet.Move }
        };
        
        InstructionSet.Lookup[0x01] = new InstructionGroup
        {
            Implicit = new ImplicitInstruction { Name = "RET", Execute = InstructionSet.Return },
            SimpleAddressing = new SimpleInstruction { Name = "CALL", Execute = InstructionSet.Call },
            ExtendedImplicit = new ImplicitInstruction { Name = "SUS", Execute = InstructionSet.Suspend },
            ExtendedAddressing = nopExtended
        };
        
        InstructionSet.Lookup[0x02] = new InstructionGroup
        {
            Implicit = new ImplicitInstruction { Name = "BZ(LX)", Execute = InstructionSet.BranchIfZeroLx },
            SimpleAddressing = new SimpleInstruction { Name = "BZ", Execute = InstructionSet.BranchIfZero },
            ExtendedImplicit = new ImplicitInstruction { Name = "BR", Execute = InstructionSet.Branch },
            ExtendedAddressing = nopExtended
        };
        
        InstructionSet.Lookup[0x03] = new InstructionGroup
        {
            Implicit = nop,
            SimpleAddressing = new SimpleInstruction { Name = "PUSH", Execute = InstructionSet.Push },
            ExtendedImplicit = nop,
            ExtendedAddressing = nopExtended
        };
        
        InstructionSet.Lookup[0x04] = new InstructionGroup
        {
            Implicit = nop,
            SimpleAddressing = new SimpleInstruction { Name = "POP", Execute = InstructionSet.Pop },
            ExtendedImplicit = nop,
            ExtendedAddressing = nopExtended
        };
        
        InstructionSet.Lookup[0x05] = new InstructionGroup
        {
            Implicit = nop,
            SimpleAddressing = new SimpleInstruction { Name = "INT", Execute = InstructionSet.Interrupt },
            ExtendedImplicit = nop,
            ExtendedAddressing = nopExtended
        };

        #endregion
        
        #region Data Group

        InstructionSet.Lookup[0x10] = new InstructionGroup
        {
            Implicit = new ImplicitInstruction { Name = "INV(DX)", Execute = InstructionSet.InverseDx },
            SimpleAddressing = new SimpleInstruction { Name = "INV", Execute = InstructionSet.Inverse },
            ExtendedImplicit = nop,
            ExtendedAddressing = new ExtendedInstruction { Name = "AND", Execute = InstructionSet.And }
        };
        
        InstructionSet.Lookup[0x11] = new InstructionGroup
        {
            Implicit = nop,
            SimpleAddressing = nopSimple,
            ExtendedImplicit = nop,
            ExtendedAddressing = new ExtendedInstruction { Name = "OR", Execute = InstructionSet.Or }
        };
        
        InstructionSet.Lookup[0x12] = new InstructionGroup
        {
            Implicit = nop,
            SimpleAddressing = nopSimple,
            ExtendedImplicit = nop,
            ExtendedAddressing = new ExtendedInstruction { Name = "XOR", Execute = InstructionSet.Xor }
        };
        
        InstructionSet.Lookup[0x13] = new InstructionGroup
        {
            Implicit = new ImplicitInstruction { Name = "SHPL(DX)", Execute = InstructionSet.ShiftPlaceLeftDx },
            SimpleAddressing = new SimpleInstruction { Name = "SHPL", Execute = InstructionSet.ShiftPlaceLeft },
            ExtendedImplicit = nop,
            ExtendedAddressing = new ExtendedInstruction { Name = "SHL", Execute = InstructionSet.ShiftLeft }
        };
        
        InstructionSet.Lookup[0x14] = new InstructionGroup
        {
            Implicit = new ImplicitInstruction { Name = "SHPR(DX)", Execute = InstructionSet.ShiftPlaceRightDx },
            SimpleAddressing = new SimpleInstruction { Name = "SHPR", Execute = InstructionSet.ShiftPlaceRight },
            ExtendedImplicit = nop,
            ExtendedAddressing = new ExtendedInstruction { Name = "SHR", Execute = InstructionSet.ShiftRight }
        };

        #endregion

        #region Logic Group

        InstructionSet.Lookup[0x20] = new InstructionGroup
        {
            Implicit = new ImplicitInstruction { Name = "NOT(LX)", Execute = InstructionSet.NotLx },
            SimpleAddressing = new SimpleInstruction { Name = "NOT", Execute = InstructionSet.Not },
            ExtendedImplicit = nop,
            ExtendedAddressing = new ExtendedInstruction { Name = "CMP", Execute = InstructionSet.Compare }
        };
        
        InstructionSet.Lookup[0x21] = new InstructionGroup
        {
            Implicit = nop,
            SimpleAddressing = nopSimple,
            ExtendedImplicit = nop,
            ExtendedAddressing = new ExtendedInstruction { Name = "LT", Execute = InstructionSet.LessThan }
        };
        
        InstructionSet.Lookup[0x22] = new InstructionGroup
        {
            Implicit = nop,
            SimpleAddressing = nopSimple,
            ExtendedImplicit = nop,
            ExtendedAddressing = new ExtendedInstruction { Name = "GT", Execute = InstructionSet.GreaterThan }
        };

        #endregion
        
        #region Arithmatic Group

        InstructionSet.Lookup[0x30] = new InstructionGroup
        {
            Implicit = new ImplicitInstruction { Name = "INC(AX)", Execute = InstructionSet.IncrementAx },
            SimpleAddressing = new SimpleInstruction { Name = "INC", Execute = InstructionSet.Increment },
            ExtendedImplicit = nop,
            ExtendedAddressing = new ExtendedInstruction { Name = "ADD", Execute = InstructionSet.Add }
        };
        
        InstructionSet.Lookup[0x31] = new InstructionGroup
        {
            Implicit = new ImplicitInstruction { Name = "DEC(AX)", Execute = InstructionSet.DecrementAx },
            SimpleAddressing = new SimpleInstruction { Name = "DEC", Execute = InstructionSet.Decrement },
            ExtendedImplicit = nop,
            ExtendedAddressing = new ExtendedInstruction { Name = "SUB", Execute = InstructionSet.Subtract }
        };
        
        InstructionSet.Lookup[0x32] = new InstructionGroup
        {
            Implicit = nop,
            SimpleAddressing = nopSimple,
            ExtendedImplicit = nop,
            ExtendedAddressing = new ExtendedInstruction { Name = "DIV", Execute = InstructionSet.Divide }
        };
        
        InstructionSet.Lookup[0x33] = new InstructionGroup
        {
            Implicit = nop,
            SimpleAddressing = nopSimple,
            ExtendedImplicit = nop,
            ExtendedAddressing = new ExtendedInstruction { Name = "MUL", Execute = InstructionSet.Multiply }
        };
        
        InstructionSet.Lookup[0x34] = new InstructionGroup
        {
            Implicit = nop,
            SimpleAddressing = nopSimple,
            ExtendedImplicit = nop,
            ExtendedAddressing = new ExtendedInstruction { Name = "MOD", Execute = InstructionSet.Modulo }
        };

        #endregion
    }

    public static void Nop(Chip chip, bool word) { /* No operation */ }
    public static void Nop(Chip chip, u8 m1, bool word) { /* No operation */ }
    public static void Nop(Chip chip, u8 m1, u8 m2, bool word) { /* No operation */ }

    #region Control Instructions

    public static void Halt(Chip chip, bool word)
    {
        throw new HaltException();
    }

    public static void Suspend(Chip chip, bool word)
    {
        throw new NotImplementedException("SUS instruction is not implemented yet.");
    }
    
    public static void Interrupt(Chip chip, u8 m1, bool word)
    {
        u16 interruptNumber = chip.Load(m1, word);
        
        chip.Interrupt(interruptNumber);
    }
    
    public static void Move(Chip chip, u8 m1, u8 m2, bool word)
    {
        u16 value = chip.Load(m1, word);
        chip.Store(m2, value, word);
    }
    
    public static void Jump(Chip chip, u8 m1, bool word)
    {
        u16 address = chip.Load(m1, word);
        chip.Pc = address;
    }

    public static void Call(Chip chip, u8 m1, bool word)
    {
        u16 targetAddress = chip.Load(m1, word);
        chip.Push(chip.Pc);
        chip.Pc = targetAddress;
    }
    
    public static void Return(Chip chip, bool word)
    {
        u16 returnAddress = chip.Pop();
        chip.Pc = returnAddress;
    }
    
    public static void BranchIfZero(Chip chip, u8 m1, bool word)
    {
        u16 value = chip.Load(m1, word);
        if (value == 0)
            chip.SkipInstruction();
    }
    
    public static void BranchIfZeroLx(Chip chip, bool word)
    {
        if (chip.Lx == 0)
            chip.SkipInstruction();
    }

    public static void Branch(Chip chip, bool word)
    {
        chip.SkipInstruction();
    }
    
    public static void Push(Chip chip, u8 m1, bool word)
    {
        u16 value = chip.Load(m1, word);
        chip.Push(value);
    }
    
    public static void Pop(Chip chip, u8 m1, bool word)
    {
        u16 value = chip.Pop();
        chip.Store(m1, value, word);
    }
    
    #endregion

    #region Data Instructions
    
    public static void Inverse(Chip chip, u8 m1, bool word)
    {
        u16 value = chip.Load(m1, word);
        value = (u16)~value;

        chip.Dx = value;
    }
    
    public static void InverseDx(Chip chip, bool word)
    {
        chip.Dx = (u16)~chip.Dx;
    }
    
    public static void And(Chip chip, u8 m1, u8 m2, bool word)
    {
        u16 value1 = chip.Load(m1, word);
        u16 value2 = chip.Load(m2, word);
        u16 result = (u16)(value1 & value2);

        chip.Dx = result;
    }
    
    public static void Or(Chip chip, u8 m1, u8 m2, bool word)
    {
        u16 value1 = chip.Load(m1, word);
        u16 value2 = chip.Load(m2, word);
        u16 result = (u16)(value1 | value2);

        chip.Dx = result;
    }
    
    public static void Xor(Chip chip, u8 m1, u8 m2, bool word)
    {
        u16 value1 = chip.Load(m1, word);
        u16 value2 = chip.Load(m2, word);
        u16 result = (u16)(value1 ^ value2);

        chip.Dx = result;
    }

    public static void ShiftLeft(Chip chip, u8 m1, u8 m2, bool word)
    {
        u16 value = chip.Load(m1, word);
        u16 shiftAmount = (u16) (chip.Load(m2, word) & 0xF); // Limit shift to 0-15
        u16 result = (u16) (value << shiftAmount);

        chip.Dx = result;
    }
    
    public static void ShiftRight(Chip chip, u8 m1, u8 m2, bool word)
    {
        u16 value = chip.Load(m1, word);
        u16 shiftAmount = (u16) (chip.Load(m2, word) & 0xF); // Limit shift to 0-15
        u16 result = (u16) (value >> shiftAmount);

        chip.Dx = result;
    }
    
    public static void ShiftPlaceLeft(Chip chip, u8 m1, bool word)
    {
        u16 value = chip.Load(m1, word);
        u16 result = (u16) ((value << 1) | (value >> 15)); // Rotate left

        chip.Dx = result;
    }
    
    public static void ShiftPlaceRight(Chip chip, u8 m1, bool word)
    {
        u16 value = chip.Load(m1, word);
        u16 result = (u16) ((value >> 1) | (value << 15)); // Rotate right

        chip.Dx = result;
    }
    
    public static void ShiftPlaceLeftDx(Chip chip, bool word)
    {
        u16 value = chip.Dx;
        u16 result = (u16) ((value << 1) | (value >> 15)); // Rotate left

        chip.Dx = result;
    }
    
    public static void ShiftPlaceRightDx(Chip chip, bool word)
    {
        u16 value = chip.Dx;
        u16 result = (u16) ((value >> 1) | (value << 15)); // Rotate right

        chip.Dx = result;
    }

    #endregion
    
    #region Logic Instructions
    
    public static void Not(Chip chip, u8 m1, bool word)
    {
        u16 value = chip.Load(m1, word);
        chip.Lx = value == 0 ? (u16)1 : (u16)0;
    }
    
    public static void NotLx(Chip chip, bool word)
    {
        chip.Lx = chip.Lx == 0 ? (u16)1 : (u16)0;
    }
    
    public static void Compare(Chip chip, u8 m1, u8 m2, bool word)
    {
        u16 value1 = chip.Load(m1, word);
        u16 value2 = chip.Load(m2, word);
        chip.Lx = value1 == value2 ? (u16)1 : (u16)0;
    }
    
    public static void LessThan(Chip chip, u8 m1, u8 m2, bool word)
    {
        u16 value1 = chip.Load(m1, word);
        u16 value2 = chip.Load(m2, word);
        chip.Lx = value1 < value2 ? (u16)1 : (u16)0;
    }
    
    public static void GreaterThan(Chip chip, u8 m1, u8 m2, bool word)
    {
        u16 value1 = chip.Load(m1, word);
        u16 value2 = chip.Load(m2, word);
        chip.Lx = value1 > value2 ? (u16)1 : (u16)0;
    }

    #endregion
    
    #region Arithmetic Instructions

    public static void Increment(Chip chip, u8 m1, bool word)
    {
        u16 value = chip.Load(m1, word);
        value++;
        chip.Store(m1, value, word);
    }
    
    public static void IncrementAx(Chip chip, bool word)
    {
        chip.Ax++;
    }
    
    public static void Decrement(Chip chip, u8 m1, bool word)
    {
        u16 value = chip.Load(m1, word);
        value--;
        chip.Store(m1, value, word);
    }
    
    public static void DecrementAx(Chip chip, bool word)
    {
        chip.Ax--;
    }
    
    public static void Add(Chip chip, u8 m1, u8 m2, bool word)
    {
        u16 value1 = chip.Load(m1, word);
        u16 value2 = chip.Load(m2, word);
        u16 result = (u16)(value1 + value2);

        chip.Ax = result;
    }
    
    public static void Subtract(Chip chip, u8 m1, u8 m2, bool word)
    {
        u16 value1 = chip.Load(m1, word);
        u16 value2 = chip.Load(m2, word);
        u16 result = (u16)(value1 - value2);

        chip.Ax = result;
    }
    
    public static void Multiply(Chip chip, u8 m1, u8 m2, bool word)
    {
        u16 value1 = chip.Load(m1, word);
        u16 value2 = chip.Load(m2, word);
        u16 result = (u16)(value1 * value2);

        chip.Ax = result;
    }
    
    public static void Divide(Chip chip, u8 m1, u8 m2, bool word)
    {
        u16 value1 = chip.Load(m1, word);
        u16 value2 = chip.Load(m2, word);
        if (value2 == 0)
            throw new DivideByZeroException("Attempted to divide by zero.");

        u16 result = (u16)(value1 / value2);
        chip.Ax = result;
    }
    
    public static void Modulo(Chip chip, u8 m1, u8 m2, bool word)
    {
        u16 value1 = chip.Load(m1, word);
        u16 value2 = chip.Load(m2, word);
        if (value2 == 0)
            throw new DivideByZeroException("Attempted to modulo by zero.");

        u16 result = (u16)(value1 % value2);
        chip.Ax = result;
    }

    #endregion
}