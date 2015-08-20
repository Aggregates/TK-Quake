using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Infrastructure.Math;
using TKQuake.Engine.Infrastructure.Texture;

namespace TKQuake.Engine.Infrastructure.Abstract
{
    public abstract class PlayerEntity : ILivableEntity, IGameObject
    {
        public double Health { get; set;}
        public Vector Position { get; set; }
        public double MoveSpeed { get; set; }
        public double RotationSpeed { get; set; }
        public Matrix4 Matrix { get; protected set; }
        public bool Alive { get; protected set; }
        public Sprite2 Sprite { get; set; }

        /// <summary>
        /// Increments the current position by each component in the vector
        /// </summary>
        /// <param name="amount">The amount in 3D space to move by</param>
        public virtual void Move(Vector amount)
        {
            this.Position += amount;
        }

        /// <summary>
        /// Moves the entity by incrementing each of its vector coordinates by a small change
        /// </summary>
        /// <param name="dx">The change in the X-Axis direction</param>
        /// <param name="dy">The change in the Y-Axis direction</param>
        /// <param name="dz">The change in the Z-Axis direction</param>
        public virtual void Move(double dx, double dy, double dz)
        {
            Move(new Vector(dx, dy, dz));
        }

        public virtual void Spawn()
        {
            Alive = true;
        }

        public virtual void Die()
        {
            Alive = false;
        }

        public virtual void Hit(double damage)
        {
            Health -= damage;
            if (Health <= 0)
                Die();
        }

        public virtual void Update(double elapsedTime) { }

        public virtual void Render()
        {
            Sprite.Render();
        }
    }
}
