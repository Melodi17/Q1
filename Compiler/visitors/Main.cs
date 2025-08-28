namespace Q1.Compiler.visitors;

using System.Text;
using Antlr4.Runtime.Tree;
using Q1.Emulator;

public partial class CCompilerVisitor : CGrammarBaseVisitor<object?>
{
    private        int                                          _branchCount   = 1;
    private        List<string>                                 _instructions  = new();
    private        Stack<Dictionary<string, VariableReference>> _variableStack = new();
    private        List<FunctionReference>                      _methods       = new();
    private static int                                          _memLowest     = Q1Layout.FreeMemoryStart;
    private static int                                          _memPointer    = CCompilerVisitor._memLowest;

    private const int ColumnWidth = 30;

    public readonly CommentCompilationMode CommentMode;

    public CCompilerVisitor(CommentCompilationMode commentMode, Encoding encoding)
    {
        this.CommentMode = commentMode;
        this.Encoding = encoding;
    }

    private string GenerateBranchLabel(string type)
    {
        return $"_{type}_{this._branchCount++}";
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
    private void Branch(string name, string? generatedComment = null)
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
        this.Instruction("jmp _main");
        this.Visit(program);

        foreach ((CGrammarParser.FunctionDefinitionContext context, 
                     FunctionReference function, FunctionOverload overload, 
                     List<string> paramNames, List<CType> paramTypes) in this._functionBodies)
            this.VisitFunctionBody(context, function, overload, paramNames, paramTypes);

        return this._instructions;
    }

    private Dictionary<string, VariableReference> EnterScope()
    {
        Dictionary<string, VariableReference> scope = new();
        this._variableStack.Push(scope);

        return scope;
    }
    private void ExitScope()
    {
        foreach ((string key, VariableReference value) in this._variableStack.Peek())
        {
            // CCompilerVisitor._memPointer -= value.Type.Size;
            // if (CCompilerVisitor._memPointer < CCompilerVisitor._memLowest)
            //     throw new CompilerException("Memory pointer is lower than mem lowest!");
        }

        // Delete scope
        this._variableStack.Pop();
    }

    public void DynamicMove(CType type, string source, string dest, string? comment = null)
    {
        if (type.Size == 1)
            this.Instruction($"movb {source}, {dest}", comment);
        else if (type.Size == 2)
            this.Instruction($"mov {source}, {dest}", comment);
        else
            throw new CompilerException($"Cannot move {type.Size} bytes {source} to {dest}");
    }
    
    private CType VisitCType(CGrammarParser.TypeContext typeContext)
    {
        return GetCType(typeContext.GetText().Trim());
    }
    
    private CType GetCType(string type)
    {
        return type switch
        {
            "int"                     => CType.Int,
            "char"                    => CType.Char,
            _ when type.EndsWith("*") => new CTypePtr(this.GetCType(type[..^1])),
            _                         => throw new CompilerException($"Unknown CTYPE '{type}'")
        };
    }
    
    public override object? VisitErrorNode(IErrorNode node)
    {
        if (node.Symbol.Type == CGrammarParser.COMMENT)
        {
            // strip // and \n and whitespace
            string comment = node
                .Symbol.Text
                .TrimStart("//")
                .TrimEnd('\n')
                .Trim();

            this.UserComment(comment);
        }
        
        if (node.Symbol.Type == CGrammarParser.DIRECTIVE)
        {
            // strip // and \n and whitespace
            string directive = node
                .Symbol.Text
                .TrimStart("#")
                .TrimEnd('\n')
                .Trim();

            this.Instruction(directive);
        }

        return base.VisitErrorNode(node);
    }

    public override object? Visit(IParseTree tree)
    {
        if (tree is Context childCtx && tree.Parent is Context ctx)
            ctx.SyncProps(childCtx);

        return base.Visit(tree);
    }

    /// <summary>
    /// Basically identical to base version, except calls Visit on each child, making Visit not redundant.
    /// </summary>
    public override object? VisitChildren(IRuleNode node)
    {
        object? result = this.DefaultResult;
        int childCount = node.ChildCount;
        for (int i = 0; i < childCount && this.ShouldVisitNextChild(node, result); ++i)
        {
            object? nextResult = this.Visit(node.GetChild(i));
            result = this.AggregateResult(result, nextResult);
        }
        return result;
    }
}