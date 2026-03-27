namespace script.Utilities;

public static class ConsoleUtilities
{
    /// <summary>
    /// Print with custom colors on console
    /// </summary>
    /// <param name="message"></param>
    /// <param name="color"></param>
    public static void Print(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}
