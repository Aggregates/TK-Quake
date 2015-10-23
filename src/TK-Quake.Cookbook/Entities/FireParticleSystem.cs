using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Isg.Range;
using OpenTK;
using TKQuake.Engine.Core;
using TKQuake.Engine.Infrastructure.Entities;
using TKQuake.Engine.Infrastructure.Physics;

namespace TKQuake.Cookbook.Entities
{
    public class FireParticleSystem : Entity
    {
        private Camera _cam;
        public Camera Camera {
            get { return _cam; }
            set
            {
                _cam = value;
                _fireEmitter.Camera = value;
                _smokeEmitter.Camera = value;
            }
        }

        private Vector3 _pos;

        public new Vector3 Position
        {
            get { return _pos; }
            set
            {
                _pos = value;
                _fireEmitter.Position = value;
                _smokeEmitter.Position = value;
            }
        }

        private ParticleEmitter<FireParticle> _fireEmitter;
        private ParticleEmitter<SmokeParticle> _smokeEmitter;

        public FireParticleSystem()
        {
            // Fire Emitter
            _fireEmitter = new ParticleEmitter<FireParticle>
            {
                DirectionX = new Range<float> { Lower = -0.1f, Upper = 0.1f },
                DirectionY = new Range<float> { Lower = 1, Upper = 1 },
                DirectionZ = new Range<float> { Lower = -0.1f, Upper = 0.1f },
                TimeToLive = new Range<float> { Lower = 1, Upper = 5 },
                Velocity = new Range<float> { Lower = 1, Upper = 1 },
                ParticleScale = new Range<float> { Lower = 0.1f, Upper = 0.3f },
                SpawnVariance = new Range<float> { Lower = 1, Upper = 1 },
            };

            // Smoke Emitter
            _smokeEmitter = new ParticleEmitter<SmokeParticle>
            {
                DirectionX = new Range<float> { Lower = -0.5f, Upper = 0.5f },
                DirectionY = new Range<float> { Lower = 1, Upper = 1 },
                DirectionZ = new Range<float> { Lower = -0.5f, Upper = 0.5f },
                TimeToLive = new Range<float> { Lower = 1, Upper = 5 },
                Velocity = new Range<float> { Lower = 1, Upper = 2 },
                ParticleScale = new Range<float> { Lower = 0.1f, Upper = 0.3f },
                SpawnVariance = new Range<float> { Lower = 2, Upper = 4 },
            };

            Children.Add(_fireEmitter);
            Children.Add(_smokeEmitter);
        }
    }
}
