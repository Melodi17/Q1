namespace Q1.Compiler;

using System.Text.RegularExpressions;

public class Optimizer
{
    private string _instructions;
    private int _currentIndex = 0;
    
    public Optimizer(List<string> instructions)
    {
        this._instructions = string.Join(Environment.NewLine, instructions);
    }
    
    public string Optimize()
    {
        string output = this._instructions;
        output = this.RemoveRedundantFallbackReturns(output);
        
        return output;
    }
    
    private string RemoveRedundantFallbackReturns(string input)
    {
        string pattern = @"(mov .*, V0\n\s*ret\n)\s*mov 0, V0\n\s*ret";
        return Regex.Replace(input, pattern, "$1");
    }
}