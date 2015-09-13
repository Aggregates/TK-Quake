using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Infrastructure.Entities;

namespace TKQuake.Engine.Infrastructure.Input
{
    /// <summary>
    /// Moves the entity in the positive y-axis
    /// </summary>
    public class JumpCommand : ICommand
    {

        private Vector3 _direction;
        private float _jumpSpeed;

        public JumpCommand(Vector3 direction, float jumpSpeed)
        {
            this._direction = direction;
            this._jumpSpeed = jumpSpeed;
        }

        public JumpCommand(Vector3 direction, double jumpSpeed)
            : this(direction, (float)jumpSpeed)
        { }

        public void Execute(IEntity entity)
        {
            entity.Position += (_direction * _jumpSpeed);
        }
    }
}
