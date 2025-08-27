namespace Q1.Compiler.visitors;

public partial class CCompilerVisitor
{
    public override string? VisitBlock(CGrammarParser.BlockContext context)
    {
        this.EnterScope();

        this.VisitChildren(context);

        this.ExitScope();

        return null;
    }
    public override string? VisitReturnStatement(CGrammarParser.ReturnStatementContext context)
    {
        if (context.expression() != null)
        {
            string expression = this.VisitExpression(context.expression());
            this.Instruction($"mov {expression}, V0", "return value");
            this.Instruction($"ret");
        }
        else
            this.Instruction("ret");

        return null;
    }
    public override string? VisitIfStatement(CGrammarParser.IfStatementContext context)
    {
        string cond = this.VisitExpression(context.expression());
        string endBranch = this.GenerateBranchLabel("end");

        if (context.@else != null)
        {
            this.Instruction($"bz {cond}", "check if condition");
            // Else branch is first since it saves us from having to perform a NOT operation.
            // If cond, jump to then, else fall through to else, then jump to end

            string thenBranch = this.GenerateBranchLabel("then");

            this.Instruction($"jmp {thenBranch}");

            this.GeneratedComment("if failed");
            if (context.@else != null)
                this.Visit(context.@else);
            this.Instruction($"jmp {endBranch}");

            this.Branch(thenBranch, "if succeeded");
            this.Visit(context.then);
        }
        else
        {
            this.Instruction($"not {cond}");
            this.Instruction($"bz", "check if condition");
            this.Instruction($"jmp {endBranch}", "if succeeds, continue, else jump to end");
            this.Visit(context.then);
        }

        this.Branch(endBranch);

        return null;
    }
    public override string? VisitForExprStatement(CGrammarParser.ForExprStatementContext context)
    {
        string bodyBranch = this.GenerateBranchLabel("body");
        string endBranch = this.GenerateBranchLabel("end");

        context.LoopBodyBranch = bodyBranch;
        context.LoopEndBranch  = endBranch;

        this.EnterScope();
        this.Visit(context.initializer);

        this.Branch(bodyBranch);
        string cond = this.VisitExpression(context.cond);

        this.Instruction($"not {cond}");
        this.Instruction($"bz");
        this.Instruction($"jmp {endBranch}", "if for condition fails, jump to end");

        this.Visit(context.body);
        this.Visit(context.post);

        this.Branch(endBranch);
        this.ExitScope();

        return null;
    }
    public override string? VisitForDeclStatement(CGrammarParser.ForDeclStatementContext context)
    {
        string bodyBranch = this.GenerateBranchLabel("body");
        string endBranch = this.GenerateBranchLabel("end");

        context.LoopBodyBranch = bodyBranch;
        context.LoopEndBranch  = endBranch;

        this.EnterScope();
        this.Visit(context.initializer);

        this.Branch(bodyBranch);
        string cond = this.VisitExpression(context.cond);

        this.Instruction($"not {cond}");
        this.Instruction($"bz");
        this.Instruction($"jmp {endBranch}", "if for condition fails, jump to end");

        this.Visit(context.body);
        this.Visit(context.post);
        
        this.Instruction($"jmp {bodyBranch}");

        this.Branch(endBranch);
        this.ExitScope();

        return null;
    }
    public override string? VisitWhileStatement(CGrammarParser.WhileStatementContext context)
    {
        string bodyBranch = this.GenerateBranchLabel("body");
        string endBranch = this.GenerateBranchLabel("end");

        context.LoopBodyBranch = bodyBranch;
        context.LoopEndBranch  = endBranch;

        this.Branch(bodyBranch);
        string cond = this.VisitExpression(context.cond);

        this.Instruction($"not {cond}");
        this.Instruction($"bz");
        this.Instruction($"jmp {endBranch}", "if while condition fails, jump to end");

        this.Visit(context.body);
        this.Instruction($"jmp {bodyBranch}");

        this.Branch(endBranch);

        return null;
    }
    public override string? VisitDoWhileStatement(CGrammarParser.DoWhileStatementContext context)
    {
        string bodyBranch = this.GenerateBranchLabel("body");
        string endBranch = this.GenerateBranchLabel("end");

        context.LoopBodyBranch = bodyBranch;
        context.LoopEndBranch  = endBranch;

        this.Branch(bodyBranch);
        this.Visit(context.body);

        string cond = this.VisitExpression(context.cond);

        this.Instruction($"bz {cond}");
        this.Instruction($"jmp {bodyBranch}", "if while condition succeeds, jump back to start");

        this.Branch(endBranch);

        return null;
    }
    public override string? VisitBreakStatement(CGrammarParser.BreakStatementContext context)
    {
        if (context.LoopEndBranch == null)
            throw new CompilerException("Cannot break, not inside loop with end");

        this.Instruction($"jmp {context.LoopEndBranch}", "break");

        return null;
    }
    public override string? VisitContinueStatement(CGrammarParser.ContinueStatementContext context)
    {
        if (context.LoopBodyBranch == null)
            throw new CompilerException("Cannot skip body (continue), not inside loop with body");

        this.Instruction($"jmp {context.LoopBodyBranch}", "continue");

        return null;
    }
}