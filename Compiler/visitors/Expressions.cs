namespace Q1.Compiler.visitors;

public partial class CCompilerVisitor
{
    private string VisitExpression(CGrammarParser.ExpressionContext expression)
    {
        string? expressionString = this.Visit(expression) as string;
        if (expressionString == null)
            throw new CompilerException("Expression did not return a string");

        return expressionString;
    }
    
    public override string VisitCallExpression(CGrammarParser.CallExpressionContext context)
    {
        string functionName = context.ID().GetText();

        FunctionReference function = this.GetFunctionReference(functionName);
        FunctionOverload overload = function.GetOverload(context._params.Count);

        // We reverse arugments so when we perform pop, its in the right order
        CGrammarParser.ExpressionContext[] argumentsReversed = context._params.Reverse().ToArray();

        for (int i = 0; i < argumentsReversed.Length; i++)
        {
            CGrammarParser.ExpressionContext argument = argumentsReversed[i];
            string result = this.VisitExpression(argument);

            this.Instruction($"push {result}", $"arg {overload.ParameterTypes[i]} {overload.ParameterNames[i]}");
        }

        this.Instruction($"call {function.GetBranchName(overload)}", function.GetBranchFriendlyName(overload));

        return "V0";
    }
    public override string VisitParenthesizedExpression(CGrammarParser.ParenthesizedExpressionContext context)
    {
        return this.VisitExpression(context.expression());
    }
    public override string VisitVariableExpression(CGrammarParser.VariableExpressionContext context)
    {
        string varName = context.ID().GetText();
        VariableReference reference = this.FindVariable(varName);

        return $"[${reference.Address:X4}]";
    }
    public override string VisitAssignmentExpression(CGrammarParser.AssignmentExpressionContext context)
    {
        Target target = this.VisitTarget(context.target());
        string value = this.VisitExpression(context.expression());

        this.DynamicMove(target.Type, value, target.Value, $"{target.Value} = {value}");
        return target.Value;
    }
    public override string VisitTernaryExpression(CGrammarParser.TernaryExpressionContext context)
    {
        string cond = this.VisitExpression(context.cond);

        string thenBranch = this.GenerateBranchLabel("then");
        string endBranch = this.GenerateBranchLabel("end");
        string result;

        // Else branch is first since it saves us from having to perform a NOT operation.
        // If cond, jump to then, else fall through to else, then jump to end

        this.Instruction($"bz {cond}", "check ternary condition");
        this.Instruction($"jmp {thenBranch}");

        result = this.VisitExpression(context.@else);
        this.Instruction($"mov {result}, V0", "store ternary fail result");
        this.Instruction($"jmp {endBranch}");

        this.Branch(thenBranch);
        result = this.VisitExpression(context.then);
        this.Instruction($"mov {result}, V0", "store ternary success result");

        this.Branch(endBranch);

        return "V0";
    }
    public override string VisitAddressOfExpression(CGrammarParser.AddressOfExpressionContext context)
    {
        Target target = this.VisitTarget(context.target());

        return target.Address;
    }
    public override string VisitDereferenceExpression(CGrammarParser.DereferenceExpressionContext context)
    {
        string value = this.VisitExpression(context.expression());

        this.Instruction($"mov {value}, V0", "move pointer into register for dereference");
        return "[V0]";
    }

    public override object? VisitIndexExpression(CGrammarParser.IndexExpressionContext context)
    {
        Target obj = this.VisitTarget(context.obj);
        if (obj.Type is not CTypePtr ptr)
            throw new CompilerException("Indexer target was not a pointer!");
        
        this.Instruction($"push {obj.Value}", "store index obj");

        string indexer = this.VisitExpression(context.indexer);

        this.Instruction($"mov {indexer}, V1",      "store indexer");
        this.Instruction($"pop V0",                 "restore index obj");
        this.Instruction($"mul V1, {ptr.Sub.Size}", $"multiply indexer by {ptr.Sub.Size}");
        this.Instruction($"add AX, V0",             "add indexer and index obj");
        this.Instruction($"mov AX, V0",             "move to general purpose register");
        return "[V0]";
    }

    public override object? VisitArrayExpression(CGrammarParser.ArrayExpressionContext context)
    {
        CType type = this.VisitCType(context.type());
        int? arrPtr = null;

        if (context._params.Count == 0)
            throw new CompilerException("Cannot create empty array");
        
        foreach (CGrammarParser.ExpressionContext? param in context._params)
        {
            int itemPtr = Alloc(type.Size);
            
            if (arrPtr == null)
                arrPtr = itemPtr;

            string value = this.VisitExpression(param);
            this.DynamicMove(type, value, $"[${itemPtr:X4}]");
        }

        return $"${arrPtr:X4}";
    }
}