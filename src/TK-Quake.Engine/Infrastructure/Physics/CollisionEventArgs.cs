using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Infrastructure.Entities;

namespace TKQuake.Engine.Infrastructure.Physics
{
    /// <summary>
    /// Defines the arguments send when firing a Collision Event
    /// </summary>
    public class CollisionEventArgs : EventArgs
    {
        /// <summary>
        /// The entity who fired the event
        /// </summary>
        public Entity Sender { get; }

        /// <summary>
        /// The Entity the sender collided with
        /// </summary>
        public Entity Collider { get; }

        // It might be worth having a direction vetor

        /// <summary>
        /// Constructor for the Collision Event Arguments. Sets the argument variables
        /// </summary>
        /// <param name="sender">The entity who fired the event</param>
        /// <param name="collider">The Entity the sender collided with</param>
        public CollisionEventArgs(Entity sender, Entity collider)
        {
            this.Sender = sender;
            this.Collider = collider;
        }
    }
}
