namespace Platformer
{
    internal static class Driver
    {
        public static void Main(string[] args)
        {
            using (var pw = new PlatformWindow())
                pw.Run();
        }
    }
}