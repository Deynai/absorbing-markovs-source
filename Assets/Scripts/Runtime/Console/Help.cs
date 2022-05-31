using UnityEngine;
using Deynai.Markov;

public class Help
{
    private static ConsoleService _console => ObjectContainer.ConsoleService;

    public static void Notify(object caller, string message)
    {
        Debug.Log($"{message}");

        if (_console != null)
            _console.Warning(message);
    }

    public static void Clear()
    {
        _console.ClearConsole();
    }
}
