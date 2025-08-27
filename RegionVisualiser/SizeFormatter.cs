namespace RegionVisualiser;

public static class SizeFormatter
{
    private static readonly string[] Units = ["bytes", "KB", "MB", "GB", "TB"];

    public static string HumanReadable(long bytes)
    {
        if (bytes < 1024) return $"{bytes} {SizeFormatter.Units[0]}";

        double value = bytes;
        int unit = 0;

        while (value >= 1024 && unit < SizeFormatter.Units.Length - 1)
        {
            value /= 1024;
            unit++;
        }

        // Show up to 2 decimal places if not whole
        return value % 1 == 0
            ? $"{value:0} {SizeFormatter.Units[unit]}"
            : $"{value:0.##} {SizeFormatter.Units[unit]}";
    }
}