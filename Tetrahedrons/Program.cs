using OpenTK;

namespace Tetrahedrons
{
    internal class Program : GameWindow
    {
        public static void Main(string[] args)
        {
            using(var p = new TetrahedronWindow())
                p.Run();
        }
    }
}