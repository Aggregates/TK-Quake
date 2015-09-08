using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Infrastructure.Entities;

namespace TKQuake.Engine.Infrastructure.Physics
{
    public class CollisionEventArgs : EventArgs
    {
        public Entity Sender { get; set; }
        public Entity Collider { get; set; }

        // It might be worth having a direction vetor

        public CollisionEventArgs(Entity sender, Entity collider)
        {
            this.Sender = sender;
            this.Collider = collider;
        }
    }
}
