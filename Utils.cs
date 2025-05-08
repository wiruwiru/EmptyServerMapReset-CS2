namespace EmptyServerMapReset;

public class Utils
{
    public static BaseConfigs? Config { get; set; }

    public static void DebugMessage(string message)
    {
        if (Config?.EnableDebug != true) return;
        Console.WriteLine($"================================= [ EmptyServerMapReset ] =================================");
        Console.WriteLine(message);
        Console.WriteLine("=========================================================================================");
    }

}