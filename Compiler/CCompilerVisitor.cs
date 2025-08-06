namespace Q1.Compiler;

public class CCompilerVisitor : CGrammarBaseVisitor<string?>
{
    private List<string> Instructions = new();
    public IEnumerable<string> Compile(CGrammarParser.ProgramContext program)
    {
        this.Visit(program);
        return this.Instructions;
    }

    public override string? VisitFunction(CGrammarParser.FunctionContext context)
    {
        string functionName = context.ID().GetText();
        Instructions.Add($"_{functionName}:");
        this.VisitChildren(context);
        this.Instructions.Add("");

        return null;
    }
    public override string? VisitParenthesizedExpression(CGrammarParser.ParenthesizedExpressionContext context)
    {
        return this.Visit(context.expression());
    }
    public override string? VisitReturnStatement(CGrammarParser.ReturnStatementContext context)
    {
        if (context.expression() != null)
        {
            string? expression = this.Visit(context.expression());
            if (expression != null)
            {
                this.Instructions.Add($"MOV {expression}, V0");
                Instructions.Add($"RET");
            }
            else
                throw new CompilerException("Return value was not an expression.");
        }
        else
            Instructions.Add("RET");

        return null;
    }
    public override string? VisitIntConstant(CGrammarParser.IntConstantContext context)
    {
        string valueString = context.INT().GetText();
        if (int.TryParse(valueString, out int value))
            return $"{value}";
        else
            throw new CompilerException($"Invalid integer value: {valueString}");
    }

    public override string? VisitNotExpression(CGrammarParser.NotExpressionContext context)
    {
        string? value = this.Visit(context.expression());
        if (value == null)
            throw new CompilerException("Value for negating was not an expression.");

        this.Instructions.Add($"NOT {value}");
        return "LX";
    }

    public override string? VisitInvertExpression(CGrammarParser.InvertExpressionContext context)
    {
        string? value = this.Visit(context.expression());
        if (value == null)
            throw new CompilerException("Value for inversion was not an expression.");

        this.Instructions.Add($"INV {value}");
        return "DX";
    }

    public override string? VisitAddExpression(CGrammarParser.AddExpressionContext context)
    {
        string? left = this.Visit(context.left);
        string? right = this.Visit(context.right);

        if (left == null || right == null)
            throw new CompilerException("Left or right operand for addition was not an expression.");

        this.Instructions.Add($"ADD {left}, {right}");
        return "AX";
    }
    public override string? VisitSubtractExpression(CGrammarParser.SubtractExpressionContext context)
    {
        string? left = this.Visit(context.left);
        string? right = this.Visit(context.right);

        if (left == null || right == null)
            throw new CompilerException("Left or right operand for subtraction was not an expression.");

        this.Instructions.Add($"SUB {left}, {right}");
        return "AX";
    }
    public override string? VisitMultiplyExpression(CGrammarParser.MultiplyExpressionContext context)
    {
        string? left = this.Visit(context.left);
        string? right = this.Visit(context.right);

        if (left == null || right == null)
            throw new CompilerException("Left or right operand for multiplication was not an expression.");

        this.Instructions.Add($"MUL {left}, {right}");
        return "AX";
    }
    public override string? VisitDivideExpression(CGrammarParser.DivideExpressionContext context)
    {
        string? left = this.Visit(context.left);
        string? right = this.Visit(context.right);

        if (left == null || right == null)
            throw new CompilerException("Left or right operand for division was not an expression.");

        this.Instructions.Add($"DIV {left}, {right}");
        return "AX";
    }
    public override string? VisitModulusExpression(CGrammarParser.ModulusExpressionContext context)
    {
        string? left = this.Visit(context.left);
        string? right = this.Visit(context.right);

        if (left == null || right == null)
            throw new CompilerException("Left or right operand for modulus was not an expression.");

        this.Instructions.Add($"MOD {left}, {right}");
        return "AX";
    }

    public override string? VisitBitwiseXorExpression(CGrammarParser.BitwiseXorExpressionContext context)
    {
        throw new NotImplementedException();
    }
    public override string? VisitLessThanOrEqualExpression(CGrammarParser.LessThanOrEqualExpressionContext context)
    {
        throw new NotImplementedException();
    }
    public override string? VisitNotEqualExpression(CGrammarParser.NotEqualExpressionContext context)
    {
        throw new NotImplementedException();
    }
    public override string? VisitGreaterThanExpression(CGrammarParser.GreaterThanExpressionContext context)
    {
        throw new NotImplementedException();
    }
    public override string? VisitLogicalOrExpression(CGrammarParser.LogicalOrExpressionContext context)
    {
        throw new NotImplementedException();
    }
    public override string? VisitGreaterThanOrEqualExpression(CGrammarParser.GreaterThanOrEqualExpressionContext context)
    {
        throw new NotImplementedException();
    }
    public override string? VisitBitwiseOrExpression(CGrammarParser.BitwiseOrExpressionContext context)
    {
        throw new NotImplementedException();
    }
    public override string? VisitBitwiseAndExpression(CGrammarParser.BitwiseAndExpressionContext context)
    {
        throw new NotImplementedException();
    }
    public override string? VisitLessThanExpression(CGrammarParser.LessThanExpressionContext context)
    {
        throw new NotImplementedException();
    }
    public override string? VisitLogicalAndExpression(CGrammarParser.LogicalAndExpressionContext context)
    {
        throw new NotImplementedException();
    }
    public override string? VisitEqualExpression(CGrammarParser.EqualExpressionContext context)
    {
        throw new NotImplementedException();
    }
}