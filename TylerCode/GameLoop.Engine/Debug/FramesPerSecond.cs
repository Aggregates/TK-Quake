using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLoop.Engine.Debug
{
    public class FramesPerSecond
    {
        private static int _numberOfFrames = 0;
        private static double _timePassed = 0;

        public static double CurrentFPS { get; set; }
        
        public static void Process(double timeElapsed)
        {
            _numberOfFrames++;
            _timePassed += timeElapsed;

            if (_timePassed > 1)
            {
                CurrentFPS = ((double) _numberOfFrames) / _timePassed;
                _timePassed = 0;
                _numberOfFrames = 0;
            }
        }

    }
}
