namespace Q1.core.Arch.Constants;

public static class ChipRegisters
{
    public const u8 GENERIC_REGISTER_COUNT = 64;

    public const u8 AX = 0x40;
    public const u8 AH = 0x41;
    public const u8 AL = 0x42;
    
    public const u8 DX = 0x44;
    public const u8 DH = 0x45;
    public const u8 DL = 0x46;
    
    public const u8 LX = 0x48;
    
    public const u8 PC = 0x4C;
    public const u8 SP = 0x4D;
    
    public static u8? GetRegisterIndex(string name)
    {
        return name switch
        {
            "AX" => AX,
            "AH" => AH,
            "AL" => AL,
            "DX" => DX,
            "DH" => DH,
            "DL" => DL,
            "LX" => LX,
            "PC" => PC,
            "SP" => SP,
            _ when name.StartsWith("V") && byte.TryParse(name[1..], out u8 index) && index < GENERIC_REGISTER_COUNT => index,
            _ => null
        };
    }
    
    public static string? GetRegisterName(u8 index)
    {
        return index switch
        {
            AX => "AX",
            AH => "AH",
            AL => "AL",
            DX => "DX",
            DH => "DH",
            DL => "DL",
            LX => "LX",
            PC => "PC",
            SP => "SP",
            _ when index < GENERIC_REGISTER_COUNT => $"V{index}",
            _ => null
        };
    }
}