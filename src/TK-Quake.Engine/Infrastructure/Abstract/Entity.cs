using System;
using System.Collections;
using System.Collections.Generic;
using TKQuake.Engine.Infrastructure.Math;
using System.Threading.Tasks;

namespace TKQuake.Engine.Infrastructure.Abstract
{
    public abstract class Entity
    {
        public List<IComponent> Components { get; set; }
        public List<IEntity> Children { get; set; }
        public Vector Position { get; set; }
        public Vector Rotation { get; set; }
        public Vector ViewDirection { get; }
        public Vector OrthogonalDirection { get; }

        public void Update(double elapsedTime) {
            //update all components
            Components.ForEach(c => c.Update(elapsedTime));

            //update all children
            Children.ForEach(c => c.Update(elapsedTime));
        }
    }
}
