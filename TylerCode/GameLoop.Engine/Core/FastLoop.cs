using GameLoop.Engine.InterOp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameLoop.Engine.Core
{
    /// <summary>
    /// Forces the main game loop to be called each frame. We will pass the main game
    /// loop function to this class when we run this method
    /// </summary>
    public class FastLoop
    {

        /// <summary>
        /// The funcion used by the FastLoop class
        /// </summary>
        public delegate void LoopCallback(double elapsedTime);

        private LoopCallback _callback;
        private PreciseTimer _timer = new PreciseTimer();

        /// <summary>
        /// Constructor for the FastLoop. Consumes the function to loop
        /// </summary>
        /// <param name="callback">The function to loop</param>
        public FastLoop(LoopCallback callback) 
        {
            /*
             * When the application is not handling events, it silently calls the Application.Idle event
             * which is when we need to execute the main game loop to update the game logic
             */

            this._callback = callback;
            Application.Idle += OnApplicationEnterIdle;
        }

        private void OnApplicationEnterIdle(object sender, EventArgs e)
        {
            // Loop while application is idle
            while (ApplicationIsIdle())
            {
                // Run the callback method
                _callback(_timer.GetElapsedTime());
            }
        }

        private bool ApplicationIsIdle()
        {
            // Check the Application event queue to see if there are unhandled messages. If there are,
            // we need to stop looping and let the application handle them
            CMessage.Message msg = new CMessage.Message();
            return !CMessage.PeekMessage(out msg);
        }

    }
}
