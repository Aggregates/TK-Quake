using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GameLoop.Engine.InterOp
{
    /// <summary>
    /// User C Interops to calculate the time between frame renders in the game
    /// </summary>
    public class PreciseTimer
    {

        private long _ticksPerSecond = 0;
        private long _previousTime = 0;

        public PreciseTimer()
        {
            QueryPerformanceFrequency(ref _ticksPerSecond); // Initialise ticks per second
            GetElapsedTime(); // Set previous time to now
        }

        public double GetElapsedTime()
        {
            // Get the current time
            long currentTime = 0;
            QueryPerformanceCounter(ref currentTime);
            
            // Calculate the elapsed time
            double elapsedTime = (double)(currentTime - _previousTime) / (double)_ticksPerSecond;

            // Update the previous time and return
            _previousTime = currentTime;
            return elapsedTime;
        }

        /// <summary>
        /// Retrieves the frequency of a high-resolution performance counter. Most hardware have a high-resolution
        /// timer which this function uses to find when the timer increments
        /// </summary>
        /// <param name="PerformanceFrequency"></param>
        /// <returns></returns>
        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32")]
        public static extern bool QueryPerformanceFrequency(ref long PerformanceFrequency);

        /// <summary>
        /// Retrieves the current value of the high-resolution performance counter Used to time how long te last frame
        /// took to render
        /// </summary>
        /// <param name="PerformanceCount"></param>
        /// <returns></returns>
        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32")]
        public static extern bool QueryPerformanceCounter(ref long PerformanceCount);
    }
}
