namespace Q1.Compiler.visitors;

public partial class CCompilerVisitor
{
    private List<(CGrammarParser.FunctionDefinitionContext context,
        FunctionReference function, FunctionOverload overload,
        List<string> paramNames, List<CType> paramTypes)> _functionBodies = new();
    
    private VariableReference FindVariable(string varName)
    {
        foreach (Dictionary<string, VariableReference> variables in this._variableStack)
        {
            if (variables.TryGetValue(varName, out VariableReference? reference))
                return reference;
        }

        throw new CompilerException($"Variable {varName} could not be found in current scope.");
    }
    private int Alloc(int size)
    {
        int loc = CCompilerVisitor._memPointer;
        CCompilerVisitor._memPointer += size;

        return loc;
    }
    private FunctionReference CreateOrGetFunctionReference(string name, CType type)
    {
        FunctionReference? function = this._methods.FirstOrDefault(x => x.Name == name);
        if (function != null)
            return function;

        function = new FunctionReference(name, type);
        this._methods.Add(function);

        return function;
    }
    private FunctionReference GetFunctionReference(string name)
    {
        FunctionReference? function = this._methods.FirstOrDefault(x => x.Name == name);
        if (function != null)
            return function;

        throw new CompilerException($"Function '{name}' has not been defined or prototyped.");
    }
    
    public override string? VisitFunctionPrototype(CGrammarParser.FunctionPrototypeContext context)
    {
        string functionName = context.name.Text;
        CType type = this.VisitCType(context._type);

        List<CType> parameterTypes = context._types.Select(this.VisitCType).ToList();
        List<string> parameterNames = context._params.Select(x => x.Text).ToList();

        FunctionReference function = this.CreateOrGetFunctionReference(functionName, type);
        FunctionOverload overload = function.CreateOverloadWithoutBody(parameterNames, parameterTypes);

        function.Overloads.Add(overload);

        return null;
    }
    public override string? VisitFunctionDefinition(CGrammarParser.FunctionDefinitionContext context)
    {
        string functionName = context.name.Text;
        CType type = this.VisitCType(context.rtype);

        List<CType> paramTypes = context._types.Select(this.VisitCType).ToList();
        List<string> paramNames = context._params.Select(x => x.Text).ToList();

        FunctionReference function = this.CreateOrGetFunctionReference(functionName, type);
        FunctionOverload overload = function.CreateOverloadWithBody(paramNames, paramTypes);

        this._functionBodies.Add((context, function, overload, paramNames, paramTypes));

        return null;
    }
    private void VisitFunctionBody(CGrammarParser.FunctionDefinitionContext context, FunctionReference function, FunctionOverload overload, List<string> paramNames, List<CType> paramTypes)
    {
        this.Branch(function.GetBranchName(overload), function.GetBranchFriendlyName(overload));

        if (paramNames.Count > 0)
            this.Instruction("pop V0", "preserve function return address");

        Dictionary<string, VariableReference> scope = this.EnterScope();
        foreach ((string paramName, CType paramType) in paramNames.Zip(paramTypes))
        {
            int addr = this.Alloc(paramType.Size);
            scope[paramName] = new(addr, paramType);
            this.Instruction($"pop [${addr:X4}]", $"param {paramType} {paramName}");
        }

        if (paramNames.Count > 0)
            this.Instruction("push V0", "restore function return address");

        this.VisitChildren(context);
        this.ExitScope();

        this.Instruction("mov 0, V0", "fallback return 0");
        this.Instruction("ret");
        this.Blank();
    }
    public override string? VisitVariableDeclaration(CGrammarParser.VariableDeclarationContext context)
    {
        CType type = this.VisitCType(context.type());
        int addr = this.Alloc(CType.Int.Size);
        string name = context.name.Text;

        Dictionary<string, VariableReference> scope = this._variableStack.Peek();
        if (scope.ContainsKey(name))
            throw new CompilerException($"Variable {name} already exists in current scope.");

        scope[name] = new VariableReference(addr, type);
        string initialValue = "0";

        if (context.value != null)
        {
            initialValue = this.VisitExpression(context.value);
        }

        this.DynamicMove(CType.Int, initialValue, $"[${addr:X4}]", $"int {name} = {initialValue}");

        return null;
    }
}