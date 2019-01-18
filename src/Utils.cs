using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

public static class Utils
{
    /// <summary>
    /// Prints line to console in foreground color Blue.
    /// </summary>
    /// <param name="value"></param>
    public static void logBlue(string value)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(value);
        Console.ForegroundColor = ConsoleColor.Green;
    }

    public static void logRed(string value)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(value);
        Console.ForegroundColor = ConsoleColor.Green;
    }

    /// <summary>
    /// Helper to print value to console with WriteLine
    /// </summary>
    /// <param name="value"></param>
    public static void cLog(string value)
    {
        Console.WriteLine(value);
    }

}
