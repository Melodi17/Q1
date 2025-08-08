namespace Q1.Compiler;

public static class CompilerExtensions
{
    public static string TrimStart(this string source, string search)
    {
        if (source.StartsWith(search))
            return source[search.Length..];

        return source;
    }
}