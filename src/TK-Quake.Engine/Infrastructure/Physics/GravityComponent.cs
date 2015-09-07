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
        private float _velocity;
        public bool Active { get; set; }

        public GravityComponent(Entity entity, float force = 9.8f)
        {
            this._entity = entity;
            this.Force = force;
            this._velocity = 0;
            this.Active = true;
        }

        public void Shutdown() { }
        public void Startup() { }

        public void Update(double elapsedTime)
        {
            if (Active)
            {
                _velocity += (float)(Force / 10 * elapsedTime);

                var direction = new Vector3(0f, _velocity, 0f);
                _entity.Position -= direction;

                // The following is test code until it can be moved
                // into a collision detection routine.
                // Stop at y=0
                if (_entity.Position.Y <= 0)
                {
                    _entity.Position = new Vector3(_entity.Position.X, 0, _entity.Position.Z);
                    _velocity = 0;
                    Active = false;
                }

            }
        }
    }
}
