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
        public Matrix4 Matrix { get; private set; }
        public bool Alive { get; private set; }
        public Sprite2 Sprite { get; set; }

        public virtual void Move(Vector position)
        {
            this.Position = position;
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
