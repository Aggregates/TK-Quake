using GameLoop.Engine;
using GameLoop.Engine.Core;
using GameLoop.Engine.Infrastructure.Font;
using GameLoop.Engine.Infrastructure.GameScreen;
using GameLoop.Engine.Infrastructure.Texture;
using GameLoop.ScreenStates;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
//using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameLoop
{
    public partial class GlForm : Form
    {
        private bool _glComponentLoaded = false;
        private bool _fullScreen = false;
        private FastLoop _fastLoop;
        private ScreenManager _stateManager;
        private TextureManager _textureManager;
        private FontManager _fontManager;
        
        public GlForm()
        {
            InitializeComponent();

            _fastLoop = new FastLoop(GameLoop);
            _stateManager = new ScreenManager();
            _textureManager = new TextureManager();
            _fontManager = new FontManager();
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
            _textureManager.Add("face", @"Resources\Textures\Face.bmp");
            _textureManager.Add("faceAlpha", @"Resources\Textures\FaceAlpha.png");
            _textureManager.Add("myriadPro", @"Resources\Fonts\MyriadPro.tga");
        }

        private void RegisterFonts()
        {
            Texture myriadText = _textureManager.Get("myriadPro");
            Font myriadPro = new Font(myriadText, @"Resources\Fonts\MyriadPro.fnt");
            _fontManager.Add("myriadPro", myriadPro);
        }

        /// <summary>
        /// Modifies GL Viewport to use Top-Left corner as (0,0)
        /// </summary>
        private void SetupViewport()
        {
            int height = glControl.Height;
            int width = glControl.Width;

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

        private void GameLoop(double elapsedTime)
        {
            if (!_glComponentLoaded)
                return;

            // Clear the buffers before rendering frame
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _stateManager.Update(elapsedTime);
            _stateManager.Render();
            
            // Last part of update to finish off render
            GL.Finish();
            glControl.Refresh();
            glControl.SwapBuffers();

        }

        private void GlForm_Load(object sender, EventArgs e)
        {
            //this._glComponentLoaded = true;
        } 

        private void glControl_Load(object sender, EventArgs e)
        {
            /*
             * Apparently this event is not called at any time. So this may not work.
             * But something is obviously calling it. 
             */

            this._glComponentLoaded = true;
            GL.ClearColor(System.Drawing.Color.CornflowerBlue);
            //SetupViewport();
            RegisterTextures();
            RegisterFonts();
            RegisterScreens();

            // Use fullscreen in production environments
            if (_fullScreen)
            {
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                ClientSize = new System.Drawing.Size(1280, 720);
            }

            // Set the viewport
            Setup2DGraphics(ClientSize.Width, ClientSize.Height);
        }

        private void glControl_Resize(object sender, EventArgs e)
        {
            if (!_glComponentLoaded)
                return;
        }

        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            if (!_glComponentLoaded)
                return;
        }

        /// <summary>
        /// Update the client size. This is all elements inside the window and does not include the
        /// frame or title bar
        /// </summary>
        /// <param name="sender">The form</param>
        /// <param name="e">Prameters sent by the form</param>
        private void glControl_ClientSizeChanged(object sender, EventArgs e)
        {
            base.OnClientSizeChanged(e);
            
            // Update the GL Viewport dimensions
            GL.Viewport(0, 0, this.ClientSize.Width, this.ClientSize.Height);
            
            // Update the GL projection matrix
            Setup2DGraphics(ClientSize.Width, ClientSize.Height);
        }

    }
}
