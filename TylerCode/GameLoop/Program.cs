using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameLoop
{
    static class Program
    {
        //private static FastLoop _loop = new FastLoop(MainGame.GameLoop);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            var game = new Game();
            game.Run();
        }
    }
}
