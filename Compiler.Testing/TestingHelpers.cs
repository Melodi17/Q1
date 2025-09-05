namespace Compiler.Testing;

using Antlr4.Runtime;
using Q1.Compiler;

public class TestingHelpers
{
    public static string CompileString(string input)
    {
        CompilerOptions options = new() { CommentCompilationMode = CommentCompilationMode.None };
        IEnumerable<string> lines = Program.Compile(options, new AntlrInputStream(input));
        return Program.Optimize(lines);
    }
}