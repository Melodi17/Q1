namespace Q1.core.Arch;

using Components;
using Constants;
using Lookups;

// Big endian
public class Chip
{
    public Bus Bus;
    public u16 Pc, Sp;
    public u16 Ax, Dx, Lx;
    
    private u16[] _registers;
    
    public Chip()
    {
        this.Reset();
    }

    public void Reset()
    {
        this._registers = new u16[ChipRegisters.GENERIC_REGISTER_COUNT];
        this.Pc = ChipLayout.PROGRAM_START;
        this.Sp = ChipLayout.STACK_START;
    }

    public void Clock()
    {
        this.Bus.Clock();
        
        this.Fetch(out u8 opcode, out u8 m1, out u8 m2, out bool word);
        
        var instruction = InstructionSet.Lookup[opcode];

        switch (m1, m2)
        {
            case (0x00, 0x00):
                instruction.Implicit.Execute(this, word);
                break;
            
            case (_, 0x00):
                instruction.SimpleAddressing.Execute(this, m1, word);
                break;
            
            case (0x00, 0x01):
                instruction.ExtendedImplicit.Execute(this, word);
                break;
            
            case (_, _):
                instruction.ExtendedAddressing.Execute(this, m1, m2, word);
                break;
        }
    }
    
    /// [word: 1 bits] [opcode: 7 bits] [m1: 4 bits] [m2: 4 bits]
    public void Fetch(out u8 opcode, out u8 m1, out u8 m2, out bool word)
    { 
        u16 instruction = this.Bus.ReadWord(this.Pc);
        this.Pc += 2;
        
        InstructionEncoding.Decode(instruction, out opcode, out m1, out m2, out word);
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
        if (index < ChipRegisters.GENERIC_REGISTER_COUNT) return this._registers[index];
        
        if (index == ChipRegisters.AX) return this.Ax;
        if (index == ChipRegisters.AH) return (u16) (this.Ax >> 8);   // High byte of Ax
        if (index == ChipRegisters.AL) return (u16) (this.Ax & 0xFF); // Low byte of Ax
        
        if (index == ChipRegisters.DX) return this.Dx;
        if (index == ChipRegisters.DH) return (u16) (this.Dx >> 8); // High byte of Dx
        if (index == ChipRegisters.DL) return (u16) (this.Dx & 0xFF); // Low byte of Dx
        
        if (index == ChipRegisters.LX) return this.Lx;
        
        if (index == ChipRegisters.PC) return this.Pc;
        if (index == ChipRegisters.SP) return this.Sp;
        
        throw new InvalidOperationException($"Invalid register index: {index}");
    }
    
    public void SetRegister(u8 index, u16 value)
    {
        if (index < ChipRegisters.GENERIC_REGISTER_COUNT) this._registers[index] = value;
        
        else if (index == ChipRegisters.AX) this.Ax = value;
        else if (index == ChipRegisters.AH) this.Ax = (u16)((this.Ax & 0x00FF) | ((value & 0xFF) << 8)); // Set high byte of Ax
        else if (index == ChipRegisters.AL) this.Ax = (u16)((this.Ax & 0xFF00) | (value & 0xFF));        // Set low byte of Ax
        
        else if (index == ChipRegisters.DX) this.Dx = value;
        else if (index == ChipRegisters.DH) this.Dx = (u16)((this.Dx & 0x00FF) | ((value & 0xFF) << 8)); // Set high byte of Dx
        else if (index == ChipRegisters.DL) this.Dx = (u16)((this.Dx & 0xFF00) | (value & 0xFF));        // Set low byte of Dx
        
        else if (index == ChipRegisters.LX) this.Lx = value;
        
        else if (index == ChipRegisters.PC) this.Pc = value;
        else if (index == ChipRegisters.SP) this.Sp = value;
        
        else throw new InvalidOperationException($"Invalid register index: {index}");
    }
    
    public void Interrupt(u16 code)
    {
        if (code >= ChipLayout.IVT_COUNT)
            throw new InvalidOperationException($"Invalid interrupt code: {code}");
        
        this.Push(this.Pc);
        u16 interruptVectorAddress = (u16) (ChipLayout.IVT_START + code * 2);
        this.Pc = this.Bus.ReadWord(interruptVectorAddress);
    }
    
    public void RegisterInterruptHandler(u16 code, u16 handlerAddress)
    {
        if (code >= ChipLayout.IVT_COUNT)
            throw new InvalidOperationException($"Invalid interrupt code: {code}");
        
        u16 interruptVectorAddress = (u16) (ChipLayout.IVT_START + code * 2);
        this.Bus.WriteWord(interruptVectorAddress, handlerAddress);
    }
}