namespace Q1.Compiler;

using Antlr4.Runtime.Tree;
using Emulator;

public class CCompilerVisitor : CGrammarBaseVisitor<string?>
{
    private int                            _branchCount   = 1;
    private List<string>                   _instructions  = new();
    private Stack<Dictionary<string, int>> _variableStack = new();
    private int                            _memPointer    = Q1Layout.FreeMemoryStart;

    private const int ColumnWidth = 30;

    public readonly CommentCompilationMode CommentMode;
    
    public CCompilerVisitor(CommentCompilationMode commentMode)
    {
        this.CommentMode = commentMode;
    }

    private int FindVariable(string varName)
    {
        foreach (Dictionary<string, int> variables in this._variableStack)
        {
            if (variables.TryGetValue(varName, out int address))
                return address;
        }

        throw new CompilerException($"Variable {varName} could not be found in current scope.");
    }

    private int Alloc(int size)
    {
        int loc = this._memPointer;
        this._memPointer += size;

        return loc;
    }

    private string GenerateBranchLabel(string type)
    {
        return $"_{type}_{_branchCount++}";
    }
    private void Instruction(string instruction, string? generatedComment = null)
    {
        this._instructions.Add($"    {instruction}");
        if (generatedComment != null)
            this.CommentLastInstruction(generatedComment);
    }
    private void Blank()
    {
        this._instructions.Add("");
    }
    private void Label(string name, string? generatedComment = null)
    {
        this._instructions.Add($"{name}:");
        if (generatedComment != null)
            this.CommentLastInstruction(generatedComment);
    }
    private void UserComment(string comment)
    {
        if (this.CommentMode.HasFlag(CommentCompilationMode.UserDefined))
            this._instructions.Add($"    ; {comment}");
    }
    
    private void GeneratedComment(string comment)
    {
        if (this.CommentMode.HasFlag(CommentCompilationMode.Generated))
            this._instructions.Add($"{"".PadRight(CCompilerVisitor.ColumnWidth)}; {comment}");
    }

    private void CommentLastInstruction(string comment)
    {
        if (!this.CommentMode.HasFlag(CommentCompilationMode.Generated))
            return;

        if (this._instructions.Count == 0)
            throw new Exception("Cannot add comment to last instruction, there are none");

        string lastInstruction = this._instructions[^1];
        lastInstruction = $"{lastInstruction.PadRight(CCompilerVisitor.ColumnWidth)}; {comment}";

        this._instructions[^1] = lastInstruction;
    }


    public IEnumerable<string> Compile(CGrammarParser.ProgramContext program)
    {
        this.Visit(program);
        return this._instructions;
    }

