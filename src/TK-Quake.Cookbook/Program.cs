using GameLoop.Engine.Extensions;
using GameLoop.Engine.Infrastructure.Texture;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLoop.Cookbook
{
    public class Program
    {
        private GameWindow game;

        const float rotation_speed = 180.0f;
        float angle = 0.0f;

        public static void Main(string[] args)
        {
            Program prog = new Program();
            prog.Run();
        }

        private void Run()
        {
            using (game = new GameWindow())
            {
                game.Load += game_Load;
                game.Resize += game_Resize;
                game.UpdateFrame += game_UpdateFrame;
                game.RenderFrame += game_RenderFrame;

                game.Run(60.0);
            }
        }

        private void game_RenderFrame(object sender, FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            /*
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-10, 10, -10, 10, 0.0, 4.0);
            */

            Matrix4 lookat = Matrix4.LookAt(0, 5, 5, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);

            angle += rotation_speed * (float)e.Time;
            GL.Rotate(angle, 0.0f, 1.0f, 0.0f);


            GL.Begin(PrimitiveType.Quads);
            {
                /*GLX.Color3(Color.Red);
                GL.Vertex2(-1.0f, 1.0f);
                GLX.Color3(Color.Green);
                GL.Vertex2(0.0f, -1.0f);
                GLX.Color3(Color.Blue);
                GL.Vertex2(1.0f, 1.0f);
                 */
                DrawCube();
            }
            GL.End();

            game.SwapBuffers();
        }

        private void DrawCube()
        {
            GLX.Color3(Color.Red);
            GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);
            GL.Vertex3(1.0f, 1.0f, -1.0f);
            GL.Vertex3(1.0f, -1.0f, -1.0f);

            GLX.Color3(Color.Green);
            GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.Vertex3(1.0f, -1.0f, -1.0f);
            GL.Vertex3(1.0f, -1.0f, 1.0f);
            GL.Vertex3(-1.0f, -1.0f, 1.0f);

            GLX.Color3(Color.Blue);
            GL.Vertex3(-1.0f, -1.0f, -1.0f);
            GL.Vertex3(-1.0f, -1.0f, 1.0f);
            GL.Vertex3(-1.0f, 1.0f, 1.0f);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);

            GLX.Color3(Color.Red);
            GL.Vertex3(-1.0f, -1.0f, 1.0f);
            GL.Vertex3(1.0f, -1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(-1.0f, 1.0f, 1.0f);

            GLX.Color3(Color.Green);
            GL.Vertex3(-1.0f, 1.0f, -1.0f);
            GL.Vertex3(-1.0f, 1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, -1.0f);

            GLX.Color3(Color.Blue);
            GL.Vertex3(1.0f, -1.0f, -1.0f);
            GL.Vertex3(1.0f, 1.0f, -1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f);
            GL.Vertex3(1.0f, -1.0f, 1.0f);
        }

        private void game_UpdateFrame(object sender, FrameEventArgs e)
        {
            if (game.Keyboard[Key.Escape])
                game.Exit();
        }

        private void game_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, game.Width, game.Height);

            double aspect_ratio = game.Width / (double)game.Height;

            OpenTK.Matrix4 perspective = OpenTK.Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)aspect_ratio, 1, 64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);
        }

        private void game_Load(object sender, EventArgs e)
        {
            game.VSync = VSyncMode.On;
            GL.Enable(EnableCap.DepthTest);
            Console.Write("GL Window loaded");
        }
    }
}
