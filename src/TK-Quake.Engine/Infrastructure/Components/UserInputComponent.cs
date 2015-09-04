using System;
using System.Resources;
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
        public float MouseSensitivity { get; set; } = 0.05f;
        private MouseState _lastMouseState;

        private float _yaw = -90,
            _pitch;
        private bool firstMouse = true;

        private readonly ILivableEntity _entity;
        public UserInputComponent(ILivableEntity entity)
        {
            _entity = entity;
            _lastMouseState = new MouseState();
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

            HandleMouseInput();
        }

        private void HandleMouseInput()
        {
            var mouseState = Mouse.GetState();

            var xPos = (float)mouseState.X;
            var yPos = (float)mouseState.Y;

            float lastX, lastY;
            if (firstMouse)
            {
                lastX = xPos;
                lastY = yPos;
                firstMouse = false;
            }
            else
            {
                lastX = _lastMouseState.X;
                lastY = _lastMouseState.Y;
            }

            var yOffset = xPos - lastX;
            var xOffset = lastY - yPos;
            _lastMouseState = mouseState;

            xOffset *= MouseSensitivity;
            yOffset *= MouseSensitivity;

            _yaw += xOffset;
            _pitch += yOffset;

            _pitch = _pitch > 89f ? 89f : _pitch < -89f ? -89f : _pitch;

            var pitch = MathHelper.DegreesToRadians(_pitch);
            var yaw = MathHelper.DegreesToRadians(_yaw);
            var sinPitch = (float) Sin(pitch);
            var cosPitch = (float) Cos(pitch);
            var sinYaw = (float) Sin(yaw);
            var cosYaw = (float) Cos(yaw);

            var front = new Vector3(cosPitch*cosYaw, sinPitch, cosPitch*sinYaw);
            _entity.Rotation = Vector3.Normalize(front);
        }

        public void HandleInput(Key k, double elapsedTime)
        {
            ICommand command = null;
            switch(k)
            {
                case Key.W:
                    {
                        // Forward
                        command = CommandFactory.Create(typeof(MoveCommand),
                            _entity.ViewDirection, _entity.MoveSpeed * elapsedTime);
                        break;
                    }

                case Key.S:
                    {
                        // Back
                        command = CommandFactory.Create(typeof(MoveCommand),
                            -_entity.ViewDirection, _entity.MoveSpeed * elapsedTime);
                        break;
                    }

                case Key.A:
                    {
                        // Strafe left
                        var to = Vector3.Normalize(Vector3.Cross(_entity.ViewDirection, Vector3.UnitY));
                        command = CommandFactory.Create(typeof(MoveCommand),
                            -to, _entity.MoveSpeed * elapsedTime);
                        break;
                    }
                case Key.D:
                    {
                        // Strafe right
                        var to = Vector3.Normalize(Vector3.Cross(_entity.ViewDirection, Vector3.UnitY));
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
                case Key.F10:
                    {
                        Console.WriteLine("World Change");
                        //command 
                        break;
                    }
            }

            if (command != null)
                CommandCentre.PushCommand(command, _entity);
        }
    }
}
