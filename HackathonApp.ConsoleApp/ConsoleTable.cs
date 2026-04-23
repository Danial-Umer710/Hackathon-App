using System.Reflection;

public static class ConsoleTable
{
    public static void Print<T>(IEnumerable<T> items)
    {
        var list = items.ToList();
        if (!list.Any())
        {
            Console.WriteLine("(no results)\n");
            return;
        }

        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        int[] colWidths = new int[props.Length];
        for (int i = 0; i < props.Length; i++)
        {
            int max = props[i].Name.Length;
            foreach (var item in list)
            {
                string val = props[i].GetValue(item)?.ToString() ?? "";
                if (val.Length > max) max = val.Length;
            }
            colWidths[i] = max + 3;
        }

        PrintSeparator(colWidths);

        for (int i = 0; i < props.Length; i++)
            Console.Write(props[i].Name.PadRight(colWidths[i]));
        Console.WriteLine();

        PrintSeparator(colWidths);

        foreach (var item in list)
        {
            for (int i = 0; i < props.Length; i++)
            {
                string val = props[i].GetValue(item)?.ToString() ?? "";
                Console.Write(val.PadRight(colWidths[i]));
            }
            Console.WriteLine();
        }

        PrintSeparator(colWidths);
        Console.WriteLine();
    }

    private static void PrintSeparator(int[] widths)
    {
        foreach (var w in widths)
            Console.Write(new string('-', w));
        Console.WriteLine();
    }
}
