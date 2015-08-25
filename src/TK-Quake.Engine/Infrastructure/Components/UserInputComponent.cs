using System;
using OpenTK.Input;
using TKQuake.Engine.Infrastructure.Abstract;
using TKQuake.Engine.Infrastructure.Entities;
using static System.Math;

namespace TKQuake.Engine.Infrastructure.Components
{
    public class UserInputComponent : IComponent
    {
        private readonly PlayerEntity _entity;
        public UserInputComponent(PlayerEntity entity)
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
                        double x = (float)Cos(_entity.Rotation.Y + System.Math.PI / 2);
                        double z = (float)Sin(_entity.Rotation.Y + System.Math.PI / 2);
                        _entity.Move(-x, 0, -z);
                        break;
                    }

                case Key.S:
                    {
                        // Back
                        double x = (float)Cos(_entity.Rotation.Y + System.Math.PI / 2);
                        double z = (float)Sin(_entity.Rotation.Y + System.Math.PI / 2);
                        _entity.Move(x, 0, z);
                        break;
                    }

                case Key.A:
                    {
                        // Strafe left
                        double x = (float)Cos(_entity.Rotation.Y);
                        double z = (float)Sin(_entity.Rotation.Y);
                        _entity.Move(-x, 0, -z);
                        break;
                    }
                case Key.D:
                    {
                        // Strafe right
                        double x = (float)Cos(_entity.Rotation.Y);
                        double z = (float)Sin(_entity.Rotation.Y);
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
