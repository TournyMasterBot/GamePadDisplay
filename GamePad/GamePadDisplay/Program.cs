using GamePadGameMachine;
using System;
using System.Threading.Tasks;

namespace GamePadDisplay
{
#if WINDOWS || LINUX
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using(var game = new GamePadGame())
            {
                game.Run();
            }
        }
    }
#endif
}
