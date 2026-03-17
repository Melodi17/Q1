namespace Q1.core.Arch;

using Components;

public static class AddressingModes
{
    public static AddressingMode[] Lookup = new AddressingMode[8];
    
    static AddressingModes()
    {
        AddressingModes.Lookup[0x00] = new AddressingMode
        {
            Name = "Implicit",
            SizeB = 0,
            SizeW = 0,
            LoadByte = AddressingModes.ImplicitLoadByte,
            LoadWord = AddressingModes.ImplicitLoadWord,
            StoreByte = AddressingModes.ImplicitStoreByte,
            StoreWord = AddressingModes.ImplicitStoreWord
        };
        
        AddressingModes.Lookup[0x01] = new AddressingMode
        {
            Name = "Immediate",
            SizeB = 1,
            SizeW = 2,
            LoadByte = AddressingModes.ImmediateLoadByte,
            LoadWord = AddressingModes.ImmediateLoadWord,
            StoreByte = AddressingModes.ImmediateStoreByte,
            StoreWord = AddressingModes.ImmediateStoreWord
        };
        
        AddressingModes.Lookup[0x02] = new AddressingMode
        {
            Name = "Direct",
            SizeB = 2,
            SizeW = 2,
            LoadByte = AddressingModes.DirectLoadByte,
            LoadWord = AddressingModes.DirectLoadWord,
            StoreByte = AddressingModes.DirectStoreByte,
            StoreWord = AddressingModes.DirectStoreWord
        };
        
        AddressingModes.Lookup[0x03] = new AddressingMode
        {
            Name = "Indirect",
            SizeB = 2,
            SizeW = 2,
            LoadByte = AddressingModes.IndirectLoadByte,
            LoadWord = AddressingModes.IndirectLoadWord,
            StoreByte = AddressingModes.IndirectStoreByte,
            StoreWord = AddressingModes.IndirectStoreWord
        };
        
        AddressingModes.Lookup[0x04] = new AddressingMode
        {
            Name = "Relative",
            SizeB = 2,
            SizeW = 2,
            LoadByte = AddressingModes.RelativeLoadByte,
            LoadWord = AddressingModes.RelativeLoadWord,
            StoreByte = AddressingModes.RelativeStoreByte,
            StoreWord = AddressingModes.RelativeStoreWord
        };
        
        AddressingModes.Lookup[0x05] = new AddressingMode
        {
            Name = "Register",
            SizeB = 1,
            SizeW = 1,
            LoadByte = AddressingModes.RegisterLoadByte,
            LoadWord = AddressingModes.RegisterLoadWord,
            StoreByte = AddressingModes.RegisterStoreByte,
            StoreWord = AddressingModes.RegisterStoreWord
        };
        
        AddressingModes.Lookup[0x06] = new AddressingMode
        {
            Name = "Register Indirect",
            SizeB = 1,
            SizeW = 1,
            LoadByte = AddressingModes.RegisterIndirectLoadByte,
            LoadWord = AddressingModes.RegisterIndirectLoadWord,
            StoreByte = AddressingModes.RegisterIndirectStoreByte,
            StoreWord = AddressingModes.RegisterIndirectStoreWord
        };

        AddressingModes.Lookup[0x07] = new AddressingMode
        {
            Name = "Register Indirect with Offset",
            SizeB = 3,
            SizeW = 3,
            LoadByte = AddressingModes.RegisterIndirectOffsetLoadByte,
            LoadWord = AddressingModes.RegisterIndirectOffsetLoadWord,
            StoreByte = AddressingModes.RegisterIndirectOffsetStoreByte,
            StoreWord = AddressingModes.RegisterIndirectOffsetStoreWord
        };
    }

    private static u8 ImplicitLoadByte(Chip chip)
        => throw new InvalidOperationException("Cannot load from an implicit value");
    private static u16 ImplicitLoadWord(Chip chip)
        => throw new InvalidOperationException("Cannot load from an implicit value");
    private static void ImplicitStoreByte(Chip chip, u8 value)
        => throw new InvalidOperationException("Cannot store to an implicit value");
    private static void ImplicitStoreWord(Chip chip, u16 value)
        => throw new InvalidOperationException("Cannot store to an implicit value");


