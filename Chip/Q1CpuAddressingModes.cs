namespace Q1Emu.Chip;

using System;
using System.Collections.Generic;

public partial class Q1Cpu
{
    public u16 FetchData(u8 mode)
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
    
    public void StoreData(u8 mode, u16 value)
    {
        switch (mode)
        {
            case 0x0:
                throw new InvalidOperationException("Cannot store data in implicit mode.");
            case 0x1:
                throw new InvalidOperationException("Cannot store data in immediate mode.");
            case 0x2:
                this.AddressingMode_Direct_Store(value);
                break;
            case 0x3:
                this.AddressingMode_Indirect_Store(value);
                break;
            case 0x4:
                this.AddressingMode_Relative_Store(value);
                break;
            case 0x5:
                this.AddressingMode_Register_Store(value);
                break;
            case 0x6:
                this.AddressingMode_RegisterIndirect_Store(value);
                break;
            case 0x7:
                this.AddressingMode_RegisterIndirectOffset_Store(value);
                break;
            default:
                throw new InvalidOperationException($"Unknown addressing mode: {mode:X2}");
        }
    }
    
    public void TransformData(u8 mode, Func<u16, u16> transform)
    {
        // if (mode != 0x05)
        // {
        //     u16 address = this.FetchData(mode);
        //     u16 value = this.Bus.ReadWord(address);
        //     u16 transformedValue = transform(value);
        //     this.Bus.WriteWord(address, transformedValue);
        // }
        // else
        // {
        //     u8 regIndex = this.Bus.Read(this.PC);
        //     this.PC += 1;
        //
        //     u16 value = this.FetchRegister(regIndex);
        //     u16 transformedValue = transform(value);
        //     this.StoreRegister(regIndex, transformedValue);
        // }
        
        throw new NotImplementedException("TransformData is not implemented yet.");
    }

    public u8 AddressingModeLength(u8 mode)
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
    
    public u16 AddressingMode_Implicit()
    {
        return 0;
    }


    public u16 AddressingMode_Immediate()
    {
        u16 value = this.Bus.ReadWord(this.PC);
        this.PC += 2;

        return value;
    }

    public u16 AddressingMode_Direct()
    {
        u16 address = this.Bus.ReadWord(this.PC);
        this.PC += 2;

        u16 value = this.Bus.ReadWord(address);
        return value;
    }

    public u16 AddressingMode_Indirect()
    {
        u16 address = this.Bus.ReadWord(this.PC);
        this.PC += 2;

        u16 indirectAddress = this.Bus.ReadWord(address);
        u16 value = this.Bus.ReadWord(indirectAddress);
        return value;
    }

    public u16 AddressingMode_Relative()
    {
        u16 offset = this.Bus.ReadWord(this.PC);
        this.PC += 2;

        u16 address = (u16) (this.PC + offset);
        return address;
    }

    public u16 AddressingMode_Register()
    {
        u8 regIndex = this.Bus.Read(this.PC);
        this.PC += 1;

        return this.FetchRegister(regIndex);
    }

    public u16 AddressingMode_RegisterIndirect()
    {
        u8 regIndex = this.Bus.Read(this.PC);
        this.PC += 1;

        u16 address = this.FetchRegister(regIndex);
        u16 value = this.Bus.ReadWord(address);
        Console.WriteLine($"Value at {address:X4}: {value:X4}");
        return value;
    }

    public u16 AddressingMode_RegisterIndirectOffset()
    {
        u8 regIndex = this.Bus.Read(this.PC);
        this.PC += 1;

        i16 offset = Make.i16(this.Bus.ReadSeq(this.PC, 2));
        Console.WriteLine("Offset: " + offset);
        this.PC += 2;

        u16 address = (u16) (this.FetchRegister(regIndex) + offset);
        u16 value = this.Bus.ReadWord(address);
        return value;
    }

    public void AddressingMode_Direct_Store(u16 data)
    {
        u16 address = this.Bus.ReadWord(this.PC);
        this.PC += 2;

        this.Bus.WriteWord(address, data);
    }

    public void AddressingMode_Indirect_Store(u16 data)
    {
        u16 address = this.Bus.ReadWord(this.PC);
        this.PC += 2;

        u16 indirectAddress = this.Bus.ReadWord(address);
        this.Bus.WriteWord(indirectAddress, data);
    }

    public void AddressingMode_Relative_Store(u16 data)
    {
        u16 offset = this.Bus.ReadWord(this.PC);
        this.PC += 2;

        u16 address = (u16) (this.PC + offset);
        this.Bus.WriteWord(address, data);
    }

    public void AddressingMode_Register_Store(u16 data)
    {
        u8 regIndex = this.Bus.Read(this.PC);
        this.PC += 1;
        
        this.StoreRegister(regIndex, data);
    }

    public void AddressingMode_RegisterIndirect_Store(u16 data)
    {
        u8 regIndex = this.Bus.Read(this.PC);
        this.PC += 1;

        u16 address = this.FetchRegister(regIndex);
        this.Bus.WriteWord(address, data);
    }

    public void AddressingMode_RegisterIndirectOffset_Store(u16 data)
    {
        u8 regIndex = this.Bus.Read(this.PC);
        this.PC += 1;

        i16 offset = Make.i16(this.Bus.ReadSeq(this.PC, 2));
        this.PC += 2;

        u16 address = (u16) (this.FetchRegister(regIndex) + offset);
        this.Bus.WriteWord(address, data);
    }
}