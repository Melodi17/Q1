namespace Q1.Compiler;

using Antlr4.Runtime;
using CommandLine;
using Parser = CommandLine.Parser;

class Program
{
    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args).WithParsed(Program.Main);
    }

    static void Main(Options options)
    {
        Environment.CurrentDirectory = Path.GetDirectoryName(options.InputFile) ?? Environment.CurrentDirectory;
        options.InputFile            = Path.GetFileName(options.InputFile);

        AntlrFileStream inputStream = new(options.InputFile);
        CGrammarLexer lexer = new(inputStream);
        lexer.RemoveErrorListeners();
        
        CommonTokenStream tokenStream = new(lexer);
        CGrammarParser parser = new(tokenStream);
        parser.RemoveErrorListeners();
        
        CCompilerVisitor compiler = new();
        var result = compiler.Compile(parser.program());
        File.WriteAllLines(options.OutputFile, result);
    }
}