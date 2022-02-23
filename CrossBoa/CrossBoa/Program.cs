using System;

namespace CrossBoa
{
    public static class Program
    {
        public static Random RNG = new Random();

        [STAThread]
        static void Main()
        {
            using (var game = new Game1())
                game.Run();
        }
    }
}
