namespace Q1.Compiler;

using System.Reflection.Metadata;
using System.Text;
using visitors;

public class FunctionReference
{
    // TODO: Implement return type
    public string                 Name;
    public CType                 Type;
    public List<FunctionOverload> Overloads = new();

    public FunctionReference(string name, CType type)
    {
        this.Type = type;
        this.Name = name;
    }

    public string GetBranchName(FunctionOverload overload)
    {
        List<(string paramName, CType paramType)> parameters = overload
            .ParameterNames
            .Zip(overload.ParameterTypes)
            .ToList();

        StringBuilder branchName = new();
        branchName.Append($"_{this.Name}");

        if (parameters.Count > 0)
        {
            branchName.Append($"_{string.Join("__", parameters
                .Select(x => $"{x.paramType}_{x.paramName}"))}");
        }

        return branchName.ToString();
    }

    public string GetBranchFriendlyName(FunctionOverload overload)
    {
        List<(string paramName, CType paramType)> parameters = overload
            .ParameterNames
            .Zip(overload.ParameterTypes)
            .ToList();

        StringBuilder branchName = new();
        branchName.Append($"{this.Type} {this.Name}");
        branchName.Append("(");

        branchName.Append(string.Join(", ", parameters
            .Select(x => $"{x.paramType} {x.paramName}")));

        branchName.Append(")");

        return branchName.ToString();
    }

    public FunctionOverload CreateOverloadWithBody(List<string> parameterNames, List<CType> parameterTypes)
    {
        FunctionOverload? existingOverload = this.Overloads
            .FirstOrDefault(x => x.ParamsMatch(parameterNames, parameterTypes));

        if (existingOverload != null)
        {
            if (existingOverload.BodyImplemented)
                throw new CompilerException($"Cannot define another overload for {this.Name},"
                                            + $" as one with an identical signature and body already exists.");

            existingOverload.BodyImplemented = true;
            return existingOverload;
        }
        else
        {
            FunctionOverload newOverload = new(parameterNames, parameterTypes, true);
            this.Overloads.Add(newOverload);
            return newOverload;
        }
    }

    public FunctionOverload CreateOverloadWithoutBody(List<string> parameterNames, List<CType> parameterTypes)
    {
        FunctionOverload? existingOverload = this.Overloads
            .FirstOrDefault(x => x.ParamsMatch(parameterNames, parameterTypes));

        if (existingOverload != null)
        {
            if (!existingOverload.BodyImplemented)
                throw new CompilerException($"Cannot prototype another overload for {this.Name},"
                                            + $" as one with an identical signature already exists.");
            return existingOverload;
        }
        else
        {
            FunctionOverload newOverload = new(parameterNames, parameterTypes, false);
            this.Overloads.Add(newOverload);
            return newOverload;
        }
    }
    public FunctionOverload GetOverload(int paramsCount)
    {
        FunctionOverload? overload = this.Overloads
            .FirstOrDefault(x => x.ParameterTypes.Count == paramsCount);

        if (overload != null)
            return overload;

        throw new CompilerException($"Overload for {this.Name} with {paramsCount} arguments could not be found.");
    }
}

public class FunctionOverload
{
    public List<CType> ParameterTypes;
    public List<string> ParameterNames;
    public bool         BodyImplemented;

    public FunctionOverload(List<string> parameterNames, List<CType> parameterTypes, bool bodyImplemented)
    {
        this.ParameterTypes  = parameterTypes;
        this.ParameterNames  = parameterNames;
        this.BodyImplemented = bodyImplemented;
    }

    public bool ParamsMatch(List<string> parameterNames, List<CType> parameterTypes)
    {
        return this.ParameterNames.SequenceEqual(parameterNames)
               && this.ParameterTypes.SequenceEqual(parameterTypes);
    }
}