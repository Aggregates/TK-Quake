using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Infrastructure.Components;
using TKQuake.Engine.Infrastructure.Entities;

namespace TKQuake.Engine.Debug
{
    public class DebugPositionComponent : IComponent
    {
        private static List<IEntity> _entities = new List<IEntity>();

        public DebugPositionComponent(IEntity entities)
        {
            _entities.Add(entities);
        }

        public void Startup() { }

        public void Shutdown() { }

        public void Update(double elapsedTime)
        {
            Console.Clear();
            foreach (var entity in _entities)
            {
                Console.WriteLine("Entity: {0} - Position: {1}", entity, entity.Position);
            }
        }
    }
}
