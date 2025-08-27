namespace Q1.Compiler.visitors;

public partial class CCompilerVisitor
{
    private void BaseBinaryOperation(CGrammarParser.ExpressionContext leftExpr, CGrammarParser.ExpressionContext rightExpr, string operation, string operationFriendlyName)
    {
        string left = this.VisitExpression(leftExpr);

        this.Instruction($"push {left}", "left operand");

        string right = this.VisitExpression(rightExpr);

        this.Instruction($"mov {right}, V1",    "right operand");
        this.Instruction($"pop V0",             "get left operand back");
        this.Instruction($"{operation} V0, V1", $"compute {operationFriendlyName}");
    }

    private string BaseBinaryCompoundOperation(CGrammarParser.TargetContext leftTarget, CGrammarParser.ExpressionContext rightExpr, string operation, string operationFriendlyName, string operationOutput)
    {
        Target left = this.VisitTarget(leftTarget);
        
        this.Instruction($"push {left.Value}", "left operand");

        string? right = this.VisitExpression(rightExpr);
        if (right == null)
            throw new CompilerException($"Right operand for {operationFriendlyName} was not an expression.");
        
        this.Instruction($"mov {right}, V1",    "right operand");
        this.Instruction($"pop V0",             "get left operand back");
        this.Instruction($"{operation} V0, V1", $"compute {operationFriendlyName}");
        
        this.Instruction($"push {operationOutput}", "computation results");
        
        left = this.VisitTarget(leftTarget);
        this.Instruction("pop V1", "get computation results back");
        this.DynamicMove(left.Type, "V1", left.Value, "store back into left param");
        
        return left.Value;
    }

