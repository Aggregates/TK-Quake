using TKQuake.Engine;
using TKQuake.Engine.Core;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using TKQuake.Engine.Infrastructure.Font;
using TKQuake.Engine.Infrastructure.GameScreen;
using TKQuake.Engine.Infrastructure.Texture;
using TKQuake.Engine.Infrastructure.Input;
using TKQuake.ScreenStates;

namespace TKQuake
{
    public class Game
    {
        private bool _fullScreen = false;
        private readonly ScreenManager _stateManager;
        private readonly TextureManager _textureManager;
        private readonly FontManager _fontManager;
        private readonly GameWindow _game;
        private readonly InputSystem _inputSystem;
        private static readonly string ResourcesPath =
            Path.Combine(Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location), "Resources");

        public Game()
        {
            _game = new GameWindow();
            _stateManager = new ScreenManager();
            _textureManager = new TextureManager();
            _fontManager = new FontManager();
            _inputSystem = new InputSystem();

            SetupViewport();
        }

        private void RegisterScreens()
        {
            SplashScreen splash = new SplashScreen(_stateManager);
            TitleScreen title = new TitleScreen(_stateManager);
            DrawSpritesScreen sprites = new DrawSpritesScreen(_stateManager, _textureManager);
            TestSpriteClassScreen testSprites = new TestSpriteClassScreen(_stateManager, _textureManager);
            TestFontScreen renderFont = new TestFontScreen(_stateManager, _textureManager, _fontManager);

            _stateManager.Add(SplashScreen.StateNameKey, splash);
            _stateManager.Add(TitleScreen.StateNameKey, title);
            _stateManager.Add(DrawSpritesScreen.StateNameKey, sprites);
            _stateManager.Add(TestSpriteClassScreen.StateNameKey, testSprites);
            _stateManager.Add(TestFontScreen.StateNameKey, renderFont);

            _stateManager.ChangeScreen(TestFontScreen.StateNameKey);
        }

        private void RegisterTextures()
        {
            //guarantees support cross platform
            var texturesPath = Path.Combine(ResourcesPath, "Textures");
            var fontsPath = Path.Combine(ResourcesPath, "Fonts");

            _textureManager.Add("face", Path.Combine(texturesPath, "Face.bmp"));
            _textureManager.Add("faceAlpha",
                Path.Combine(texturesPath, "FaceAlpha.png"));
            _textureManager.Add("myriadPro",
                Path.Combine(fontsPath, "MyriadPro.tga"));
        }

        private void RegisterFonts()
        {
            var fontsPath = Path.Combine(ResourcesPath, "Fonts");

            Texture myriadText = _textureManager.Get("myriadPro");
            Font myriadPro = new Font(myriadText,
                Path.Combine(fontsPath, "MyriadPro.fnt"));
            _fontManager.Add("myriadPro", myriadPro);
        }

        /// <summary>
        /// Modifies GL Viewport to use Top-Left corner as (0,0)
        /// </summary>
        private void SetupViewport()
        {
            int height = _game.Height;
            int width = _game.Width;

            GL.Ortho(0, width, height, 0, -1, 1);
            GL.Viewport(0, 0, width, height);
        }

        private void Setup2DGraphics(double width, double height)
        {
            double halfWidth = width / 2;
            double halfHeight = height / 2;

            // Update the projection matrix
            GL.MatrixMode(MatrixMode.Projection);   // Sets the current matrix to use Projection Matrix
            GL.LoadIdentity();                      // Clear matrix data and reset to Identity Matrix
            GL.Ortho(-halfWidth, halfWidth, -halfHeight, halfHeight, -100, 100);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        public void Run()
        {
            _game.UpdateFrame += (sender, args) => GameLoop(args.Time);
            _game.Load += Load;
            _game.Resize += Resize;
            _game.RenderFrame += Render;

            //Input processing
            _game.KeyDown += (sender, args)
                => _inputSystem.ProcessKeyboardInput(args.Key);
            _game.MouseDown += (sender, args)
                => _inputSystem.ProcessMouseInput(args.Button);
            //end Input processing

            //start game loop
            _game.Run();
        }

        private void GameLoop(double elapsedTime)
        {
            _stateManager.Update(elapsedTime);
        }

        private void Render(object sender, FrameEventArgs e)
        {
            // Clear the buffers before rendering frame
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //_stateManager.Render();

            _game.SwapBuffers();
        }

        private void Load(object sender, EventArgs e)
        {
            /*
             * Apparently this event is not called at any time. So this may not work.
             * But something is obviously calling it.
             */
            _game.VSync = VSyncMode.On;

            GL.ClearColor(System.Drawing.Color.CornflowerBlue);

            RegisterTextures();
            RegisterFonts();
            RegisterScreens();

            // Use fullscreen in production environments
            if (_fullScreen)
            {
                _game.WindowState = WindowState.Fullscreen;
            }
            else
            {
                _game.WindowState = WindowState.Normal;
                _game.ClientSize = new System.Drawing.Size(1280, 720);
            }

            // Set the viewport
            Setup2DGraphics(_game.Width, _game.Height);
        }

        private void Resize(object sender, EventArgs e)
        {
            // Update the GL Viewport dimensions
            GL.Viewport(0, 0, _game.Width, _game.Height);

            // Update the GL projection matrix
            Setup2DGraphics(_game.Width, _game.Height);
        }
    }
}
