using System;
using System.Collections;
using System.Collections.Generic;
using TKQuake.Engine.Infrastructure.Math;

namespace TKQuake.Engine.Infrastructure.Abstract
{
    public class RenderableEntity : Entity
    {
        private RenderableEntity() {
            Components.Add(new RenderComponent(this));
        }

        //mesh data
        //texture data
        //lighting

        public static RenderableEntity Create() {
            return new RenderableEntity();
        }
    }
}
