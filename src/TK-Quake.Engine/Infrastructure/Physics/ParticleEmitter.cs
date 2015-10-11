using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Isg.Range;
using OpenTK;
using TKQuake.Engine.Infrastructure.Entities;

namespace TKQuake.Engine.Infrastructure.Physics
{
    public class ParticleEmitter<T> : Entity where T : Particle, new()
    {

        public Range<float> DirectionX { get; set; }
        public Range<float> DirectionY { get; set; }
        public Range<float> DirectionZ { get; set; }
        public Range<float> TimeToLive { get; set; }
        public Range<float> Velocity { get; set; }

        private Random rand = new Random();

        private float RandRange(Range<float> range)
        {
            // Get random value between min and max
            
            if (range.Lower > range.Upper)
                return(float)(range.Upper + rand.NextDouble() * (range.Lower - range.Upper));
            else
                return (float)(range.Lower + rand.NextDouble() * (range.Upper - range.Lower));
        }

        public void Emit()
        {
            // Get random value between TimeToLive min and max
            float ttl = RandRange(TimeToLive);

            // Geneate a random vector for velocity
            Vector3 dir = new Vector3
            {
                X = RandRange(DirectionX),
                Y = RandRange(DirectionY),
                Z = RandRange(DirectionZ)
            };

            float speed = RandRange(Velocity);

            T particle = (T)Activator.CreateInstance(typeof(T));

            // Set the data
            particle.TimeToLive = ttl;
            particle.Direction = dir.Normalized();
            particle.Speed = speed;
            particle.Position = this.Position;
            particle.Scale = 0.1f;

            particle.Destroy += Particle_Destroy;

            Children.Add(particle);
        }

        private void Particle_Destroy(object sender, EventArgs e)
        {
            RemoveEntity((IEntity)sender);
        }

        public override void Update(double elapsedTime)
        {
            Emit();
            base.Update(elapsedTime);
        }
    }
}
