using TKQuake.Engine.Infrastructure.Entities;
using TKQuake.Engine.Infrastructure.Math;

namespace TKQuake.Engine.Infrastructure.Input
{
    public class MoveCommand : ICommand
    {
        private Vector _to;
        private double _moveSpeed;

        public MoveCommand(Vector to, double moveSpeed)
        {
            _to = to;
            _moveSpeed = moveSpeed;
        }

        public void Execute(IEntity entity)
        {
            entity.Position += (_to * _moveSpeed);
        }
    }
}
