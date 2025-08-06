namespace Q1.Compiler;

using CommandLine;

public class Options
{
    [Option('i', "input", Required = true, HelpText = "Input file to compile.")]
    public string InputFile { get; set; }

    [Option('o', "output", Required = true, HelpText = "Output file to write the compiled code.")]
    public string OutputFile { get; set; }
}