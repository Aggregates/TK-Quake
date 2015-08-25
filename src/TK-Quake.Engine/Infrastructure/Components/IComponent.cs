using System;

namespace TKQuake.Engine.Infrastructure.Components
{
    public interface IComponent
    {
        void Startup();
        void Shutdown();
        void Update(double elapsedTime);
    }
}
