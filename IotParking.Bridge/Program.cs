public class Program
{
    static void Main()
    {
        WebPollingService pollingService = new();
        pollingService.Start();
        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();

        pollingService.Stop();
    }
}
