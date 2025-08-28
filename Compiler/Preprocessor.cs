namespace Q1.Compiler;

using System.Text.RegularExpressions;

public static class Preprocessor
{
    private static readonly Dictionary<string, string> Defines = new();

    public static string Process(string filePath)
    {
        List<string> lines = [];
        ProcessFile(filePath, lines, []);
        return string.Join(Environment.NewLine, lines);
    }

    private static void ProcessFile(string filePath, List<string> output, HashSet<string> includeGuard)
    {
        if (includeGuard.Contains(Path.GetFullPath(filePath)))
            return; // prevent recursive includes
        
        includeGuard.Add(Path.GetFullPath(filePath));

        foreach (string rawLine in File.ReadAllLines(filePath))
        {
            string line = rawLine.Trim();

            if (line.StartsWith("#include"))
            {
                Match match = Regex.Match(line, "#include\\s+\"([^\"]+)\"");
                if (match.Success)
                {
                    string includePath = Path.Combine(Path.GetDirectoryName(filePath)!, match.Groups[1].Value);
                    ProcessFile(includePath, output, includeGuard);
                }
                continue;
            }
            else if (line.StartsWith("#define"))
            {
                Match match = Regex.Match(line, "#define\\s+(\\w+)\\s+(.*)");
                if (match.Success)
                    Defines[match.Groups[1].Value] = match.Groups[2].Value;
                continue;
            }
            else if (line.StartsWith("#load"))
            {
                output.Add(line.Replace("#load", "#.include"));
                continue;
            }

            // expand macros
            foreach (KeyValuePair<string, string> kvp in Defines.OrderByDescending(k => k.Key.Length))
                line = line.Replace(kvp.Key, kvp.Value);

            output.Add(line);
        }
    }
}