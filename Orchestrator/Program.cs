namespace Orchestrator;

using CommandLine;
using Q1.Assembler;
using Q1.Compiler;
using Q1.Emulator;

class Program
{
    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args).WithParsed(Main);
    }
    
    private static void Main(Options options)
    {
        string compiledFile = Path.ChangeExtension(options.InputFile, "asm");
        string assembledFile = Path.ChangeExtension(options.InputFile, "q1");
        
        Q1.Compiler.Program.Main(new CompilerOptions
        {
            CommentCompilationMode = options.CommentCompilationMode,
            InputFile = options.InputFile,
            OutputFile = compiledFile
        });
        
        Q1.Assembler.Program.Main(new AssemblerOptions
        {
            InputFile = compiledFile,
            OutputFile = assembledFile
        });
        
        Q1.Emulator.Program.Main(new EmulatorOptions
        {
            InputFile = assembledFile,
        });
    }
}

class Options
{
    [Option('i', "input", Required = true, HelpText = "Input file to process.")]
    public string InputFile { get; set; }
    
    [Option('c', "comment-compilation", Required = false, HelpText = "Comments to preserve in the compiled result")]
    public CommentCompilationMode CommentCompilationMode { get; set; } = CommentCompilationMode.UserDefined;
}