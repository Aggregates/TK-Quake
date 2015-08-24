using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Infrastructure.Math;

namespace TKQuake.Engine.Infrastructure.Abstract
{
    /// <summary>
    /// Defines the core mechanics for any game object used in an OpenTK game
    /// </summary>
    public class RenderComponent : IComponent
    {
        private readonly RenderableEntity _entity;

        public RenderComponent(RenderableEntity entity) {
            _entity = entity;
        }

        public void Update(double elapsedTime) {

        }

        public void Startup() {
            //load what we need to render the entity
        }

        public void Shutdown() {

        }
    }
}
