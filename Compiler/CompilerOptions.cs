namespace Q1.Compiler;

using CommandLine;

public class CompilerOptions
{
    [Option('i', "input", Required = true, HelpText = "Input file to compile.")]
    public string InputFile { get; set; }

    [Option('o', "output", Required = true, HelpText = "Output file to write the compiled code.")]
    public string OutputFile { get; set; }

    [Option('c', "comment-compilation", Required = false, HelpText = "Comments to preserve in the compiled result")]
    public CommentCompilationMode CommentCompilationMode { get; set; } = CommentCompilationMode.UserDefined;
    
    [Option('v', "verbose", Required = false, HelpText = "Enable verbose output during compilation.")]
    public bool Verbose { get; set; } = false;
}