    public override string VisitNotExpression(CGrammarParser.NotExpressionContext context)
    {
        string value = this.VisitExpression(context.expression());

        this.Instruction($"not {value}");
        return "LX";
    }
    public override string VisitInvertExpression(CGrammarParser.InvertExpressionContext context)
    {
        string value = this.VisitExpression(context.expression());

        this.Instruction($"inv {value}");
        return "DX";
    }
    public override string VisitAddExpression(CGrammarParser.AddExpressionContext context)
    {
        this.BaseBinaryOperation(context.left, context.right, "add", "addition");
        return "AX";
    }
    public override string VisitSubtractExpression(CGrammarParser.SubtractExpressionContext context)
    {
        this.BaseBinaryOperation(context.left, context.right, "sub", "subtraction");
        return "AX";
    }
    public override string VisitMultiplyExpression(CGrammarParser.MultiplyExpressionContext context)
    {
        this.BaseBinaryOperation(context.left, context.right, "mul", "multiplication");
        return "AX";
    }
    public override string VisitDivideExpression(CGrammarParser.DivideExpressionContext context)
    {
        this.BaseBinaryOperation(context.left, context.right, "div", "division");
        return "AX";
    }
    public override string VisitModulusExpression(CGrammarParser.ModulusExpressionContext context)
    {
        this.BaseBinaryOperation(context.left, context.right, "mod", "modulus");
        return "AX";
    }
    public override string VisitEqualExpression(CGrammarParser.EqualExpressionContext context)
    {
        this.BaseBinaryOperation(context.left, context.right, "cmp", "equality check");
        return "LX";
    }
    public override string VisitNotEqualExpression(CGrammarParser.NotEqualExpressionContext context)
    {
        this.BaseBinaryOperation(context.left, context.right, "cmp", "inequality check");
        this.Instruction($"not"); // lx is implicit
        return "LX";
    }
    public override string VisitGreaterThanExpression(CGrammarParser.GreaterThanExpressionContext context)
    {
        this.BaseBinaryOperation(context.left, context.right, "gt", "greater than check");
        return "LX";
    }
    public override string VisitLessThanExpression(CGrammarParser.LessThanExpressionContext context)
    {
        this.BaseBinaryOperation(context.left, context.right, "lt", "less than check");
        return "LX";
    }
    public override string VisitLogicalOrExpression(CGrammarParser.LogicalOrExpressionContext context)
    {
        string left = this.VisitExpression(context.left);

        string clauseBranch = this.GenerateBranchLabel("clause");
        string endBranch = this.GenerateBranchLabel("end");

        this.Instruction($"cmp {left}, 0", "logical or");
        this.Instruction("bz");                  // implicit LX
        this.Instruction($"jmp {clauseBranch}"); // only if bz is not 0
        this.Instruction("mov 1, V0", "first operand succeeded, store 1 and skip second operand");
        this.Instruction($"jmp {endBranch}");
        this.Branch(clauseBranch);

        string right = this.VisitExpression(context.right);

        this.Instruction($"cmp {right}, 0", "compare result from second operand to 0");
        this.Instruction("not");
        this.Instruction("mov LX, V0", "store second operand result");
        this.Branch(endBranch);

        return "V0";
    }
    public override string VisitLogicalAndExpression(CGrammarParser.LogicalAndExpressionContext context)
    {
        string left = this.VisitExpression(context.left);

        string clauseBranch = this.GenerateBranchLabel("clause");
        string endBranch = this.GenerateBranchLabel("end");

        this.Instruction($"cmp {left}, 0", "logical and");
        this.Instruction("not");
        this.Instruction("bz");                  // implicit LX
        this.Instruction($"jmp {clauseBranch}"); // only if bz is not 0
        this.Instruction("mov 0, V0", "first operand failed, store 0 and skip second");
        this.Instruction($"jmp {endBranch}");
        this.Branch(clauseBranch);

        string right = this.VisitExpression(context.right);

        this.Instruction($"cmp {right}, 0", "compare result from second operand to 0");
        this.Instruction("not");
        this.Instruction("mov LX, V0", "store second operand result");
        this.Branch(endBranch);

        return "V0";
    }
    public override string VisitGreaterThanOrEqualExpression(CGrammarParser.GreaterThanOrEqualExpressionContext context)
    {
        this.BaseBinaryOperation(context.left, context.right, "cmp", "equality check");
        string endBranch = this.GenerateBranchLabel("end");

        this.Instruction("bz");
        this.Instruction($"jmp {endBranch}");
        this.Instruction($"gt V0, V1");
        this.Branch(endBranch);

        return "LX";
    }
    public override string VisitLessThanOrEqualExpression(CGrammarParser.LessThanOrEqualExpressionContext context)
    {
        this.BaseBinaryOperation(context.left, context.right, "cmp", "equality check");
        string endBranch = this.GenerateBranchLabel("end");

        this.Instruction("bz");
        this.Instruction($"jmp {endBranch}");
        this.Instruction($"lt V0, V1");
        this.Branch(endBranch);

        return "LX";
    }
    public override string VisitBitwiseXorExpression(CGrammarParser.BitwiseXorExpressionContext context)
    {
        this.BaseBinaryOperation(context.left, context.right, "xor", "exclusive or");
        return "DX";
    }
    public override string VisitBitwiseOrExpression(CGrammarParser.BitwiseOrExpressionContext context)
    {
        this.BaseBinaryOperation(context.left, context.right, "or", "or");
        return "DX";
    }
    public override string VisitBitwiseAndExpression(CGrammarParser.BitwiseAndExpressionContext context)
    {
        this.BaseBinaryOperation(context.left, context.right, "and", "and");
        return "DX";
    }
    public override string VisitBitwiseLeftShiftExpression(CGrammarParser.BitwiseLeftShiftExpressionContext context)
    {
        this.BaseBinaryOperation(context.left, context.right, "shl", "left shift");
        return "DX";
    }
    public override string VisitBitwiseRightShiftExpression(CGrammarParser.BitwiseRightShiftExpressionContext context)
    {
        this.BaseBinaryOperation(context.left, context.right, "shr", "right shift");
        return "DX";
    }
    public override string VisitCompoundAddExpression(CGrammarParser.CompoundAddExpressionContext context)
    {
        return this.BaseBinaryCompoundOperation(context.target(),
            context.expression(),
            "add",
            "addition",
            "AX");
    }
    public override string VisitCompoundSubtractExpression(CGrammarParser.CompoundSubtractExpressionContext context)
    {
        return this.BaseBinaryCompoundOperation(context.target(),
            context.expression(),
            "sub",
            "subtraction",
            "AX");
    }
    public override string VisitCompoundMultiplyExpression(CGrammarParser.CompoundMultiplyExpressionContext context)
    {
        return this.BaseBinaryCompoundOperation(context.target(),
            context.expression(),
            "mul",
            "multiplication",
            "AX");
    }
    public override string VisitCompoundDivideExpression(CGrammarParser.CompoundDivideExpressionContext context)
    {
        return this.BaseBinaryCompoundOperation(context.target(),
            context.expression(),
            "div",
            "division",
            "AX");
    }
    public override string VisitCompoundModulusExpression(CGrammarParser.CompoundModulusExpressionContext context)
    {
        return this.BaseBinaryCompoundOperation(context.target(),
            context.expression(),
            "mod",
            "modulus",
            "AX");
    }
    public override string VisitCompoundBitwiseOrExpression(CGrammarParser.CompoundBitwiseOrExpressionContext context)
    {
        return this.BaseBinaryCompoundOperation(context.target(),
            context.expression(),
            "or",
            "or",
            "DX");
    }
    public override string VisitCompoundBitwiseXorExpression(CGrammarParser.CompoundBitwiseXorExpressionContext context)
    {
        return this.BaseBinaryCompoundOperation(context.target(),
            context.expression(),
            "xor",
            "exclusive or",
            "DX");
    }
    public override string VisitCompoundBitwiseAndExpression(CGrammarParser.CompoundBitwiseAndExpressionContext context)
    {
        return this.BaseBinaryCompoundOperation(context.target(),
            context.expression(),
            "and",
            "and",
            "DX");
    }
    public override string VisitCompoundBitwiseLeftShiftExpression(CGrammarParser.CompoundBitwiseLeftShiftExpressionContext context)
    {
        return this.BaseBinaryCompoundOperation(context.target(),
            context.expression(),
            "shl",
            "left shift",
            "DX");
    }
    public override string VisitCompoundBitwiseRightShiftExpression(CGrammarParser.CompoundBitwiseRightShiftExpressionContext context)
    {
        return this.BaseBinaryCompoundOperation(context.target(),
            context.expression(),
            "shr",
            "right shift",
            "DX");
    }
    public override string VisitIncrementPrefixExpression(CGrammarParser.IncrementPrefixExpressionContext context)
    {
        Target target = this.VisitTarget(context.target());
        this.Instruction($"inc {target.Value}", "++i");
        return target.Value;
    }
    public override string VisitDecrementPrefixExpression(CGrammarParser.DecrementPrefixExpressionContext context)
    {
        Target target = this.VisitTarget(context.target());
        this.Instruction($"dec {target.Value}", "--i");
        return target.Value;
    }
    public override string VisitIncrementPostfixExpression(CGrammarParser.IncrementPostfixExpressionContext context)
    {
        Target target = this.VisitTarget(context.target());
        this.Instruction($"push {target.Value}", "i++");
        this.Instruction($"inc {target.Value}");
        this.Instruction("pop V0");
        return "V0";
    }
    public override string VisitDecrementPostfixExpression(CGrammarParser.DecrementPostfixExpressionContext context)
    {
        Target target = this.VisitTarget(context.target());
        this.Instruction($"push {target.Value}", "i--");
        this.Instruction($"dec {target.Value}");
        this.Instruction($"pop V0");
        return "V0";
    }
}