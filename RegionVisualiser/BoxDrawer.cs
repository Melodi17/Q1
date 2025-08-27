namespace RegionVisualiser;

public static class BoxDrawer
{
    public static void DrawBox(string title, string body, int width = 0, string head = "", string tail = "")
    {
        int leftPad = Math.Max(head.Length, tail.Length);
        var lines = body.Split('\n');
        int maxWidth = Math.Max(Math.Max(title.Length + 2, BoxDrawer.GetMaxWidth(lines) + 1), width);

        // Top border with centered title
        string top = "┌" + BoxDrawer.CenterText($" {title} ", maxWidth, '─') + "┐";
        Console.Write(head);
        Console.CursorLeft = leftPad;
        Console.WriteLine(top);

        // Body lines, left aligned
        foreach (var line in lines)
        {
            Console.CursorLeft = leftPad;
            Console.WriteLine("│ " + line.PadRight(maxWidth - 1) + "│");
        }

        // Bottom border
        string bottom = "└" + new string('─', maxWidth) + "┘";
        Console.Write(tail);
        Console.CursorLeft = leftPad;
        Console.WriteLine(bottom);
    }

    private static int GetMaxWidth(string[] lines)
    {
        int width = 0;
        foreach (var l in lines)
            if (l.Length + 1 > width) width = l.Length + 1;
        return width;
    }

    private static string CenterText(string text, int width, char fill)
    {
        int totalPad = width - text.Length;
        if (totalPad <= 0) return text;

        int left = totalPad / 2;
        int right = totalPad - left;

        return new string(fill, left) + text + new string(fill, right);
    }
}