namespace Q1.Compiler.visitors;

using System.Text;

public partial class CCompilerVisitor
{
    public Encoding Encoding;
    public override string VisitIntConstant(CGrammarParser.IntConstantContext context)
    {
        string valueString = context.INT().GetText();
        if (int.TryParse(valueString, out int value))
            return $"{value}";
        else
            throw new CompilerException($"Invalid integer value: {valueString}");
    }
    public override string VisitHexConstant(CGrammarParser.HexConstantContext context)
    {
        string valueString = context.HEX().GetText();
        valueString = valueString[2..]; // Remove 0x
        return $"${valueString}";
    }

    public override object? VisitCharConstant(CGrammarParser.CharConstantContext context)
    {
        char c = context.LETTER().GetText()[1];
        return $"{this.Encoding.GetBytes(c.ToString())[0]}";
    }

    public override object? VisitStringConstant(CGrammarParser.StringConstantContext context)
    {
        string text = context.STRING().GetText()[1..^1];
        
        int? arrPtr = null;

        if (text.Length == 0)
            throw new CompilerException("Cannot create empty string");
        
        foreach (char c in text)
        {
            int itemPtr = Alloc(CType.Int.Size);
            
            if (arrPtr == null)
                arrPtr = itemPtr;

            this.DynamicMove(CType.Int, $"{this.Encoding.GetBytes(c.ToString())[0]}", $"[${itemPtr:X4}]");
        }

        return $"${arrPtr:X4}";
    }
}