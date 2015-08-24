using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Infrastructure.Texture;
using TKQuake.Sprites;

namespace TKQuake.Engine.Infrastructure.GameScreen
{
    public class DrawSpritesScreen : GameScreen
    {
        public static new string StateNameKey = "DrawSprites";

        private Face _face;

        public DrawSpritesScreen(ScreenManager screenManager, TextureManager textureManager)
        {
            this._screenManager = screenManager;
            this._textureManager = textureManager;

            _face = new Face(_textureManager.Get("faceAlpha"));
            _face.X = -400;
            _face.Y = -100;
            _face.SetScale(0.5);
        }

        /*
        public override void Update(double elapsedTime)
        {
            _face.Update(elapsedTime);
        }

        public override void Render()
        {
            GL.ClearColor(System.Drawing.Color.Blue);
            _face.Render();
        }
        */
    }
}