    public override string? VisitFunction(CGrammarParser.FunctionContext context)
    {
        string functionName = context.name.Text;
        this._variableStack.Push(new());

        this.Label($"_{functionName}");
        this.VisitChildren(context);
        this.Instruction("mov 0, V0", "fallback return 0");
        this.Instruction("ret");
        this.Blank();

        this._variableStack.Pop();

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
                this.Instruction($"mov {expression}, V0", "return value");
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

        this.Instruction($"push {left}", "left operand");

        string? right = this.Visit(rightExpr);
        if (right == null)
            throw new CompilerException($"Right operand for {operationFriendlyName} was not an expression.");

        this.Instruction($"mov {right}, V1", "right operand");
        this.Instruction($"pop V0", "get left operand back");
        this.Instruction($"{operation} V0, V1", $"compute {operationFriendlyName}");
    }
    private string BaseBinaryCompoundOperation(CGrammarParser.TargetContext leftTarget, CGrammarParser.ExpressionContext rightExpr, string operation, string operationFriendlyName, string operationOutput)
    {
        string? left = this.Visit(leftTarget);
        if (left == null)
            throw new CompilerException($"Left operand for {operationFriendlyName} was not an expression.");

        string? right = this.Visit(rightExpr);
        if (right == null)
            throw new CompilerException($"Right operand for {operationFriendlyName} was not an expression.");

        // No need for stack maniupulation because left is a target and is assumed to stay constant
        this.Instruction($"{operation} {left}, {right}", $"compute {operationFriendlyName}");
        this.Instruction($"mov {operationOutput}, {left}", "store back into left param");
        return left;
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

        string clauseBranch = this.GenerateBranchLabel("clause");
        string endBranch = this.GenerateBranchLabel("end");

        this.Instruction($"cmp {left}, 0", "logical or");
        this.Instruction("bz");                  // implicit LX
        this.Instruction($"jmp {clauseBranch}"); // only if bz is not 0
        this.Instruction("mov 1, V0", "first operand succeeded, store 1 and skip second operand");
        this.Instruction($"jmp {endBranch}");
        this.Label(clauseBranch);

        string? right = this.Visit(context.right);
        if (right == null)
            throw new CompilerException("Right operand for logical OR was not an expression.");

        this.Instruction($"cmp {right}, 0", "compare result from second operand to 0");
        this.Instruction("not");
        this.Instruction("mov LX, V0", "store second operand result");
        this.Label(endBranch);

        return "V0";
    }
    public override string? VisitLogicalAndExpression(CGrammarParser.LogicalAndExpressionContext context)
    {
        string? left = this.Visit(context.left);
        if (left == null)
            throw new CompilerException("Left operand for logical OR was not an expression.");

        string clauseBranch = this.GenerateBranchLabel("clause");
        string endBranch = this.GenerateBranchLabel("end");

        this.Instruction($"cmp {left}, 0", "logical and");
        this.Instruction("not");
        this.Instruction("bz");                  // implicit LX
        this.Instruction($"jmp {clauseBranch}"); // only if bz is not 0
        this.Instruction("mov 0, V0", "first operand failed, store 0 and skip second");
        this.Instruction($"jmp {endBranch}");
        this.Label(clauseBranch);

        string? right = this.Visit(context.right);
        if (right == null)
            throw new CompilerException("Right operand for logical OR was not an expression.");

        this.Instruction($"cmp {right}, 0", "compare result from second operand to 0");
        this.Instruction("not");
        this.Instruction("mov LX, V0", "store second operand result");
        this.Label(endBranch);

        return "V0";
    }

