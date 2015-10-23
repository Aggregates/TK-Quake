using TKQuake.Engine;
using TKQuake.Engine.Infrastructure.Math;
using TKQuake.Fonts;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Core;
using TKQuake.Engine.Debug;
using TKQuake.Engine.Infrastructure.Font;
using TKQuake.Engine.Infrastructure.GameScreen;
using TKQuake.Engine.Infrastructure.Texture;
using TKQuake.Engine.Infrastructure.Entities;
using TKQuake.Engine.Infrastructure.Components;

namespace TKQuake.ScreenStates
{
    public class TestFontScreen : GameScreen
    {
        public static new string StateNameKey = "TestFontScreen";

        private Font _font;
        //private TextEntity _text;

        public TestFontScreen(ScreenManager screenManager
            , TextureManager texManager
            , FontManager fontManager)
        {
            this._screenManager = screenManager;
            this._textureManager = texManager;
            this._fontManager = fontManager;

            this._renderer = Renderer.Singleton();
            //this._spriteList = new List<Sprite2>();

            /*
            _spriteList.Add( new MyriadPro(texManager.Get("myriadPro")) );
            _spriteList[0].SetUVs(new Point((float)(4.0/255.0), 0), new Point((float)(14.0/255.0),(float)(23.0/255.0)));
            _spriteList[0].RenderWidth = 10;
            _spriteList[0].RenderHeight = 23;
             */

            _font = _fontManager.Get("myriadPro");
            //_text = new TextEntity(string.Format("FPS: {0}", FramesPerSecond.CurrentFPS), _font);
            //_text = new Text("The quick brown fox jumped over the lazy dog", _font, 200);
            //_text.SetColor(new Color(1,1,1,1));

        }

        /*
        public override void Update(double elapsedTime)
        {
            FramesPerSecond.Process(elapsedTime);
            _text = new TextEntity(string.Format("FPS: {0}", FramesPerSecond.CurrentFPS), _font);
            //_text.SetColor(new Color(1, 1, 1, 1));
        }

        public override void Render()
        {
            GL.ClearColor(System.Drawing.Color.Green);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            _renderer.DrawText(_text);
            _renderer.Render();
        }
        */
    }
}