    private static u8 ImmediateLoadByte(Chip chip)
    {
        u8 value = chip.Bus.Read(chip.Pc);
        chip.Pc += 1;
        return value;
    }
    private static u16 ImmediateLoadWord(Chip chip)
    {
        u16 value = chip.Bus.ReadWord(chip.Pc);
        chip.Pc += 2;
        return value;
    }
    private static void ImmediateStoreByte(Chip chip, u8 value)
        => throw new InvalidOperationException("Cannot store to an immediate value");
    private static void ImmediateStoreWord(Chip chip, u16 value)
        => throw new InvalidOperationException("Cannot store to an immediate value");


    private static u8 DirectLoadByte(Chip chip)
    {
        u16 address = chip.Bus.ReadWord(chip.Pc);
        chip.Pc += 2;

        return chip.Bus.Read(address);
    }
    private static u16 DirectLoadWord(Chip chip)
    {
        u16 address = chip.Bus.ReadWord(chip.Pc);
        chip.Pc += 2;

        return chip.Bus.ReadWord(address);
    }
    private static void DirectStoreByte(Chip chip, u8 value)
    {
        u16 address = chip.Bus.ReadWord(chip.Pc);
        chip.Pc += 2;

        chip.Bus.Write(address, value);
    }
    private static void DirectStoreWord(Chip chip, u16 value)
    {
        u16 address = chip.Bus.ReadWord(chip.Pc);
        chip.Pc += 2;

        chip.Bus.WriteWord(address, value);
    }


    private static u8 IndirectLoadByte(Chip chip)
    {
        u16 pointer = chip.Bus.ReadWord(chip.Pc);
        chip.Pc += 2;

        u16 address = chip.Bus.ReadWord(pointer);
        return chip.Bus.Read(address);
    }
    private static u16 IndirectLoadWord(Chip chip)
    {
        u16 pointer = chip.Bus.ReadWord(chip.Pc);
        chip.Pc += 2;

        u16 address = chip.Bus.ReadWord(pointer);
        return chip.Bus.ReadWord(address);
    }
    private static void IndirectStoreByte(Chip chip, u8 value)
    {
        u16 pointer = chip.Bus.ReadWord(chip.Pc);
        chip.Pc += 2;

        u16 address = chip.Bus.ReadWord(pointer);
        chip.Bus.Write(address, value);
    }
    private static void IndirectStoreWord(Chip chip, u16 value)
    {
        u16 pointer = chip.Bus.ReadWord(chip.Pc);
        chip.Pc += 2;

        u16 address = chip.Bus.ReadWord(pointer);
        chip.Bus.WriteWord(address, value);
    }


    private static u8 RelativeLoadByte(Chip chip)
    {
        i16 offset = (i16) chip.Bus.ReadWord(chip.Pc);
        chip.Pc += 2;

        u16 address = (u16) (chip.Pc + offset);
        return chip.Bus.Read(address);
    }
    private static u16 RelativeLoadWord(Chip chip)
    {
        i16 offset = (i16) chip.Bus.ReadWord(chip.Pc);
        chip.Pc += 2;

        u16 address = (u16) (chip.Pc + offset);
        return chip.Bus.ReadWord(address);
    }
    private static void RelativeStoreByte(Chip chip, u8 value)
    {
        i16 offset = (i16) chip.Bus.ReadWord(chip.Pc);
        chip.Pc += 2;

        u16 address = (u16) (chip.Pc + offset);
        chip.Bus.Write(address, value);
    }
    private static void RelativeStoreWord(Chip chip, u16 value)
    {
        i16 offset = (i16) chip.Bus.ReadWord(chip.Pc);
        chip.Pc += 2;

        u16 address = (u16) (chip.Pc + offset);
        chip.Bus.WriteWord(address, value);
    }
    
