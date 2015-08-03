using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Infrastructure.GameScreen;

namespace TKQuake.ScreenStates
{
    public class SplashScreen : GameScreen
    {
        private const double DISPLAY_TIME = 3.0; // In Seconds
        private double _timeRemaining;

        public static new string StateNameKey = "SplashScreen";
        
        public SplashScreen(ScreenManager manager)
        {
            this._screenManager = manager;
            this._timeRemaining = DISPLAY_TIME;
        }
        
        public override void Update(double elapsedTime)
        {
            // Subtract from the time remaining
            _timeRemaining -= elapsedTime;
            
            if (_timeRemaining <= 0)
            {
                _timeRemaining = DISPLAY_TIME;
                _screenManager.ChangeScreen(TitleScreen.StateNameKey);
            }
        }

        public override void Render()
        {
            GL.ClearColor(Color.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Finish();
        }
       
    }
}
