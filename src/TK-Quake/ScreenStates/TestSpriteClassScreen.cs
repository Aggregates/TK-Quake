using TKQuake.Engine;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Core;
using TKQuake.Engine.Infrastructure.GameScreen;
using TKQuake.Engine.Infrastructure.Texture;
using TKQuake.Sprites;

namespace TKQuake.ScreenStates
{
    public class TestSpriteClassScreen : GameScreen
    {

        public static new string StateNameKey = "TestSprites";

        public TestSpriteClassScreen(ScreenManager screenManager, TextureManager texManager)
        {
            this._screenManager = screenManager;
            this._textureManager = texManager;

            this._renderer = new Renderer();
            this._spriteList = new List<Sprite2>();
            _spriteList.Add(new Face(texManager.Get("face")));
            _spriteList.Add(new Face(texManager.Get("faceAlpha")));

            _spriteList[1].SetColor(new Color(1, 0, 0, 1));
            _spriteList[1].SetPosition(256, 256);
        }

        public override void Update(double elapsedTime)
        {
            //throw new NotImplementedException();
        }

        public override void Render()
        {
            GL.ClearColor(System.Drawing.Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            _renderer.DrawSprites(_spriteList);
            _renderer.Render();
            GL.Finish();
        }
    }
}
