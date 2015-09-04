using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
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
            var mod = 2*System.Math.PI;
            _value = (_value + (Speed * elapsedTime))%mod;

            //get trig function value
            var trigValue = Sin(_value);
            var scaledValue = trigValue*Scale;
            var yValue = (float)(scaledValue);

            Console.Clear();
            Console.WriteLine(@"Angle: {0}", _value);
            Console.WriteLine(@"Y: {0}", yValue);

            var v = new Vector3(0, yValue, 0);
            _entity.Position += v;
        }
    }
}
