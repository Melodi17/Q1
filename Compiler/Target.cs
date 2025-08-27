namespace Q1.Compiler.visitors;

public class Target
{
    public string Value;
    public string Address;
    public CType  Type;

    public Target(CType type, string value, string address)
    {
        this.Value   = value;
        this.Address = address;
        this.Type    = type;
    }
}