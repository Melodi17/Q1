namespace Q1.Compiler;

using Antlr4.Runtime;

public class Context : ParserRuleContext
{
    public string? LoopEndBranch;
    public string? LoopBodyBranch;

    public Context() { }
    
    public Context(ParserRuleContext parent, int invokingStateNumber)
        : base((Context) parent, invokingStateNumber)
    {
    }
    
    
    public void SyncProps(Context childCtx)
    {
        childCtx.LoopBodyBranch = this.LoopBodyBranch;
        childCtx.LoopEndBranch   = this.LoopEndBranch;
    }
}