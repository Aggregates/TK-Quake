using System;
using System.Collections;
using System.Collections.Generic;
using TKQuake.Engine.Infrastructure.Math;
using TKQuake.Engine.Infrastructure.Components;

namespace TKQuake.Engine.Infrastructure.Entities
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
