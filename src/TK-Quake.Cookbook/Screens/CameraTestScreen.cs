using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Core;
using TKQuake.Engine.Extensions;
using TKQuake.Engine.Infrastructure.GameScreen;
using TKQuake.Engine.Infrastructure.Texture;

namespace TKQuake.Cookbook.Screens
{
    public class CameraTestScreen : GameScreen
    {
        private Camera _camera = new Camera();

        public void HandleInput(Key k)
        {
            switch(k)
            {
                case Key.A: _camera.Move(-0.01, 0, 0); break;
                case Key.D: _camera.Move(0.01, 0, 0); break;
                case Key.W: _camera.Move(0, 0, 0.01); break;
                case Key.S: _camera.Move(0, 0, -0.01); break;
                default: break;
            }
        }

        public override void Update(double elapsedTime, FrameEventArgs e)
        {
            base.Update(elapsedTime, e);
        }

        public override void Update(double elapsedTime)
        {
            _camera.Update(elapsedTime);
        }

        public override void Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadIdentity();

            //var game = new { Width=600, Height=600 };
            //var ClientSize = new { Width = 600, Height = 600 };

            //Matrix4 viewProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(1.3f, game.Width / (float)game.Height, 1.0f, 40.0f);
            //viewProjectionMatrix = _camera.WorldToLocalMatrix() * Matrix4.CreatePerspectiveFieldOfView(1.3f, ClientSize.Width / (float)ClientSize.Height, 1.0f, 40.0f);
            GL.Enable(EnableCap.DepthTest);
            _camera.Render();


            // Display some planes
            for (int x = -10; x <= 10; x++)
            {
                for (int z = -10; z <= 10; z++)
                {
                    GL.PushMatrix();
                    GL.Translate((float)x * 5f, 0f, (float)z * 5f);
                    GL.Begin(PrimitiveType.Quads);
                    {
                        GLX.Color3(Color.Red);
                        GL.Vertex3(1f, 4f, 0f);
                        GLX.Color3(Color.Green);
                        GL.Vertex3(-1f, 4f, 0f);
                        GLX.Color3(Color.Blue);
                        GL.Vertex3(-1f, 0f, 0f);
                        GLX.Color3(Color.White);
                        GL.Vertex3(1f, 0f, 0f);
                    }
                    GL.End();
                    GL.PopMatrix();
                }
            }

            /*

            GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);


            GL.Begin(PrimitiveType.Triangles);

            GLX.Color3(Color.Blue);
            GL.Vertex2(-1.0f, 1.0f);
            GLX.Color3(Color.Green);
            GL.Vertex2(0.0f, -1.0f);
            GLX.Color3(Color.Red);
            GL.Vertex2(1.0f, 1.0f);

            GL.End();
             *
             * */

        }
    }
}