    public override string? VisitGreaterThanOrEqualExpression(CGrammarParser.GreaterThanOrEqualExpressionContext context)
    {
        BaseBinaryOperation(context.left, context.right, "cmp", "equality check");
        string endBranch = this.GenerateBranchLabel("end");

        this.Instruction("bz");
        this.Instruction($"jmp {endBranch}");
        this.Instruction($"gt V0, V1");
        this.Label(endBranch);

        return "LX";
    }
    public override string? VisitLessThanOrEqualExpression(CGrammarParser.LessThanOrEqualExpressionContext context)
    {
        BaseBinaryOperation(context.left, context.right, "cmp", "equality check");
        string endBranch = this.GenerateBranchLabel("end");

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
    public override string? VisitBitwiseLeftShiftExpression(CGrammarParser.BitwiseLeftShiftExpressionContext context)
    {
        this.BaseBinaryOperation(context.left, context.right, "shl", "left shift");
        return "DX";
    }
    public override string? VisitBitwiseRightShiftExpression(CGrammarParser.BitwiseRightShiftExpressionContext context)
    {
        this.BaseBinaryOperation(context.left, context.right, "shr", "right shift");
        return "DX";
    }

    public override string? VisitVariableDeclaration(CGrammarParser.VariableDeclarationContext context)
    {
        int addr = this.Alloc(2);
        string name = context.name.Text;

        Dictionary<string, int> scope = this._variableStack.Peek();
        if (scope.ContainsKey(name))
            throw new CompilerException($"Variable {name} already exists in current scope.");

        scope[name] = addr;
        if (context.value != null)
        {
            string? initialValue = this.Visit(context.value)
                                   ?? throw new CompilerException("Initial variable definition value was not an expression");
            this.Instruction($"mov {initialValue}, [${addr:X4}]", $"int {name} = {initialValue}");
        }
        else
            this.Instruction($"mov 0, [${addr:X4}]", $"int {name}");

        return null;
    }
    public override string? VisitVariableExpression(CGrammarParser.VariableExpressionContext context)
    {
        string varName = context.ID().GetText();
        int addr = this.FindVariable(varName);

        return $"[${addr:X4}]";
    }

    public override string? VisitVariableTarget(CGrammarParser.VariableTargetContext context)
    {
        string varName = context.ID().GetText();
        int addr = this.FindVariable(varName);

        return $"[${addr:X4}]";
    }
    public override string? VisitAssignmentExpression(CGrammarParser.AssignmentExpressionContext context)
    {
        string? target = this.Visit(context.target());
        string? value = this.Visit(context.expression());

        this.Instruction($"mov {value}, {target}");
        return target;
    }

    public override string? VisitCompoundAddExpression(CGrammarParser.CompoundAddExpressionContext context)
    {
        return BaseBinaryCompoundOperation(context.target(),
            context.expression(),
            "add",
            "addition",
            "AX");
    }
    public override string? VisitCompoundSubtractExpression(CGrammarParser.CompoundSubtractExpressionContext context)
    {
        return BaseBinaryCompoundOperation(context.target(),
            context.expression(),
            "sub",
            "subtraction",
            "AX");
    }
    public override string? VisitCompoundMultiplyExpression(CGrammarParser.CompoundMultiplyExpressionContext context)
    {
        return BaseBinaryCompoundOperation(context.target(),
            context.expression(),
            "mult",
            "multiplication",
            "AX");
    }
    public override string? VisitCompoundDivideExpression(CGrammarParser.CompoundDivideExpressionContext context)
    {
        return BaseBinaryCompoundOperation(context.target(),
            context.expression(),
            "div",
            "division",
            "AX");
    }
    public override string? VisitCompoundModulusExpression(CGrammarParser.CompoundModulusExpressionContext context)
    {
        return BaseBinaryCompoundOperation(context.target(),
            context.expression(),
            "mod",
            "modulus",
            "AX");
    }

    public override string? VisitCompoundBitwiseOrExpression(CGrammarParser.CompoundBitwiseOrExpressionContext context)
    {
        return BaseBinaryCompoundOperation(context.target(),
            context.expression(),
            "or",
            "or",
            "DX");
    }
    public override string? VisitCompoundBitwiseXorExpression(CGrammarParser.CompoundBitwiseXorExpressionContext context)
    {
        return BaseBinaryCompoundOperation(context.target(),
            context.expression(),
            "xor",
            "exclusive or",
            "DX");
    }
    public override string? VisitCompoundBitwiseAndExpression(CGrammarParser.CompoundBitwiseAndExpressionContext context)
    {
        return BaseBinaryCompoundOperation(context.target(),
            context.expression(),
            "and",
            "and",
            "DX");
    }

    public override string? VisitCompoundBitwiseLeftShiftExpression(CGrammarParser.CompoundBitwiseLeftShiftExpressionContext context)
    {
        return BaseBinaryCompoundOperation(context.target(),
            context.expression(),
            "shl",
            "left shift",
            "DX");
    }
    public override string? VisitCompoundBitwiseRightShiftExpression(CGrammarParser.CompoundBitwiseRightShiftExpressionContext context)
    {
        return BaseBinaryCompoundOperation(context.target(),
            context.expression(),
            "shr",
            "right shift",
            "DX");
    }

    public override string? VisitIncrementPrefixExpression(CGrammarParser.IncrementPrefixExpressionContext context)
    {
        string target = this.Visit(context.target())
                        ?? throw new CompilerException("Increment target was not an expression");
        this.Instruction($"inc {target}", "++i");
        return target;
    }
    public override string? VisitDecrementPrefixExpression(CGrammarParser.DecrementPrefixExpressionContext context)
    {
        string target = this.Visit(context.target())
                        ?? throw new CompilerException("Decrement target was not an expression");
        this.Instruction($"dec {target}", "--i");
        return target;
    }
    public override string? VisitIncrementPostfixExpression(CGrammarParser.IncrementPostfixExpressionContext context)
    {
        string target = this.Visit(context.target())
                        ?? throw new CompilerException("Increment target was not an expression");
        this.Instruction($"push {target}", "i++");
        this.Instruction($"inc {target}");
        this.Instruction("pop V0");
        return "V0";
    }
    public override string? VisitDecrementPostfixExpression(CGrammarParser.DecrementPostfixExpressionContext context)
    {
        string target = this.Visit(context.target())
                        ?? throw new CompilerException("Decrement target was not an expression");
        this.Instruction($"push {target}", "i--");
        this.Instruction($"dec {target}");
        this.Instruction($"pop V0");
        return "V0";
    }

    public override string? VisitCommaExpression(CGrammarParser.CommaExpressionContext context)
    {
        this.Visit(context.left);
        return this.Visit(context.right);
    }

    public override string? VisitTernaryExpression(CGrammarParser.TernaryExpressionContext context)
    {
        string cond = this.Visit(context.cond)
                      ?? throw new CompilerException("Ternary condition was not an expression");

        string thenBranch = this.GenerateBranchLabel("then");
        string endBranch = this.GenerateBranchLabel("end");
        string result;

        // Else branch is first since it saves us from having to perform a NOT operation.
        // If cond, jump to then, else fall through to else, then jump to end

        this.Instruction($"bz {cond}", "check ternary condition");
        this.Instruction($"jmp {thenBranch}");
        
        result = this.Visit(context.@else)
                 ?? throw new CompilerException("Else branch was not an expression");
        this.Instruction($"mov {result}, V0", "store ternary fail result");
        this.Instruction($"jmp {endBranch}");

        this.Label(thenBranch);
        result = this.Visit(context.then)
                 ?? throw new CompilerException("Else branch was not an expression");
        this.Instruction($"mov {result}, V0", "store ternary success result");

        this.Label(endBranch);

        return "V0";
    }

    public override string? VisitIfStatement(CGrammarParser.IfStatementContext context)
    {
        string cond = this.Visit(context.expression())
                      ?? throw new CompilerException("If condition was not an expression");
        
        // Else branch is first since it saves us from having to perform a NOT operation.
        // If cond, jump to then, else fall through to else, then jump to end

        string thenBranch = this.GenerateBranchLabel("then");
        string endBranch = this.GenerateBranchLabel("end");
        
        this.Instruction($"bz {cond}", "check if condition");
        this.Instruction($"jmp {thenBranch}");

        this.GeneratedComment("if failed");
        if (context.@else != null)
            this.Visit(context.@else);
        this.Instruction($"jmp {endBranch}");
        
        this.Label(thenBranch, "if succeeded");
        this.Visit(context.then);
        
        this.Label(endBranch);
        
        return null;
    }

    public override string? VisitErrorNode(IErrorNode node)
    {
        if (node.Symbol.Type == CGrammarParser.COMMENT)
        {
            // strip // and \n and whitespace
            string comment = node.Symbol.Text
                .TrimStart("//").TrimEnd('\n').Trim();
            
            this.UserComment(comment);
        }
        
        return base.VisitErrorNode(node);
    }

    public override string? Visit(IParseTree tree)
    {
        return base.Visit(tree);
    }

    /// <summary>
    /// Basically identical to base version, except calls Visit on each child, making Visit not redundant.
    /// </summary>
    public override string? VisitChildren(IRuleNode node)
    {
        string? result = this.DefaultResult;
        int childCount = node.ChildCount;
        for (int i = 0; i < childCount && this.ShouldVisitNextChild(node, result); ++i)
        {
            string? nextResult = this.Visit(node.GetChild(i));
            result = this.AggregateResult(result, nextResult);
        }
        return result;
    }
}