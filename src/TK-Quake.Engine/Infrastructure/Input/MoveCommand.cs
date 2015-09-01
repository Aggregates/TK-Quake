using OpenTK;
using TKQuake.Engine.Infrastructure.Entities;
using TKQuake.Engine.Infrastructure.Math;

namespace TKQuake.Engine.Infrastructure.Input
{
    public class MoveCommand : ICommand
    {
        private Vector3 _to;
        private float _moveSpeed;

        public MoveCommand(Vector3 to, float moveSpeed)
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
