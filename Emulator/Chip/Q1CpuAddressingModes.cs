namespace Q1.Emulator.Chip;

using System;

public partial class Q1Cpu
{
    public (Func<u16> read, Action<u16> write, Action<u8> writeByte) Address(u8 mode)
    {
        return mode switch
        {
            0x0 => this.AddressingMode_Implicit(),
            0x1 => this.AddressingMode_Immediate(),
            0x2 => this.AddressingMode_Direct(),
            0x3 => this.AddressingMode_Indirect(),
            0x4 => this.AddressingMode_Relative(),
            0x5 => this.AddressingMode_Register(),
            0x6 => this.AddressingMode_RegisterIndirect(),
            0x7 => this.AddressingMode_RegisterIndirectOffset(),
            _   => throw new InvalidOperationException($"Unknown addressing mode: {mode:X2}"),
        };
    }

    private u8 AddressingModeLength(u8 mode)
    {
        return mode switch
        {
            0x0 => 0, // Implicit
            0x1 => 2, // Immediate
            0x2 => 2, // Direct
            0x3 => 2, // Indirect
            0x4 => 2, // Relative
            0x5 => 1, // Register
            0x6 => 1, // Register indirect
            0x7 => 3, // Register indirect with offset
            _   => throw new InvalidOperationException($"Unknown addressing mode: {mode:X2}"),
        };
    }
    
    public (Func<u16> read, Action<u16> write, Action<u8> writeByte) AddressingMode_Implicit()
    {
        u16 Read() => throw new InvalidOperationException("Cannot read from implicit mode.");
        void Write(u16 value) => throw new InvalidOperationException("Cannot write to implicit mode.");
        void WriteByte(u8 value) => throw new InvalidOperationException("Cannot write byte to implicit mode.");
        
        return (Read, Write, WriteByte);
    }


    public (Func<u16> read, Action<u16> write, Action<u8> writeByte) AddressingMode_Immediate()
    {
        u16 value = this.Bus.ReadWord(this.PC);
        this.PC += 2;

        u16 Read() => value;
        void Write(u16 v) => throw new InvalidOperationException("Cannot write to immediate mode.");
        void WriteByte(u8 v) => throw new InvalidOperationException("Cannot write byte to immediate mode.");
        
        return (Read, Write, WriteByte);
    }

    public (Func<u16> read, Action<u16> write, Action<u8> writeByte) AddressingMode_Direct()
    {
        u16 address = this.Bus.ReadWord(this.PC);
        this.PC += 2;
        
        u16 Read() => this.Bus.ReadWord(address);
        void Write(u16 value) => this.Bus.WriteWord(address, value);
        void WriteByte(u8 value) => this.Bus.Write(address, value);
        
        return (Read, Write, WriteByte);
    }

    public (Func<u16> read, Action<u16> write, Action<u8> writeByte) AddressingMode_Indirect()
    {
        u16 address = this.Bus.ReadWord(this.PC);
        this.PC += 2;
        
        u16 Read()
        {
            u16 indirectAddress = this.Bus.ReadWord(address);
            u16 value = this.Bus.ReadWord(indirectAddress);
            return value;
        }

        void Write(u16 value)
        {
            u16 indirectAddress = this.Bus.ReadWord(address);
            this.Bus.WriteWord(indirectAddress, value);
        }
        
        void WriteByte(u8 value)
        {
            u16 indirectAddress = this.Bus.ReadWord(address);
            this.Bus.Write(indirectAddress, value);
        }
        
        return (Read, Write, WriteByte);
    }

    public (Func<u16> read, Action<u16> write, Action<u8> writeByte) AddressingMode_Relative()
    {
        u16 offset = this.Bus.ReadWord(this.PC);
        this.PC += 2;

        u16 address = (u16) (this.PC + offset);

        u16 Read() => address;
        void Write(u16 value) => throw new InvalidOperationException("Cannot write to relative mode.");
        void WriteByte(u8 value) => throw new InvalidOperationException("Cannot write byte to relative mode.");
        
        return (Read, Write, WriteByte);
    }

    public (Func<u16> read, Action<u16> write, Action<u8> writeByte) AddressingMode_Register()
    {
        u8 regIndex = this.Bus.Read(this.PC);
        this.PC += 1;
        
        u16 Read() => this.FetchRegister(regIndex);
        void Write(u16 value) => this.StoreRegister(regIndex, value);
        void WriteByte(u8 value)
        {
            u16 currentValue = this.FetchRegister(regIndex);
            u16 newValue = (u16) ((currentValue & 0xFF00) | (value & 0x00FF)); // Update low byte
            this.StoreRegister(regIndex, newValue);
        }
        
        return (Read, Write, WriteByte);
    }

    public (Func<u16> read, Action<u16> write, Action<u8> writeByte) AddressingMode_RegisterIndirect()
    {
        u8 regIndex = this.Bus.Read(this.PC);
        this.PC += 1;

        u16 Read()
        {
            u16 address = this.FetchRegister(regIndex);
            u16 value = this.Bus.ReadWord(address);
            return value;
        }
        
        void Write(u16 value)
        {
            u16 address = this.FetchRegister(regIndex);
            this.Bus.WriteWord(address, value);
        }
        
        void WriteByte(u8 value)
        {
            u16 address = this.FetchRegister(regIndex);
            this.Bus.Write(address, value);
        }
        
        return (Read, Write, WriteByte);
    }

    public (Func<u16> read, Action<u16> write, Action<u8> writeByte) AddressingMode_RegisterIndirectOffset()
    {
        u8 regIndex = this.Bus.Read(this.PC);
        this.PC += 1;

        u16 offset = this.Bus.ReadWord(this.PC);
        this.PC += 2;

        u16 Read()
        {
            u16 address = (u16) (this.FetchRegister(regIndex) + offset);
            u16 value = this.Bus.ReadWord(address);
            return value;
        }
        
        void Write(u16 value)
        {
            u16 address = (u16) (this.FetchRegister(regIndex) + offset);
            this.Bus.WriteWord(address, value);
        }
        
        void WriteByte(u8 value)
        {
            u16 address = (u16) (this.FetchRegister(regIndex) + offset);
            this.Bus.Write(address, value);
        }
        
        return (Read, Write, WriteByte);
    }
}