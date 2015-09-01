using OpenTK;
using TKQuake.Engine.Infrastructure.Entities;
using TKQuake.Engine.Infrastructure.Math;

namespace TKQuake.Engine.Infrastructure.Input
{
    public class RotateCommand : ICommand
    {
        private Vector3 _rotation;
        private float _moveSpeed;

        public RotateCommand(Vector3 rotation, float moveSpeed)
        {
            _rotation = rotation;
            _moveSpeed = moveSpeed;
        }

        public RotateCommand(Vector3 rotation, double moveSpeed) : this(rotation, (float)moveSpeed) { }

        public void Execute(IEntity entity)
        {
            entity.Rotation += (_rotation * _moveSpeed);
        }
    }
}
