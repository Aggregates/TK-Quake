using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.IO;
using OpenTK.Graphics;
using TKQuake.Cookbook.Screens;
using TKQuake.Engine.Core;
using TKQuake.Engine.Infrastructure.GameScreen;
using TKQuake.Engine.Infrastructure.Input;

namespace TKQuake.Cookbook
{
    public class Program
    {
        private GameWindow game;
        private GameScreen currentScreen;

        public static void Main(string[] args)
        {
            var prog = new Program();
            prog.Run();
        }

        private void Run()
        {
            using (game = new GameWindow(800, 600, GraphicsMode.Default, "TK-Quake", GameWindowFlags.Default,
                DisplayDevice.Default, 1, 0, GraphicsContextFlags.ForwardCompatible | GraphicsContextFlags.Debug))
            {
                game.Load += game_Load;
                game.Resize += game_Resize;
                game.UpdateFrame += game_UpdateFrame;
                game.RenderFrame += game_RenderFrame;

                game.Run(60.0, 60.0);
            }
        }

        static double _time = 0.0, _frames = 0.0;
        static int _fps = 0;

        public static int GetFps(double time)
        {
            _time += time;
            if (_time < 1.0)
            {
                _frames++;
                return _fps;
            }
            else
            {
                _fps = (int)_frames;
                _time = 0.0;
                _frames = 0.0;
                return _fps;
            }
        }

        private void game_RenderFrame(object sender, FrameEventArgs e)
        {
            Console.Write("FPS: {0}\r", GetFps(e.Time));
            
            // Store the current view model projection data
            var renderer = Renderer.Singleton();
            var model = renderer.GetUniform("model");
            var view = renderer.GetUniform("view");
            var proj = renderer.GetUniform("proj");

            var mvp = model*view*proj;
            var inverseModelView = (model*view).Inverted();

            var uniPrevMvpLocation = GL.GetUniformLocation(renderer.Program, "uPrevModelViewProj");
            GL.UniformMatrix4(uniPrevMvpLocation, false, ref mvp);

            var uInverseModelViewMat = GL.GetUniformLocation(renderer.Program, "uInverseModelViewMat");
            GL.UniformMatrix4(uInverseModelViewMat, false, ref inverseModelView);
            
            game.SwapBuffers();
        }

        private void game_UpdateFrame(object sender, FrameEventArgs e)
        {
            var kbState = Keyboard.GetState();
            if (kbState[Key.Escape])
                game.Exit();
            if (kbState.IsKeyDown(Key.AltLeft) && kbState.IsKeyDown(Key.Enter))
                game.WindowState = game.WindowState == WindowState.Normal ? WindowState.Fullscreen : WindowState.Normal;

            GL.Enable(EnableCap.DepthTest);
            
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            currentScreen.Update(e.Time);

            CommandCentre.ExecuteAllCommands();
        }

        private void game_Resize(object sender, EventArgs e)
        {
            var renderer = Renderer.Singleton();

            var proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), (float)game.Width / game.Height, 0.1f, 1000f);
            var uniProj = GL.GetUniformLocation(renderer.Program, "proj");
            GL.UniformMatrix4(uniProj, false, ref proj);
            GL.Viewport(0, 0, game.Width, game.Height);
        }

        private void game_Load(object sender, EventArgs e)
        {
            var renderer = Renderer.Singleton();
            renderer.LoadShader(File.ReadAllText(Path.Combine("Shaders", "shader.vert")), ShaderType.VertexShader);
            renderer.LoadShader(File.ReadAllText(Path.Combine("Shaders", "shader.frag")), ShaderType.FragmentShader);

            // Motion Blur shader doesn't work and breaks Particle transparency
            //renderer.LoadShader(File.ReadAllText(Path.Combine("Shaders", "MotionBlur.shader")), ShaderType.FragmentShader);

            renderer.LinkShaders();

            //currentScreen = new CameraTestScreen("maps/q3dm6.bsp");
            currentScreen = new CollisionTestScreen(renderer);

            GL.ClearColor(0.25f, 0.25f, 0.25f, 1);

            Console.Write("GL Window loaded");
            Console.WriteLine();
            Console.WriteLine("Vendor: " + GL.GetString(StringName.Vendor));
            Console.WriteLine("Version: " + GL.GetString(StringName.Version));
            Console.WriteLine("Renderer: " + GL.GetString(StringName.Renderer));
            Console.WriteLine("Shading Language Version: " + GL.GetString(StringName.ShadingLanguageVersion));
        }
    }
}
