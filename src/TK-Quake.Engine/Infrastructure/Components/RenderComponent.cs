using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Core;
using TKQuake.Engine.Infrastructure.Math;
using TKQuake.Engine.Infrastructure.Entities;

namespace TKQuake.Engine.Infrastructure.Components
{
    /// <summary>
    /// Renders a given entity
    /// </summary>
    public class RenderComponent : IComponent
    {
        private readonly RenderableEntity _entity;
        private readonly Renderer _renderer = Renderer.Singleton();

        public RenderComponent(RenderableEntity entity) {
            _entity = entity;
        }

        public void Update(double elapsedTime)
        {
            _renderer.DrawEntity(_entity);
        }

        public void Startup() {
            //load what we need to render the entity
        }

        public void Shutdown() {

        }
    }
}
