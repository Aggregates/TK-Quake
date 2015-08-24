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
using TKQuake.Engine.Infrastructure.Abstract;
using TKQuake.Engine.Infrastructure.GameScreen;
using TKQuake.Engine.Infrastructure.Math;
using TKQuake.Engine.Infrastructure.Texture;

namespace TKQuake.Cookbook.Screens
{
    public class CameraTestScreen : GameScreen
    {
        private readonly Camera _camera = new Camera();

        public CameraTestScreen()
        {
            Children.Add(_camera);
            Components.Add(new MovementInputComponent(_camera));
            Components.Add(new PlanesComponent());
        }
    }

    class PlanesComponent : IComponent
    {
        public void Startup() { }
        public void Shutdown() { }
        public void Update(double elapsedTime)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

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
                        GL.Vertex3(-1f, -4f, 0f);
                        GLX.Color3(Color.White);
                        GL.Vertex3(1f, -4f, 0f);
                    }
                    GL.End();
                    GL.PopMatrix();
                }
            }
        }
    }

    class MovementInputComponent : IComponent
    {
        private readonly PlayerEntity _entity;
        public MovementInputComponent(PlayerEntity entity)
        {
            _entity = entity;
        }

        public void Startup() { }
        public void Shutdown() { }
        public void Update(double elapsedTime) {
            var state = Keyboard.GetState();
            if (state[Key.W]) HandleInput(Key.W);
            if (state[Key.A]) HandleInput(Key.A);
            if (state[Key.S]) HandleInput(Key.S);
            if (state[Key.D]) HandleInput(Key.D);
            if (state[Key.Left]) HandleInput(Key.Left);
            if (state[Key.Right]) HandleInput(Key.Right);
            if (state[Key.Up]) HandleInput(Key.Up);
            if (state[Key.Down]) HandleInput(Key.Down);
        }

        public void HandleInput(Key k)
        {
            switch(k)
            {
                case Key.W:
                    {
                        // Forward
                        double x = (float)Math.Cos(_entity.Rotation.Y + Math.PI / 2);
                        double z = (float)Math.Sin(_entity.Rotation.Y + Math.PI / 2);
                        _entity.Move(-x, 0, -z);
                        break;
                    }

                case Key.S:
                    {
                        // Back
                        double x = (float)Math.Cos(_entity.Rotation.Y + Math.PI / 2);
                        double z = (float)Math.Sin(_entity.Rotation.Y + Math.PI / 2);
                        _entity.Move(x, 0, z);
                        break;
                    }

                case Key.A:
                    {
                        // Strafe left
                        double x = (float)Math.Cos(_entity.Rotation.Y);
                        double z = (float)Math.Sin(_entity.Rotation.Y);
                        _entity.Move(-x, 0, -z);
                        break;
                    }
                case Key.D:
                    {
                        // Strafe right
                        double x = (float)Math.Cos(_entity.Rotation.Y);
                        double z = (float)Math.Sin(_entity.Rotation.Y);
                        _entity.Move(x, 0, z);
                        break;
                    }
                case Key.Left: { _entity.Rotate(0, -1, 0); break; }
                case Key.Right: { _entity.Rotate(0, 1, 0); break; }
                case Key.Up: { _entity.Rotate(1, 0, 0); break; }
                case Key.Down: { _entity.Rotate(-1, 0, 0); break; }
            }
        }
    }
}
