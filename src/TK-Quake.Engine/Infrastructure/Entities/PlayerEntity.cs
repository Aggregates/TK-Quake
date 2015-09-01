using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Infrastructure.Math;
using TKQuake.Engine.Infrastructure.Texture;
using static System.Math;

namespace TKQuake.Engine.Infrastructure.Entities
{
    /// <summary>
    /// A living entity that interacts with the game world
    /// </summary>
    public abstract class PlayerEntity : Entity, ILivableEntity
    {
        public double Health { get; set;}
        public bool Alive { get; protected set; }
        public Sprite2 Sprite { get; set; }

        // ILivableEntity properties
        public float MoveSpeed { get; set; }
        public float RotationSpeed { get; set; }

        //todo: move into entity
        public new Vector3 ViewDirection =>
            Vector3.Normalize(new Vector3((float) Sin(Rotation.Y), (float) Sin(Rotation.X), (float) -Cos(Rotation.Y)));

        //todo: move into entity
        public new Vector3 OrthogonalDirection => Vector3.Cross(ViewDirection, Vector3.UnitY);

        /// <summary>
        /// Increments the current position by each component in the vector
        /// </summary>
        /// <param name="amount">The amount in 3D space to move by</param>
        public virtual void Move(Vector3 amount)
        {
            Position = Vector3.Add(Position, amount * MoveSpeed);
        }

        public virtual void Rotate(Vector3 rotation)
        {
            this.Rotation += (rotation * RotationSpeed);
        }

        public virtual void Rotate(float dx, float dy, float dz)
        {
            this.Rotate(new Vector3(dx, dy, dz));
        }

        /// <summary>
        /// Moves the entity by incrementing each of its vector coordinates by a small change
        /// </summary>
        /// <param name="dx">The change in the X-Axis direction</param>
        /// <param name="dy">The change in the Y-Axis direction</param>
        /// <param name="dz">The change in the Z-Axis direction</param>
        public virtual void Move(float dx, float dy, float dz)
        {
            Move(new Vector3(dx, dy, dz));
        }

        /// <summary>
        /// Spanws the player in the world with initial health
        /// </summary>
        /// <param name="health">The initial health of the Player</param>
        public virtual void Spawn(double health = 100)
        {
            this.Health = health;
            Alive = true;
        }

        /// <summary>
        /// Kills the object and removes it from the world
        /// </summary>
        public virtual void Die()
        {
            Alive = false;
        }

        /// <summary>
        /// Inflicts damage on the player and kills them if their health falls below zero
        /// </summary>
        /// <param name="damage">The amount of damage to inflict</param>
        public virtual void Hit(double damage)
        {
            Health -= damage;
            if (Health <= 0)
                Die();
        }

        /// <summary>
        /// Renders the underlying sprite for the Player to the OpenGL window
        /// </summary>
        public virtual void Render()
        {
            Sprite.Render();
        }

    }
}
