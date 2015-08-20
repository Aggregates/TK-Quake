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
using TKQuake.Engine.Infrastructure.Math;
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
                case Key.W:
                    {
                        // Forward
                        double x = (float)Math.Cos(_camera.YawAngle + Math.PI / 2) * 0.1f;
                        double z = (float)Math.Sin(_camera.YawAngle + Math.PI / 2) * 0.1f;
                        _camera.Move(-x, 0, -z);
                        break;
                    }

                case Key.S:
                    {
                        // Back
                        double x = (float)Math.Cos(_camera.YawAngle + Math.PI / 2) * 0.1f;
                        double z = (float)Math.Sin(_camera.YawAngle + Math.PI / 2) * 0.1f;
                        _camera.Move(x, 0, z);
                        break;
                    }

                case Key.A:
                    {
                        // Strafe left
                        double x = (float)Math.Cos(_camera.YawAngle) * 0.1f;
                        double z = (float)Math.Sin(_camera.YawAngle) * 0.1f;
                        _camera.Move(-x, 0, -z);
                        break;
                    }
                case Key.D:
                    {
                        // Strafe right
                        double x = (float)Math.Cos(_camera.YawAngle) * 0.1f;
                        double z = (float)Math.Sin(_camera.YawAngle) * 0.1f;
                        _camera.Move(x, 0, z);
                        break;
                    }

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

        }
    }
}
