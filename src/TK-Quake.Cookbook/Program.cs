using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Cookbook.Screens;
using TKQuake.Engine.Extensions;
using TKQuake.Engine.Infrastructure.GameScreen;
using TKQuake.Engine.Infrastructure.Texture;

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
            currentScreen = new CameraTestScreen();

            using (game = new GameWindow())
            {
                game.Load += game_Load;
                game.Resize += game_Resize;
                game.UpdateFrame += game_UpdateFrame;
                game.RenderFrame += game_RenderFrame;
                game.KeyDown += game_KeyDown;
                game.KeyUp += game_KeyUp;
                game.KeyPress += game_KeyPress;

                game.Run(60.0);
            }
        }

        private void game_KeyPress(object sender, KeyPressEventArgs e)
        {
            Console.WriteLine(e.KeyChar);
        }

        void game_KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void game_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            ((CameraTestScreen)currentScreen).HandleInput(e.Key);
        }

        private void game_RenderFrame(object sender, FrameEventArgs e)
        {
            currentScreen.Render(e);
            game.SwapBuffers();
        }

        private void game_UpdateFrame(object sender, FrameEventArgs e)
        {
            if (game.Keyboard[Key.Escape])
                game.Exit();

            currentScreen.Update(0, e);


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
