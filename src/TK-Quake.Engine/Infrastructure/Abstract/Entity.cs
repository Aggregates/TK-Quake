using System;
using System.Collections;
using System.Collections.Generic;
using TKQuake.Engine.Infrastructure.Math;
using System.Threading.Tasks;

namespace TKQuake.Engine.Infrastructure.Abstract
{
    public abstract class Entity
    {
        public IList<IComponent> Components { get; set; } = new List<IComponent>();
        public IList<IEntity> Children { get; set; } = new List<IEntity>();
        public Vector Position { get; set; }
        public Vector Rotation { get; set; }
        public Vector ViewDirection { get; }
        public Vector OrthogonalDirection { get; }

        public void Update(double elapsedTime) {
            //update all components
            foreach (var component in Components)
            {
                component.Update(elapsedTime);
            }

            //update all children
            foreach (var entity in Children)
            {
                entity.Update(elapsedTime);
            }
        }
    }
}
