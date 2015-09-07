using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Cookbook.Screens;
using TKQuake.Engine.Core;
using TKQuake.Engine.Extensions;
using TKQuake.Engine.Infrastructure.GameScreen;
using TKQuake.Engine.Infrastructure.Texture;
using TKQuake.Engine.Infrastructure.Input;

namespace TKQuake.Cookbook
{
    public class Program
    {
        private GameWindow game;
        private GameScreen currentScreen;

        public static void Main(string[] args)
        {
            Program prog = new Program();
            prog.Run();
        }

        private void Run()
        {
            using (game = new GameWindow())
            {
                var renderer = Renderer.Singleton();
                renderer.LoadShader(File.ReadAllText(Path.Combine("Shaders", "shader.frag")), ShaderType.FragmentShader);
                renderer.LoadShader(File.ReadAllText(Path.Combine("Shaders", "shader.vert")), ShaderType.VertexShader);
                renderer.LinkShaders();

                currentScreen = new CameraTestScreen(renderer);

                game.Load += game_Load;
                game.Resize += game_Resize;
                game.UpdateFrame += game_UpdateFrame;
                game.RenderFrame += game_RenderFrame;

                game.Run(60.0, 60.0);
            }
        }

        private void game_RenderFrame(object sender, FrameEventArgs e)
        {
            GL.Flush();
            game.SwapBuffers();
        }

        private void game_UpdateFrame(object sender, FrameEventArgs e)
        {
            if (game.Keyboard[Key.Escape])
                game.Exit();

            GL.Viewport(0, 0, game.Width, game.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            currentScreen.Update(e.Time);

            CommandCentre.ExecuteAllCommands();
        }

        private void game_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, game.Width, game.Height);
            GL.ClearColor(0, 0, 0, 1); // Change Screen colour
            double aspect_ratio = game.Width / (double)game.Height;

            OpenTK.Matrix4 perspective = OpenTK.Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)aspect_ratio, 1, 64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);
        }

        private void game_Load(object sender, EventArgs e)
        {
            game.VSync = VSyncMode.On;
            GL.ClearColor(0, 0, 1, 0);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.LineWidth(3);    // Thickens Lines

            Console.Write("GL Window loaded");
            Console.WriteLine();
            Console.WriteLine("Vendor: " + GL.GetString(StringName.Vendor));
            Console.WriteLine("Version: " + GL.GetString(StringName.Version));
            Console.WriteLine("Renderer: " + GL.GetString(StringName.Renderer));
            Console.WriteLine("Shading Language Version: " + GL.GetString(StringName.ShadingLanguageVersion));
        }
    }
}
