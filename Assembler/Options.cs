namespace Q1.Assembler;

using CommandLine;

public class AssemblerOptions
{
    [Option('i', "input", Required = true, HelpText = "Input file to assemble.")]
    public string InputFile { get; set; }

    [Option('o', "output", Required = true, HelpText = "Output file to write the assembled code.")]
    public string OutputFile { get; set; }
}