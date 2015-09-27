using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Infrastructure.Components;
using TKQuake.Engine.Infrastructure.Entities;

namespace TKQuake.Engine.Infrastructure.Physics
{
    public class PickupComponent : IComponent
    {
        private Entity _entity;
        private BoundingBoxEntity _box;

        public PickupComponent(Entity entity)
        {
            this._entity = entity;

            // Find the attached bounding box
            var box = _entity.Children.OfType<BoundingBoxEntity>().FirstOrDefault();
            if (box == null)
                throw new ArgumentException("The entity does not hav a Bounding Box Component attached to it");

            this._box = box;
            _box.Collided += Box_Collided;
        }

        private void Box_Collided(object sender, CollisionEventArgs e)
        {
            // HACK: Pickups only work for Camera class. Need to extend for AI
            if (e.Collider is TKQuake.Engine.Core.Camera || e.Sender is TKQuake.Engine.Core.Camera)
            {
                _entity.DestroyEntity();
            }
        }

        public void Startup() { }
        public void Shutdown() { }
        public void Update(double elapsedTime) { }
    }
}
