using System;
using OpenTK;
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
            ICommand command = null;
            switch(k)
            {
                case Key.W:
                    {
                        // Forward
                        var x = (float)Cos(_entity.Rotation.Y + System.Math.PI / 2);
                        var z = (float)Sin(_entity.Rotation.Y + System.Math.PI / 2);

                        var to = new Vector3(-x, 0, -z);
                        command = CommandFactory.Create(typeof(MoveCommand),
                            to, _entity.MoveSpeed * elapsedTime);
                        break;
                    }

                case Key.S:
                    {
                        // Back
                        var x = (float)Cos(_entity.Rotation.Y + System.Math.PI / 2);
                        var z = (float)Sin(_entity.Rotation.Y + System.Math.PI / 2);

                        var to = new Vector3(x, 0, z);
                        command = CommandFactory.Create(typeof(MoveCommand),
                            to, _entity.MoveSpeed * elapsedTime);
                        break;
                    }

                case Key.A:
                    {
                        // Strafe left
                        var x = (float)Cos(_entity.Rotation.Y);
                        var z = (float)Sin(_entity.Rotation.Y);

                        var to = new Vector3(-x, 0, -z);
                        command = CommandFactory.Create(typeof(MoveCommand),
                            to, _entity.MoveSpeed * elapsedTime);
                        break;
                    }
                case Key.D:
                    {
                        // Strafe right
                        var x = (float)Cos(_entity.Rotation.Y);
                        var z = (float)Sin(_entity.Rotation.Y);

                        var to = new Vector3(x, 0, z);
                        command = CommandFactory.Create(typeof(MoveCommand),
                            to, _entity.MoveSpeed * elapsedTime);
                        break;
                    }
                case Key.Left:
                    {
                        var rotation = new Vector3(0, -1, 0);
                        command = CommandFactory.Create(typeof(RotateCommand),
                            rotation, _entity.RotationSpeed * elapsedTime);
                        break;
                    }
                case Key.Right:
                    {
                        var rotation = new Vector3(0, 1, 0);
                        command = CommandFactory.Create(typeof(RotateCommand),
                            rotation, _entity.RotationSpeed * elapsedTime);

                        break;
                    }
                case Key.Up:
                    {
                        var rotation = new Vector3(1, 0, 0);
                        command = CommandFactory.Create(typeof(RotateCommand),
                            rotation, _entity.RotationSpeed * elapsedTime);

                        break;
                    }
                case Key.Down:
                    {
                        var rotation = new Vector3(-1, 0, 0);
                        command = CommandFactory.Create(typeof(RotateCommand),
                            rotation, _entity.RotationSpeed * elapsedTime);

                        break;
                    }
            }

            if (command != null)
                CommandCentre.PushCommand(command, _entity);
        }
    }
}
