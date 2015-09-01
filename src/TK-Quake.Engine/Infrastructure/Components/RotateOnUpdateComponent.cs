using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using TKQuake.Engine.Infrastructure.Entities;

namespace TKQuake.Engine.Infrastructure.Components
{
    public class RotateOnUpdateComponent : IComponent
    {
        private readonly IEntity _entity;
        private readonly Vector3 _rotation;

        public RotateOnUpdateComponent(IEntity entity, Vector3 rotation)
        {
            _entity = entity;
            _rotation = rotation;
        }

        public void Startup() { }
        public void Shutdown() { }

        public void Update(double elapsedTime)
        {
            var rotation = _rotation*(float)elapsedTime;
            _entity.Rotation += rotation;
        }
    }
}
