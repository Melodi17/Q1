namespace Q1.Compiler;

public class CCompilerVisitor : CGrammarBaseVisitor<string?>
{
    private int          _branchCount = 1;
    private List<string> _instructions = new();
    private string GetBranchLabel(string type)
    {
        return $"_{type}_{_branchCount++}";
    }
    private void Instruction(string instruction)
    {
        this._instructions.Add($"    {instruction}");
    }
    private void Blank()
    {
        this._instructions.Add("");
    }
    private void Label(string name)
    {
        this._instructions.Add($"{name}:");
    }
    
    public IEnumerable<string> Compile(CGrammarParser.ProgramContext program)
    {
        this.Visit(program);
        return this._instructions;
    }

    public override string? VisitFunction(CGrammarParser.FunctionContext context)
    {
        string functionName = context.ID().GetText();
        this.Label($"_{functionName}");
        this.VisitChildren(context);
        this.Blank();

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
                this.Instruction($"mov {expression}, V0");
                this.Instruction($"ret");
            }
            else
                throw new CompilerException("Return value was not an expression.");
        }
        else
            this.Instruction("ret");

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

        this.Instruction($"not {value}");
        return "LX";
    }

    public override string? VisitInvertExpression(CGrammarParser.InvertExpressionContext context)
    {
        string? value = this.Visit(context.expression());
        if (value == null)
            throw new CompilerException("Value for inversion was not an expression.");

        this.Instruction($"inv {value}");
        return "DX";
    }

    private void BaseBinaryOperation(CGrammarParser.ExpressionContext leftExpr, CGrammarParser.ExpressionContext rightExpr, string operation, string operationFriendlyName)
    {
        string? left = this.Visit(leftExpr);
        if (left == null)
            throw new CompilerException($"Left operand for {operationFriendlyName} was not an expression.");

        this.Instruction($"push {left}");

        string? right = this.Visit(rightExpr);
        if (right == null)
            throw new CompilerException($"Right operand for {operationFriendlyName} was not an expression.");

        this.Instruction($"mov {right}, V1");
        this.Instruction($"pop V0");
        this.Instruction($"{operation} V0, V1");
    }
    public override string? VisitAddExpression(CGrammarParser.AddExpressionContext context)
    {
        BaseBinaryOperation(context.left, context.right, "add", "addition");
        return "AX";
    }
    public override string? VisitSubtractExpression(CGrammarParser.SubtractExpressionContext context)
    {
        BaseBinaryOperation(context.left, context.right, "sub", "subtraction");
        return "AX";
    }
    public override string? VisitMultiplyExpression(CGrammarParser.MultiplyExpressionContext context)
    {
        BaseBinaryOperation(context.left, context.right, "mul", "multiplication");
        return "AX";
    }
    public override string? VisitDivideExpression(CGrammarParser.DivideExpressionContext context)
    {
        BaseBinaryOperation(context.left, context.right, "div", "division");
        return "AX";
    }
    public override string? VisitModulusExpression(CGrammarParser.ModulusExpressionContext context)
    {
        BaseBinaryOperation(context.left, context.right, "mod", "modulus");
        return "AX";
    }

    public override string? VisitEqualExpression(CGrammarParser.EqualExpressionContext context)
    {
        BaseBinaryOperation(context.left, context.right, "cmp", "equality check");
        return "LX";
    }

    public override string? VisitNotEqualExpression(CGrammarParser.NotEqualExpressionContext context)
    {
        BaseBinaryOperation(context.left, context.right, "cmp", "inequality check");
        this.Instruction($"not"); // lx is implicit
        return "LX";
    }

    public override string? VisitGreaterThanExpression(CGrammarParser.GreaterThanExpressionContext context)
    {
        BaseBinaryOperation(context.left, context.right, "gt", "greater than check");
        return "LX";
    }

    public override string? VisitLessThanExpression(CGrammarParser.LessThanExpressionContext context)
    {
        BaseBinaryOperation(context.left, context.right, "lt", "less than check");
        return "LX";
    }

    public override string? VisitLogicalOrExpression(CGrammarParser.LogicalOrExpressionContext context)
    {
        string? left = this.Visit(context.left);
        if (left == null)
            throw new CompilerException("Left operand for logical OR was not an expression.");

        string clauseBranch = this.GetBranchLabel("clause");
        string endBranch = this.GetBranchLabel("end");

        this.Instruction($"cmp {left}, 0");
        this.Instruction("bz"); // implicit LX
        this.Instruction($"jmp {clauseBranch}"); // only if bz is not 0
        this.Instruction("mov 1, V0");
        this.Instruction($"jmp {endBranch}");
        this.Label(clauseBranch);
        
        string? right = this.Visit(context.right);
        if (right == null)
            throw new CompilerException("Right operand for logical OR was not an expression.");
        
        this.Instruction($"cmp {right}, 0");
        this.Instruction("not");
        this.Instruction("mov LX, V0");
        this.Label(endBranch);

        return "V0";
    }
    public override string? VisitLogicalAndExpression(CGrammarParser.LogicalAndExpressionContext context)
    {
        string? left = this.Visit(context.left);
        if (left == null)
            throw new CompilerException("Left operand for logical OR was not an expression.");

        string clauseBranch = this.GetBranchLabel("clause");
        string endBranch = this.GetBranchLabel("end");
        
        this.Instruction($"cmp {left}, 0");
        this.Instruction("not");
        this.Instruction("bz");                  // implicit LX
        this.Instruction($"jmp {clauseBranch}"); // only if bz is not 0
        this.Instruction("mov 0, V0");
        this.Instruction($"jmp {endBranch}");
        this.Label(clauseBranch);
        
        string? right = this.Visit(context.right);
        if (right == null)
            throw new CompilerException("Right operand for logical OR was not an expression.");
        
        this.Instruction($"cmp {right}, 0");
        this.Instruction("not");
        this.Instruction("mov LX, V0");
        this.Label(endBranch);

        return "V0";
    }

    public override string? VisitGreaterThanOrEqualExpression(CGrammarParser.GreaterThanOrEqualExpressionContext context)
    {
        BaseBinaryOperation(context.left, context.right, "cmp", "equality check");
        string endBranch = this.GetBranchLabel("end");
        
        this.Instruction("bz");
        this.Instruction($"jmp {endBranch}");
        this.Instruction($"gt V0, V1");
        this.Label(endBranch);

        return "LX";
    }
    public override string? VisitLessThanOrEqualExpression(CGrammarParser.LessThanOrEqualExpressionContext context)
    {
        BaseBinaryOperation(context.left, context.right, "cmp", "equality check");
        string endBranch = this.GetBranchLabel("end");
        
        this.Instruction("bz");
        this.Instruction($"jmp {endBranch}");
        this.Instruction($"lt V0, V1");
        this.Label(endBranch);

        return "LX";
    }
    public override string? VisitBitwiseXorExpression(CGrammarParser.BitwiseXorExpressionContext context)
    {
        this.BaseBinaryOperation(context.left, context.right, "xor", "exclusive or");
        return "DX";
    }
    public override string? VisitBitwiseOrExpression(CGrammarParser.BitwiseOrExpressionContext context)
    {
        this.BaseBinaryOperation(context.left, context.right, "or", "or");
        return "DX";
    }
    public override string? VisitBitwiseAndExpression(CGrammarParser.BitwiseAndExpressionContext context)
    {
        this.BaseBinaryOperation(context.left, context.right, "and", "and");
        return "DX";
    }
}