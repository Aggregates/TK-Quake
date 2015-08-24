using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Infrastructure.Abstract;

namespace TKQuake.Engine.Infrastructure.GameScreen
{
    public class ScreenManager : ResourceManager<GameScreen>
    {
        public GameScreen ActiveScreen { get; private set; }

        public ScreenManager() : base()
        {
            this.ActiveScreen = null;
        }

        public override void Add(string screenName, GameScreen screen)
        {
            base.Add(screenName, screen);

            // Update ActiveScreen if it is the first to be registered
            if (ActiveScreen == null)
                ChangeScreen(screenName);
        }

        public override GameScreen Get(string screenName)
        {
            return base.Get(screenName);
        }

        public override void Remove(string screenName)
        {
            if (Registered(screenName) && Database[screenName] == ActiveScreen)
                throw new Exception(string.Format("Screen '{0}' is currently the active screen. Unable to remove", screenName));
            else
                base.Remove(screenName);
        }

        public override bool Registered(string screenName)
        {
            return base.Registered(screenName);
        }

        public void ChangeScreen(string screenName)
        {
            if (Registered(screenName))
                ActiveScreen = Database[screenName];
            else
                throw new Exception(string.Format("Screen '{0}' has not been registered with the Screen Manager", screenName));
        }

        public void Update(double elapsedTime)
        {
            if (ActiveScreen != null)
                ActiveScreen.Update(elapsedTime);
        }

        public void Render()
        {
            if (ActiveScreen != null)
                ActiveScreen.Render();
        }

    }
}
