using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Infrastructure.Components;
using TKQuake.Engine.Infrastructure.Entities;

namespace TKQuake.Engine.Infrastructure.Physics
{
    public class CollisionComponent : IComponent
    {
        public bool Solid { get; set; }

        /**
         * In order to check collisions, the world needs to know about all
         * collision volumes and check volumes. Should be done using the Dot
         * Product.
         */
        private Entity _world;

        private Entity _entity;
        //private Octree tree;

        public CollisionComponent(Entity entity, Entity world)
        {
            this._entity = entity;
            this._world = world;
        }

        public void Shutdown() { }

        public void Startup() { }

        public void Update(double elapsedTime)
        {
            throw new NotImplementedException();
        }
    }
}
