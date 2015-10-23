using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES10;
using TKQuake.Engine.Core;
using TKQuake.Engine.Infrastructure.Entities;
using TKQuake.Engine.Infrastructure.Math;

namespace TKQuake.Engine.Infrastructure.Physics
{
    public class Particle : RenderableEntity
    {
        /// <summary>
        /// The current particle velocity
        /// </summary>
        public Vector3 Direction { get; set; }

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

        /// <summary>
        /// The velocity of the particle
        /// </summary>
        public float Speed { get; set; }

        public Mesh Mesh { get; set; }
        public Camera Camera { get; set; }

        public Particle() : this(0) { }

        public Particle(float timetoLive)
        {
            this.Age = 0;
            this.TimeToLive = timetoLive;
            this.Mesh = ToMesh();
        }

        public override void Update(double elapsedTime)
        {
            Age += (float)elapsedTime;

            // Destroy if dead
            if (Age >= TimeToLive)
                DestroyEntity();

            // Move the particle
            this.Position += Direction * Speed * (float)elapsedTime;

            // Align to billboard if we have a reference to the camera
            if (Camera != null)
            {
                Vector3 normal = -Camera.ViewDirection;
                float dot = Vector3.Dot(-Vector3.UnitZ, normal);
                float rotationAngle = (float)System.Math.Acos(dot);

                this.Rotation = new Vector3(0, rotationAngle, 0);
            }

            base.Update(elapsedTime);
        }

        public Mesh ToMesh()
        {
            var mesh = new Mesh();

            var test = 1;

            var bottomLeft = new Vector3
            {
                X = this.Position.X - test,
                Y = this.Position.Y - test
            };

            var bottomRight = new Vector3
            {
                X = this.Position.X + test,
                Y = this.Position.Y - test
            };

            var topLeft= new Vector3
            {
                X = this.Position.X - test,
                Y = this.Position.Y + test
            };

            var topRight = new Vector3
            {
                X = this.Position.X + test,
                Y = this.Position.Y + test
            };

            mesh.Vertices = new Vertex[]
            {
                new Vertex(bottomLeft, Vector3.UnitZ, new Vector2(0,0), Vector2.Zero), 
                new Vertex(topLeft, Vector3.UnitZ, new Vector2(0,1), Vector2.Zero),
                new Vertex(topRight, Vector3.UnitZ, new Vector2(1,1), Vector2.Zero),
                new Vertex(bottomRight, Vector3.UnitZ, new Vector2(1,0), Vector2.Zero),
            };

            mesh.Indices = new int[]
            {
                0,1,2,
                3,0,2
            };
            return mesh;
        }

    }
}
