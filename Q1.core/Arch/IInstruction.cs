namespace Q1.core.Arch;

public interface IInstruction
{
    public string Name { get; }
}

public record struct ImplicitInstruction : IInstruction
{
    public string Name { get; set; }
    
    public Action<Chip, bool> Execute;
}

public record struct SimpleInstruction : IInstruction
{
    public string Name { get; set; }
    
    public Action<Chip, u8, bool> Execute;
}

public record struct ExtendedInstruction : IInstruction
{
    public string Name { get; set; }
    
    public Action<Chip, u8, u8, bool> Execute;
}

public record struct InstructionGroup
{
    public ImplicitInstruction Implicit;
    public SimpleInstruction SimpleAddressing;
    public ImplicitInstruction ExtendedImplicit;
    public ExtendedInstruction ExtendedAddressing;
}