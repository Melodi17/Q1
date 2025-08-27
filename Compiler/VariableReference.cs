namespace Q1.Compiler;

using visitors;

public class VariableReference
{
    public int Address;
    public CType Type;
    
    public VariableReference(int address, CType type)
    {
        this.Address = address;
        this.Type    = type;
    }
}