    private static u8 RegisterLoadByte(Chip chip)
    {
        u8 regIndex = chip.Bus.Read(chip.Pc);
        chip.Pc += 1;

        return (u8) (chip.GetRegister(regIndex) & 0xFF);
    }
    private static u16 RegisterLoadWord(Chip chip)
    {
        u8 regIndex = chip.Bus.Read(chip.Pc);
        chip.Pc += 1;

        return chip.GetRegister(regIndex);
    }
    private static void RegisterStoreByte(Chip chip, u8 value)
    {
        u8 regIndex = chip.Bus.Read(chip.Pc);
        chip.Pc += 1;

        u16 current = chip.GetRegister(regIndex);
        u16 newValue = (u16)((current & 0xFF00) | value);
        chip.SetRegister(regIndex, newValue);
    }
    private static void RegisterStoreWord(Chip chip, u16 value)
    {
        u8 regIndex = chip.Bus.Read(chip.Pc);
        chip.Pc += 1;

        chip.SetRegister(regIndex, value);
    }
    
    private static u8 RegisterIndirectLoadByte(Chip chip)
    {
        u8 regIndex = chip.Bus.Read(chip.Pc);
        chip.Pc += 1;

        u16 address = chip.GetRegister(regIndex);
        return chip.Bus.Read(address);
    }
    private static u16 RegisterIndirectLoadWord(Chip chip)
    {
        u8 regIndex = chip.Bus.Read(chip.Pc);
        chip.Pc += 1;

        u16 address = chip.GetRegister(regIndex);
        return chip.Bus.ReadWord(address);
    }
    private static void RegisterIndirectStoreByte(Chip chip, u8 value)
    {
        u8 regIndex = chip.Bus.Read(chip.Pc);
        chip.Pc += 1;

        u16 address = chip.GetRegister(regIndex);
        chip.Bus.Write(address, value);
    }
    private static void RegisterIndirectStoreWord(Chip chip, u16 value)
    {
        u8 regIndex = chip.Bus.Read(chip.Pc);
        chip.Pc += 1;

        u16 address = chip.GetRegister(regIndex);
        chip.Bus.WriteWord(address, value);
    }
    
    private static u8 RegisterIndirectOffsetLoadByte(Chip chip)
    {
        u8 regIndex = chip.Bus.Read(chip.Pc);
        chip.Pc += 1;

        i16 offset = (i16) chip.Bus.ReadWord(chip.Pc);
        chip.Pc += 2;

        u16 address = (u16)(chip.GetRegister(regIndex) + offset);
        return chip.Bus.Read(address);
    }
    private static u16 RegisterIndirectOffsetLoadWord(Chip chip)
    {
        u8 regIndex = chip.Bus.Read(chip.Pc);
        chip.Pc += 1;

        i16 offset = (i16) chip.Bus.ReadWord(chip.Pc);
        chip.Pc += 2;

        u16 address = (u16)(chip.GetRegister(regIndex) + offset);
        return chip.Bus.ReadWord(address);
    }
    private static void RegisterIndirectOffsetStoreByte(Chip chip, u8 value)
    {
        u8 regIndex = chip.Bus.Read(chip.Pc);
        chip.Pc += 1;

        i16 offset = (i16) chip.Bus.ReadWord(chip.Pc);
        chip.Pc += 2;

        u16 address = (u16)(chip.GetRegister(regIndex) + offset);
        chip.Bus.Write(address, value);
    }
    private static void RegisterIndirectOffsetStoreWord(Chip chip, u16 value)
    {
        u8 regIndex = chip.Bus.Read(chip.Pc);
        chip.Pc += 1;

        i16 offset = (i16) chip.Bus.ReadWord(chip.Pc);
        chip.Pc += 2;

        u16 address = (u16)(chip.GetRegister(regIndex) + offset);
        chip.Bus.WriteWord(address, value);
    }
}