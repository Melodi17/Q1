namespace Q1.core.Arch;

// Big endian
public class Chip
{
    public IAddressable Bus;
    public u16 Pc, Sp;
    public u16 Ax, Dx, Lx;
    
    private u16[] _registers;

    public void Reset()
    {
        this._registers = new u16[64];
        this.Pc = ChipLayout.PROGRAM_START;
        this.Sp = ChipLayout.STACK_START;
    }

    public void Clock()
    {
        this.Bus.Clock();
        
        this.Fetch(out u8 opcode, out u8 m1, out u8 m2, out bool word);

        switch (m1, m2)
        {
            case (0x00, 0x00):
                InstructionSet.Lookup[opcode].Implicit.Execute(this, word);
                break;
            
            case (_, 0x00):
                InstructionSet.Lookup[opcode].SimpleAddressing.Execute(this, m1, word);
                break;
            
            case (0x00, 0x01):
                InstructionSet.Lookup[opcode].ExtendedImplicit.Execute(this, word);
                break;
            
            case (_, _):
                InstructionSet.Lookup[opcode].ExtendedAddressing.Execute(this, m1, m2, word);
                break;
        }
    }
    
    /// [opcode: 7 bits] [word: 1 bits] [m1: 4 bits] [m2: 4 bits]
    public void Fetch(out u8 opcode, out u8 m1, out u8 m2, out bool word)
    { 
        u16 instruction = this.Bus.ReadWord(this.Pc);
        this.Pc += 2;

        opcode = (u8) ((instruction >> 8) & 0xFE); // 128 opcodes
        word = ((instruction >> 8) & 0x1) != 0; // flag for word mode
        
        m1 = (u8) ((instruction >> 4) & 0x0F); // 16 modes
        m2 = (u8) (instruction        & 0x0F); // 16 modes
    }

    public void SkipInstruction()
    {
        this.Fetch(out u8 _, out u8 m1, out u8 m2, out bool word);
        this.Pc += (u8)(Size(m1, word) + Size(m2, word));
    }

    public u16 Load(u8 mode, bool word)
    {
        AddressingMode am = AddressingModes.Lookup[mode];
        return word ? am.LoadWord(this) : am.LoadByte(this);
    }
    
    public void Store(u8 mode, u16 value, bool word)
    {
        AddressingMode am = AddressingModes.Lookup[mode];
        if (word) am.StoreWord(this, value);
        else am.StoreByte(this, (u8)(value & 0xFF));
    }
    
    public u8 Size(u8 mode, bool word)
    {
        AddressingMode am = AddressingModes.Lookup[mode];
        return word ? am.SizeW : am.SizeB;
    }
    
    public void Push(u16 value)
    {
        if (this.Sp + 2 > ChipLayout.STACK_START + ChipLayout.STACK_SIZE) 
            throw new InvalidOperationException("Stack overflow");
        
        this.Bus.WriteWord(this.Sp, value);

        this.Sp += 2;
    }
    
    public u16 Pop()
    {
        if (this.Sp - 2 < ChipLayout.STACK_START) 
            throw new InvalidOperationException("Stack underflow");
        
        this.Sp -= 2;
        u16 value = this.Bus.ReadWord(this.Sp);
        
        return value;
    }
    
    public u16 GetRegister(u8 index)
    {
        if (index < 64) return this._registers[index];
        
        if (index == 0x40) return this.Ax;
        if (index == 0x41) return (u16) (this.Ax >> 8); // High byte of Ax
        if (index == 0x42) return (u16) (this.Ax & 0xFF); // Low byte of Ax
        
        if (index == 0x44) return this.Dx;
        if (index == 0x45) return (u16) (this.Dx >> 8); // High byte of Dx
        if (index == 0x46) return (u16) (this.Dx & 0xFF); // Low byte of Dx
        
        if (index == 0x48) return this.Lx;
        
        if (index == 0x4C) return this.Pc;
        if (index == 0x4D) return this.Sp;
        
        throw new InvalidOperationException($"Invalid register index: {index}");
    }
    
    public void SetRegister(u8 index, u16 value)
    {
        if (index < 64) this._registers[index] = value;
        
        else if (index == 0x40) this.Ax = value;
        else if (index == 0x41) this.Ax = (u16)((this.Ax & 0x00FF) | ((value & 0xFF) << 8)); // Set high byte of Ax
        else if (index == 0x42) this.Ax = (u16)((this.Ax & 0xFF00) | (value & 0xFF)); // Set low byte of Ax
        
        else if (index == 0x44) this.Dx = value;
        else if (index == 0x45) this.Dx = (u16)((this.Dx & 0x00FF) | ((value & 0xFF) << 8)); // Set high byte of Dx
        else if (index == 0x46) this.Dx = (u16)((this.Dx & 0xFF00) | (value & 0xFF)); // Set low byte of Dx
        
        else if (index == 0x48) this.Lx = value;
        
        else if (index == 0x4C) this.Pc = value;
        else if (index == 0x4D) this.Sp = value;
        
        else throw new InvalidOperationException($"Invalid register index: {index}");
    }
}