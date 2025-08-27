namespace Q1.Compiler;

using System.Text;
using Antlr4.Runtime;
using CommandLine;
using visitors;
using Parser = CommandLine.Parser;

public class Program
{
    static void Main(string[] args)
    {
        Parser commandLineParser = new(with =>
        {
            with.CaseInsensitiveEnumValues = true;
        });

        commandLineParser
            .ParseArguments<CompilerOptions>(args)
            .WithParsed(Program.Main);
    }

    public static void Main(CompilerOptions options)
    {
        options.InputFile = options.InputFile;

        try
        {
            AntlrFileStream inputStream = new(options.InputFile);
            CGrammarLexer lexer = new(inputStream);
            lexer.RemoveErrorListeners();

            ITokenStream tokenStream = new BufferedTokenStream(lexer);
            CGrammarParser parser = new(tokenStream);
            parser.RemoveErrorListeners();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding encoding = Encoding.GetEncoding(437);
            CCompilerVisitor compiler = new(options.CommentCompilationMode, encoding);
            IEnumerable<string> result = compiler.Compile(parser.program());

            File.WriteAllLines(options.OutputFile, result);

            Console.WriteLine("Compilation successful.");
        }
        catch (CompilerException ex)
        {
            Console.WriteLine($"{ex.Message}");
        }
    }
}