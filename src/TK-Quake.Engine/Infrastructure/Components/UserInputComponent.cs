using System;
using System.CodeDom;
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
        public float MouseSensitivity { get; set; } = 5f;
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

            HandleMouseInput(elapsedTime);
        }

        private void HandleMouseInput(double elapsedTime)
        {
            var mouseState = Mouse.GetCursorState();

            var xPos = (float)mouseState.X;
            var yPos = (float)mouseState.Y;

            if (_lastMouseState.X == mouseState.X && _lastMouseState.Y == mouseState.Y)
                return;

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

            var pitch = MathHelper.DegreesToRadians(_pitch);
            var yaw = MathHelper.DegreesToRadians(_yaw);

            var command = CommandFactory.Create(typeof (MouseLookCommand), new Vector3(yaw, pitch, 0), MouseSensitivity * elapsedTime);
            CommandCentre.PushCommand(command, _entity);
        }

        public void HandleInput(Key k, double elapsedTime)
        {
            ICommand command = null;
            Func<Vector3, Vector3> moveVector = (v) => v*(Vector3.UnitX+Vector3.UnitZ);
            var moveSpeed = _entity.MoveSpeed*elapsedTime;

            Action<Vector3> move =
                (v) => command = CommandFactory.Create(typeof (MoveCommand), moveVector(v), moveSpeed);
            Action<Vector3> rotate =
                (v) => command = CommandFactory.Create(typeof (RotateCommand), v, _entity.RotationSpeed*elapsedTime);

            switch(k)
            {
                case Key.W:
                    {
                        // Forward
                        move(_entity.ViewDirection);
                        break;
                    }

                case Key.S:
                    {
                        // Back
                        move(-_entity.ViewDirection);
                        break;
                    }

                case Key.A:
                    {
                        // Strafe left
                        move(-Vector3.Normalize(Vector3.Cross(_entity.ViewDirection, Vector3.UnitY)));
                        break;
                    }
                case Key.D:
                    {
                        // Strafe right
                        move(Vector3.Normalize(Vector3.Cross(_entity.ViewDirection, Vector3.UnitY)));
                        break;
                    }
                case Key.Left:
                    {
                        rotate(new Vector3(0, -1, 0));
                        break;
                    }
                case Key.Right:
                    {
                        rotate(new Vector3(0, 1, 0));
                        break;
                    }
                case Key.Up:
                    {
                        rotate(new Vector3(1, 0, 0));
                        break;
                    }
                case Key.Down:
                    {
                        rotate(new Vector3(-1, 0, 0));
                        break;
                    }
            }

            if (command != null)
                CommandCentre.PushCommand(command, _entity);
        }
    }
}
