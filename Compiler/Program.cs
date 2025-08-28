namespace Q1.Compiler;

using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
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
            ICharStream inputStream = new PreprocessingCharStream(options.InputFile);
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

public class PreprocessingCharStream : ICharStream
{
    private readonly AntlrInputStream inner;

    public PreprocessingCharStream(string filePath)
    {
        string preprocessed = Preprocessor.Process(filePath);
        inner = new AntlrInputStream(preprocessed);
    }

    public int Index => inner.Index;
    public int Size => inner.Size;
    public string SourceName => inner.SourceName;
    public void Consume() => inner.Consume();
    public int LA(int i) => inner.LA(i);
    public int Mark() => inner.Mark();
    public void Release(int marker) => inner.Release(marker);
    public void Seek(int index) => inner.Seek(index);
    public string GetText(Interval interval) => inner.GetText(interval);
}