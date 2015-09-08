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
    public class GravityComponent : IComponent
    {

        private Entity _entity;
        public float Force { get; set; }
        public float Velocity;
        public bool Active { get; set; }

        public GravityComponent(Entity entity, float force = 9.8f)
        {
            this._entity = entity;
            this.Force = force;
            this.Velocity = 0;
            this.Active = true;
        }

        public void Shutdown() { }
        public void Startup() { }

        public void Update(double elapsedTime)
        {
            if (Active)
            {
                Velocity += (float)(Force / 10 * elapsedTime / 2);

                var direction = new Vector3(0f, Velocity, 0f);
                _entity.Position -= direction;
            }
        }
    }
}
