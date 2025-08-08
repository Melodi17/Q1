namespace Q1.Compiler;

public class VariableReference
{
    public int Address;
    public int Size;
    
    public VariableReference(int address, int size)
    {
        this.Address = address;
        this.Size    = size;
    }
}