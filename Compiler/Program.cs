namespace Q1.Compiler;

using Antlr4.Runtime;
using CommandLine;
using Parser = CommandLine.Parser;

class Program
{
    static void Main(string[] args)
    {
        Parser commandLineParser = new(with =>
        {
            with.CaseInsensitiveEnumValues = true;
        });
        
        commandLineParser.ParseArguments<CompilerOptions>(args)
            .WithParsed(Program.Main);
    }

    static void Main(CompilerOptions options)
    {
        Environment.CurrentDirectory = Path.GetDirectoryName(options.InputFile) ?? Environment.CurrentDirectory;
        options.InputFile            = Path.GetFileName(options.InputFile);

        AntlrFileStream inputStream = new(options.InputFile);
        CGrammarLexer lexer = new(inputStream);
        lexer.RemoveErrorListeners();
        
        ITokenStream tokenStream = new BufferedTokenStream(lexer);
        CGrammarParser parser = new(tokenStream);
        parser.RemoveErrorListeners();
        
        CCompilerVisitor compiler = new(options.CommentCompilationMode);
        IEnumerable<string> result = compiler.Compile(parser.program());
        
        File.WriteAllLines(options.OutputFile, result);
    }
}