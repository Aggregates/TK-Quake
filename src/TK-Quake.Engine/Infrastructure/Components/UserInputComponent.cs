using System;
using OpenTK.Input;
using TKQuake.Engine.Infrastructure.Abstract;
using TKQuake.Engine.Infrastructure.Entities;
using TKQuake.Engine.Infrastructure.Input;
using static System.Math;

namespace TKQuake.Engine.Infrastructure.Components
{
    public class UserInputComponent : IComponent
    {
        private readonly ILivableEntity _entity;
        public UserInputComponent(ILivableEntity entity)
        {
            _entity = entity;
        }

        public void Startup() { }
        public void Shutdown() { }
        public void Update(double elapsedTime) {
            var state = Keyboard.GetState();

            //hacky "run" modifier
            elapsedTime = state[Key.LShift] ? elapsedTime * 2 : elapsedTime;

            if (state[Key.W]) HandleInput(Key.W, elapsedTime);
            if (state[Key.A]) HandleInput(Key.A, elapsedTime);
            if (state[Key.S]) HandleInput(Key.S, elapsedTime);
            if (state[Key.D]) HandleInput(Key.D, elapsedTime);
            if (state[Key.Left]) HandleInput(Key.Left, elapsedTime);
            if (state[Key.Right]) HandleInput(Key.Right, elapsedTime);
            if (state[Key.Up]) HandleInput(Key.Up, elapsedTime);
            if (state[Key.Down]) HandleInput(Key.Down, elapsedTime);
        }

        public void HandleInput(Key k, double elapsedTime)
        {
            switch(k)
            {
                case Key.W:
                    {
                        // Forward
                        double x = (float)Cos(_entity.Rotation.Y + System.Math.PI / 2);
                        double z = (float)Sin(_entity.Rotation.Y + System.Math.PI / 2);

                        var to = new Math.Vector(-x, 0, -z);
                        var command = new MoveCommand(to, _entity.MoveSpeed * elapsedTime);
                        command.Execute(_entity);
                        break;
                    }

                case Key.S:
                    {
                        // Back
                        double x = (float)Cos(_entity.Rotation.Y + System.Math.PI / 2);
                        double z = (float)Sin(_entity.Rotation.Y + System.Math.PI / 2);

                        var to = new Math.Vector(x, 0, z);
                        var command = new MoveCommand(to, _entity.MoveSpeed * elapsedTime);
                        command.Execute(_entity);
                        break;
                    }

                case Key.A:
                    {
                        // Strafe left
                        double x = (float)Cos(_entity.Rotation.Y);
                        double z = (float)Sin(_entity.Rotation.Y);

                        var to = new Math.Vector(-x, 0, -z);
                        var command = new MoveCommand(to, _entity.MoveSpeed * elapsedTime);
                        command.Execute(_entity);
                        break;
                    }
                case Key.D:
                    {
                        // Strafe right
                        double x = (float)Cos(_entity.Rotation.Y);
                        double z = (float)Sin(_entity.Rotation.Y);

                        var to = new Math.Vector(x, 0, z);
                        var command = new MoveCommand(to, _entity.MoveSpeed * elapsedTime);
                        command.Execute(_entity);
                        break;
                    }
                case Key.Left:
                    {
                        var rotation = new Math.Vector(0, -1, 0);
                        var command = new RotateCommand(rotation, _entity.RotationSpeed * elapsedTime);
                        command.Execute(_entity);

                        break;
                    }
                case Key.Right:
                    {
                        var rotation = new Math.Vector(0, 1, 0);
                        var command = new RotateCommand(rotation, _entity.RotationSpeed * elapsedTime);
                        command.Execute(_entity);

                        break;
                    }
                case Key.Up:
                    {
                        var rotation = new Math.Vector(1, 0, 0);
                        var command = new RotateCommand(rotation, _entity.RotationSpeed * elapsedTime);
                        command.Execute(_entity);

                        break;
                    }
                case Key.Down:
                    {
                        var rotation = new Math.Vector(-1, 0, 0);
                        var command = new RotateCommand(rotation, _entity.RotationSpeed * elapsedTime);
                        command.Execute(_entity);

                        break;
                    }
            }
        }
    }
}
