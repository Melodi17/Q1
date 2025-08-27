namespace Q1.Compiler.visitors;

public class CType
{
    public string Name;
    public int    Size;

    public CType(string name, int size)
    {
        this.Name = name;
        this.Size = size;

        if (size > 2)
            throw new CompilerException("Types that take up more than 2 bytes are not supported");
    }

    public override string ToString() => this.Name;

    public static readonly CType Int  = new("int", 2);
    public static readonly CType Char = new("char", 1);
}

public class CTypePtr : CType
{
    public CType Sub;

    public CTypePtr(CType sub) : base($"{sub.Name}*",2)
    {
        this.Sub = sub;
    }
}