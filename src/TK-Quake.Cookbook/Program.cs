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
            /*
            var bsp = new Engine.Loader.BSPLoader ();
            bsp.SetBSPFile ("/home/bidski/Projects/COMP3320/q3dm6.bsp");
            bsp.LoadFile ();
            bsp.DumpBSP ();
            */

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

        private void game_RenderFrame(object sender, FrameEventArgs e)
        {
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
            renderer.LinkShaders();

            currentScreen = new CameraTestScreen("q3dm6.bsp", (float)game.Width / game.Height);

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
