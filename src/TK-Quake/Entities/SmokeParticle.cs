using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Infrastructure.Physics;

namespace TKQuake
{
    public class SmokeParticle : Particle
    {
        public SmokeParticle() : this(0) { }

        public SmokeParticle(float timeToLive)
            : base(timeToLive)
        {
            this.TextureId = "SmokeParticle";
            this.Id = "SmokeParticle";
        }
    }
}
