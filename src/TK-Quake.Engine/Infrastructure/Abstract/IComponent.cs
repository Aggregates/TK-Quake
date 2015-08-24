using System;

namespace TKQuake.Engine.Infrastructure.Abstract
{
    public interface IComponent
    {
        void Startup();
        void Shutdown();
        void Update(double elapsedTime);
    }
}
