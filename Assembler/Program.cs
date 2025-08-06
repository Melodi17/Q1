namespace Q1.Assembler;

using CommandLine;

public class Program
{
    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args).WithParsed(Program.Main);
    }

    static void Main(Options options)
    {
        Environment.CurrentDirectory = Path.GetDirectoryName(options.InputFile) ?? Environment.CurrentDirectory;
        options.InputFile = Path.GetFileName(options.InputFile);
        
        string[] input = File.ReadAllLines(options.InputFile);

        try
        {
            Assembler assembler = new(input);
            byte[] bin = assembler.Assemble().ToArray();
            if (bin.Length > 8 * 1024)
                throw new AssemblerException("Assembled binary exceeds maximum size of 8KB.");

            File.WriteAllBytes(options.OutputFile, bin);

            Console.WriteLine($"Assembled {options.InputFile} to {options.OutputFile} ({bin.Length} bytes)");
        }
        catch (AssemblerException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
            Environment.Exit(1);
        }
    }
}