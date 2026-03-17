namespace Q1.core.Arch;

public static class InstructionSet
{
    public static readonly InstructionGroup[] Lookup = new InstructionGroup[128];
    
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
            ExtendedAddressing = new ExtendedInstruction { Name = "SHL", Execute = InstructionSet.ShiftRight }
        };

        #endregion
    }

    private static void Nop(Chip chip, bool word) { /* No operation */ }
    private static void Nop(Chip chip, u8 m1, bool word) { /* No operation */ }
    private static void Nop(Chip chip, u8 m1, u8 m2, bool word) { /* No operation */ }

    #region Control Instructions

    private static void Halt(Chip chip, bool word)
    {
        throw new NotImplementedException("HALT instruction is not implemented yet.");
    }

    private static void Suspend(Chip chip, bool word)
    {
        throw new NotImplementedException("SUS instruction is not implemented yet.");
    }
    
    private static void Move(Chip chip, u8 m1, u8 m2, bool word)
    {
        u16 value = chip.Load(m1, word);
        chip.Store(m2, value, word);
    }
    
    private static void Jump(Chip chip, u8 m1, bool word)
    {
        u16 address = chip.Load(m1, word);
        chip.Pc = address;
    }

    private static void Call(Chip chip, u8 m1, bool word)
    {
        u16 targetAddress = chip.Load(m1, word);
        chip.Push(chip.Pc);
        chip.Pc = targetAddress;
    }
    
    private static void Return(Chip chip, bool word)
    {
        u16 returnAddress = chip.Pop();
        chip.Pc = returnAddress;
    }
    
    private static void BranchIfZero(Chip chip, u8 m1, bool word)
    {
        u16 value = chip.Load(m1, word);
        if (value == 0)
            chip.SkipInstruction();
    }
    
    private static void BranchIfZeroLx(Chip chip, bool word)
    {
        if (chip.Lx == 0)
            chip.SkipInstruction();
    }

    private static void Branch(Chip chip, bool word)
    {
        chip.SkipInstruction();
    }
    
    private static void Push(Chip chip, u8 m1, bool word)
    {
        u16 value = chip.Load(m1, word);
        chip.Push(value);
    }
    
    private static void Pop(Chip chip, u8 m1, bool word)
    {
        u16 value = chip.Pop();
        chip.Store(m1, value, word);
    }
    
    #endregion

    #region Data Instructions
    
    private static void Inverse(Chip chip, u8 m1, bool word)
    {
        u16 value = chip.Load(m1, word);
        value = (u16)~value;

        chip.Dx = value;
    }
    
    private static void InverseDx(Chip chip, bool word)
    {
        chip.Dx = (u16)~chip.Dx;
    }
    
    private static void And(Chip chip, u8 m1, u8 m2, bool word)
    {
        u16 value1 = chip.Load(m1, word);
        u16 value2 = chip.Load(m2, word);
        u16 result = (u16)(value1 & value2);

        chip.Dx = result;
    }
    
    private static void Or(Chip chip, u8 m1, u8 m2, bool word)
    {
        u16 value1 = chip.Load(m1, word);
        u16 value2 = chip.Load(m2, word);
        u16 result = (u16)(value1 | value2);

        chip.Dx = result;
    }
    
    private static void Xor(Chip chip, u8 m1, u8 m2, bool word)
    {
        u16 value1 = chip.Load(m1, word);
        u16 value2 = chip.Load(m2, word);
        u16 result = (u16)(value1 ^ value2);

        chip.Dx = result;
    }

    private static void ShiftLeft(Chip chip, u8 m1, u8 m2, bool word)
    {
        u16 value = chip.Load(m1, word);
        u16 shiftAmount = (u16) (chip.Load(m2, word) & 0xF); // Limit shift to 0-15
        u16 result = (u16) (value << shiftAmount);

        chip.Dx = result;
    }
    
    private static void ShiftRight(Chip chip, u8 m1, u8 m2, bool word)
    {
        u16 value = chip.Load(m1, word);
        u16 shiftAmount = (u16) (chip.Load(m2, word) & 0xF); // Limit shift to 0-15
        u16 result = (u16) (value >> shiftAmount);

        chip.Dx = result;
    }
    
    private static void ShiftPlaceLeft(Chip chip, u8 m1, bool word)
    {
        u16 value = chip.Load(m1, word);
        u16 result = (u16) ((value << 1) | (value >> 15)); // Rotate left

        chip.Dx = result;
    }
    
    private static void ShiftPlaceRight(Chip chip, u8 m1, bool word)
    {
        u16 value = chip.Load(m1, word);
        u16 result = (u16) ((value >> 1) | (value << 15)); // Rotate right

        chip.Dx = result;
    }
    
    private static void ShiftPlaceLeftDx(Chip chip, bool word)
    {
        u16 value = chip.Dx;
        u16 result = (u16) ((value << 1) | (value >> 15)); // Rotate left

        chip.Dx = result;
    }
    
    private static void ShiftPlaceRightDx(Chip chip, bool word)
    {
        u16 value = chip.Dx;
        u16 result = (u16) ((value >> 1) | (value << 15)); // Rotate right

        chip.Dx = result;
    }

    #endregion
    
    #region Logic Instructions

    

    #endregion
    
    #region Arithmetic Instructions

    

    #endregion
}