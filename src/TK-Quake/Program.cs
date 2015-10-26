using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TKQuake
{
    static class Program
    {
        //private static FastLoop _loop = new FastLoop(MainGame.GameLoop);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            var game = new Game();
            game.Run();
        }
    }
}
