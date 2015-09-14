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
                DisplayDevice.Default, 4, 0, GraphicsContextFlags.ForwardCompatible | GraphicsContextFlags.Debug))
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
            if (game.Keyboard[Key.Escape])
                game.Exit();

            GL.Enable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            currentScreen.Update(e.Time);

            CommandCentre.ExecuteAllCommands();
        }

        private void game_Resize(object sender, EventArgs e)
        {
            double aspect_ratio = game.Width / (double)game.Height;

            OpenTK.Matrix4 perspective = OpenTK.Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)aspect_ratio, 1, 64);
            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadMatrix(ref perspective);
        }

        private void game_Load(object sender, EventArgs e)
        {
            var renderer = Renderer.Singleton();
            renderer.LoadShader(File.ReadAllText(Path.Combine("Shaders", "shader.vert")), ShaderType.VertexShader);
            renderer.LoadShader(File.ReadAllText(Path.Combine("Shaders", "shader.frag")), ShaderType.FragmentShader);
            renderer.LinkShaders();

            currentScreen = new CameraTestScreen(renderer);

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
