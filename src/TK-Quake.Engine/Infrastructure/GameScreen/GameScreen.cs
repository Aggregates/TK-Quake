using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Core;
using TKQuake.Engine.Infrastructure.Abstract;
using TKQuake.Engine.Infrastructure.Font;
using TKQuake.Engine.Infrastructure.Texture;
using TKQuake.Engine.Infrastructure.Entities;

namespace TKQuake.Engine.Infrastructure.GameScreen
{
    public abstract class GameScreen : Entity
    {
        protected ScreenManager _screenManager;
        protected TextureManager _textureManager;
        protected Renderer _renderer;
//        protected List<Sprite2> _spriteList;
        protected FontManager _fontManager;

        public static string StateNameKey;
    }
}
