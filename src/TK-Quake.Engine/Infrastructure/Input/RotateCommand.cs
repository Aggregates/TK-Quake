using TKQuake.Engine.Infrastructure.Entities;
using TKQuake.Engine.Infrastructure.Math;

namespace TKQuake.Engine.Infrastructure.Input
{
    public class RotateCommand : ICommand
    {
        private Vector _rotation;
        private double _moveSpeed;

        public RotateCommand(Vector rotation, double moveSpeed)
        {
            _rotation = rotation;
            _moveSpeed = moveSpeed;
        }

        public void Execute(IEntity entity)
        {
            entity.Rotation += (_rotation * _moveSpeed);
        }
    }
}
