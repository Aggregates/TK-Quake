using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using TKQuake.Engine.Infrastructure.Entities;

namespace TKQuake.Engine.Infrastructure.Physics
{
    public class Particle : Entity
    {
        /// <summary>
        /// The current particle velocity
        /// </summary>
        public Vector3 Velocity { get; set; }

        /// <summary>
        /// The particle colour
        /// </summary>
        public Color4 Color { get; set; }

        /// <summary>
        /// Age of the particle since it was emitted
        /// </summary>
        public float Age { get; set; }

        /// <summary>
        /// The time for the particle to live from when it was emitted
        /// </summary>
        public float TimeToLive { get; set; }

        public Particle(float timetoLive)
        {
            this.Age = 0;
            this.TimeToLive = timetoLive;
        }

        public override void Update(double elapsedTime)
        {
            Age += (float)elapsedTime;

            // Destroy if dead
            if (Age >= TimeToLive)
                DestroyEntity();

            // Move the particle
            this.Position += Velocity*(float)elapsedTime;

            base.Update(elapsedTime);
        }
    }
}
