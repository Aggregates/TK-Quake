using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Infrastructure.Physics;

namespace TKQuake
{
    public class FireParticle : Particle
    {
        public FireParticle() : this(0) { }

        public FireParticle(float timeToLive)
            : base(timeToLive)
        {
            this.TextureId = "FireParticle";
            this.Id = "FireParticle";
        }
    }
}
