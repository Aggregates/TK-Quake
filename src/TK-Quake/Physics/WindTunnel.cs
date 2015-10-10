using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using TKQuake.Engine.Infrastructure.Entities;
using TKQuake.Engine.Infrastructure.Physics;

namespace TKQuake.Physics
{
    public class WindTunnel : Entity
    {
        private BoundingBoxEntity _boundingBox;

        public Vector3 Direction { get; set; }
        public float Force { get; set; }

        public WindTunnel(Vector3 top, Vector3 bottom)
        {
            _boundingBox = new BoundingBoxEntity(this, top, bottom, false);
            _boundingBox.Collided += BoundingBox_Collided;
        }

        private void BoundingBox_Collided(object sender, CollisionEventArgs e)
        {
            
            // Get the other object
            var other = (e.Sender == this) ? e.Collider : e.Sender;

            // Retrieve the motion blur from depth buffer
            GL.Accum(AccumOp.Return, 1f);
            GL.Accum(AccumOp.Mult, 2f);

            // Move the other object
            Vector3 movement = Direction.Normalized()*Force;
            other.Position += movement;

            // Store the depth buffer for motion blur
            GL.Accum(AccumOp.Accum, 1f);

        }
    }
}
