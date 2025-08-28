namespace Orchestrator;

using CommandLine;
using Q1.Assembler;
using Q1.Compiler;
using Q1.Emulator;

class Program
{
    static void Main(string[] args)
    {
        Parser p = new(with =>
        {
            with.CaseInsensitiveEnumValues = true;
        });

        p.ParseArguments<Options>(args).WithParsed(Main);
    }

    private static void Main(Options options)
    {
        string compiledFile = Path.ChangeExtension(options.InputFile, "asm");
        string assembledFile = Path.ChangeExtension(options.InputFile, "q1");

        if (!options.NoCompile)
            Q1.Compiler.Program.Main(
                new CompilerOptions
                {
                    CommentCompilationMode = options.CommentCompilationMode,
                    InputFile = options.InputFile,
                    OutputFile = compiledFile,
                    Verbose = options.Verbose
                });

        if (!options.NoAssemble)
            Q1.Assembler.Program.Main(
                new AssemblerOptions
                {
                    InputFile = compiledFile,
                    OutputFile = assembledFile
                });

        if (!options.NoEmulate)
            Q1.Emulator.Program.Main(
                new EmulatorOptions
                {
                    InputFile = assembledFile,
                });
    }
}

class Options
{
    [Option('i', "input", Required = true, HelpText = "Input file to process.")]
    public string InputFile { get; set; }

    [Option(
        'c', "comment-compilation", Required = false,
        HelpText = "Comments to preserve in the compiled result")]
    public CommentCompilationMode CommentCompilationMode { get; set; } =
        CommentCompilationMode.UserDefined;

    [Option("no-compile", HelpText = "Skip compilation")]
    public bool NoCompile { get; set; } = false;

    [Option("no-assemble", HelpText = "Skip assembling")]
    public bool NoAssemble { get; set; } = false;

    [Option("no-emulate", HelpText = "Skip emulation")]
    public bool NoEmulate { get; set; } = false;
    
    [Option('v', "verbose", HelpText = "Enable verbose output")]
    public bool Verbose { get; set; } = false;
}