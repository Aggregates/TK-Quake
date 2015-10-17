using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Isg.Range;
using TKQuake.Engine.Infrastructure.Entities;
using TKQuake.Engine.Infrastructure.Physics;

namespace TKQuake.Cookbook.Entities
{
    public class FireParticleSystem : Entity
    {
        public FireParticleSystem()
        {
            // Fire Emitter
            ParticleEmitter<FireParticle> fireEmitter = new ParticleEmitter<FireParticle>
            {
                DirectionX = new Range<float> { Lower = -0.1f, Upper = 0.1f },
                DirectionY = new Range<float> { Lower = 1, Upper = 1 },
                DirectionZ = new Range<float> { Lower = -0.1f, Upper = 0.1f },
                TimeToLive = new Range<float> { Lower = 1, Upper = 10 },
                Velocity = new Range<float> { Lower = 1, Upper = 1 },
                ParticleScale = new Range<float> { Lower = 0.1f, Upper = 0.5f },
                SpawnVariance = new Range<float> { Lower = 1, Upper = 1 },
            };
            Children.Add(fireEmitter);

            // Fire Emitter
            ParticleEmitter<SmokeParticle> smokeEmitter = new ParticleEmitter<SmokeParticle>
            {
                DirectionX = new Range<float> { Lower = -0.5f, Upper = 0.5f },
                DirectionY = new Range<float> { Lower = 1, Upper = 1 },
                DirectionZ = new Range<float> { Lower = -0.5f, Upper = 0.5f },
                TimeToLive = new Range<float> { Lower = 1, Upper = 10 },
                Velocity = new Range<float> { Lower = 1, Upper = 2 },
                ParticleScale = new Range<float> { Lower = 0.1f, Upper = 0.5f },
                SpawnVariance = new Range<float> { Lower = 3, Upper = 3 },
            };
            Children.Add(smokeEmitter);
        }
    }
}
