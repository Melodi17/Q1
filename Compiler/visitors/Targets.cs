namespace Q1.Compiler.visitors;

public partial class CCompilerVisitor
{
    private Target VisitTarget(CGrammarParser.TargetContext target)
    {
        Target? expressionString = this.Visit(target) as Target;
        if (expressionString == null)
            throw new CompilerException("Target did not return a target");

        return expressionString;
    }
    
    public override Target VisitVariableTarget(CGrammarParser.VariableTargetContext context)
    {
        string varName = context.ID().GetText();
        VariableReference reference = this.FindVariable(varName);

        return new Target(reference.Type, $"[${reference.Address:X4}]", $"${reference.Address:X4}");
    }

    public override object? VisitIndexerTarget(CGrammarParser.IndexerTargetContext context)
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
        
        return new Target(ptr.Sub, $"[V0]", $"V0");
    }
}