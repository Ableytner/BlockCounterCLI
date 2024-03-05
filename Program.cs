namespace BlockCounterCLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CLI cli = new CLI();
            cli.Setup();
            cli.RunLoop();
        }
    }
}
