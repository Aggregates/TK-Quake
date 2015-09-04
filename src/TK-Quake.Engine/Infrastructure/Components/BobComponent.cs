using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenTK.Platform.Windows;
using TKQuake.Engine.Infrastructure.Entities;
using static System.Math;

namespace TKQuake.Engine.Infrastructure.Components
{
    public class BobComponent : IComponent
    {
        private readonly IEntity _entity;
        private double _value;

        public double Speed { get; set; }
        public double Scale { get; set; }

        public BobComponent(IEntity entity, double speed, double scale)
        {
            _entity = entity;
            Speed = speed;
            Scale = scale;
            _value = 0;
        }

        public void Startup() { }
        public void Shutdown() { }

        public void Update(double elapsedTime)
        {
            //the new angle in radians
            const double mod = 2*System.Math.PI;
            _value = (_value + elapsedTime * Speed) %mod;

            //get trig function value
            var trigValue = Scale * Sin(_value);
            var yValue = (float)(trigValue);

            var v = new Vector3(0, yValue, 0);
            var m = Matrix4.CreateTranslation(v);
            _entity.Translation += m;
        }
    }
}
