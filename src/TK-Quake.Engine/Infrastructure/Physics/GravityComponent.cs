using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Infrastructure.Components;
using TKQuake.Engine.Infrastructure.Entities;
using TKQuake.Engine.Infrastructure.Math;

namespace TKQuake.Engine.Infrastructure.Physics
{
    /// <summary>
    /// Adds gravitational force to an entity
    /// </summary>
    public class GravityComponent : IComponent
    {

        private Entity _entity;

        /// <summary>
        /// The accelleration force
        /// </summary>
        public float Force { get; set; }

        /// <summary>
        /// The amount of velocity acting on the entity the component is attached to
        /// </summary>
        public float Velocity { get; set; }

        /// <summary>
        /// Constructor for the Gravity Component. Sets the inital force and velocity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="force"></param>
        public GravityComponent(Entity entity, float force = 9.8f)
        {
            this._entity = entity;
            this.Force = force;
            this.Velocity = 0;
        }

        public void Shutdown() { }
        public void Startup() { }

        /// <summary>
        /// Update's the velocity acting on the entity and actuates this gravitational force
        /// </summary>
        /// <param name="elapsedTime"></param>
        public void Update(double elapsedTime)
        {
            Velocity += (float)(Force / 10 * elapsedTime / 2);

            var direction = new Vector3(0f, Velocity, 0f);
            _entity.Position -= direction;
        }
    }
}
