using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Infrastructure.Components;
using TKQuake.Engine.Infrastructure.Entities;

namespace TKQuake.Engine.Infrastructure.Physics
{
    public abstract class CollisionComponent : IComponent
    {
        private Entity _entity;

        public CollisionComponent(Entity entity)
        {
            this._entity = entity;
        }

        public virtual void Shutdown() { }
        public virtual void Startup() { }
        public virtual void Update(double elapsedTime) { }
    }
}
