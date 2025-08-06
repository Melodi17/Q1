namespace Q1.Emulator;

public class Options
{
    [CommandLine.Option('i', "input", Required = true, HelpText = "Input file to run.")]
    public string InputFile { get; set; }
}