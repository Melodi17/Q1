namespace Q1.Assembler;

using CommandLine;

public class Program
{
    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<AssemblerOptions>(args).WithParsed(Program.Main);
    }

    public static void Main(AssemblerOptions assemblerOptions)
    {
        assemblerOptions.InputFile = assemblerOptions.InputFile;
        
        string[] input = File.ReadAllLines(assemblerOptions.InputFile);

        try
        {
            Assembler assembler = new(input);
            byte[] bin = assembler.Assemble().ToArray();
            if (bin.Length > 8 * 1024)
                throw new AssemblerException("Assembled binary exceeds maximum size of 8KB.");

            File.WriteAllBytes(assemblerOptions.OutputFile, bin);

            Console.WriteLine($"Assembled {assemblerOptions.InputFile} to {assemblerOptions.OutputFile} ({bin.Length} bytes)");
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