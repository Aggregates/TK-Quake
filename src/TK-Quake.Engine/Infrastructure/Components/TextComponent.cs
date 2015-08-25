using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Infrastructure.Math;
using TKQuake.Engine.Infrastructure.Font;
using TKQuake.Engine.Infrastructure.Entities;
using TKQuake.Engine.Core;

namespace TKQuake.Engine.Infrastructure.Components
{
    /// <summary>
    /// Defines the core mechanics for any game object used in an OpenTK game
    /// </summary>
    public class TextComponent : IComponent
    {
        private readonly TextEntity _entity;
        private readonly Renderer _renderer = new Renderer();

        public TextComponent(TextEntity entity) {
            _entity = entity;
        }

        public void Update(double elapsedTime) {
            _renderer.DrawText(_entity);
        }

        public void Startup() {
            //load what we need to render the entity
        }

        public void Shutdown() {

        }
    }
}
