using GameLoop.Engine.Core;
using GameLoop.Engine.Infrastructure.Abstract;
using GameLoop.Engine.Infrastructure.Font;
using GameLoop.Engine.Infrastructure.Texture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLoop.Engine.Infrastructure.GameScreen
{
    public abstract class GameScreen : IGameObject
    {
        protected ScreenManager _screenManager;
        protected TextureManager _textureManager;
        protected Renderer _renderer;
        protected List<Sprite2> _spriteList;
        protected FontManager _fontManager;

        public static string StateNameKey;

        public abstract void Update(double elapsedTime);
        public abstract void Render();
    }
}
