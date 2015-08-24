using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
//using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Infrastructure.GameScreen;

namespace TKQuake.ScreenStates
{
    public class TitleScreen : GameScreen
    {
        public static new string StateNameKey = "TitleScreen";

        //private double _rotation = 0;

        public TitleScreen(ScreenManager manager)
        {
            base._screenManager = manager;
        }

        /*
        public override void Update(double elapsedTime)
        {
            _rotation = 10 * elapsedTime;
        }

        public override void Render()
        {
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            /* Example 1) Clear all buffers and set to red
             GL.ClearColor(Color.Red);
             */

            /* Example 2) Drawing points
            GL.Color3(Color.Black);
            GL.PointSize(5f);
            GL.Begin(PrimitiveType.Triangles);
            GL.Vertex3(100, 100, 0);
            GL.Vertex3(50, 150, 0);
            GL.Vertex3(150, 150, 0);
            GL.End();

            GL.ClearColor(System.Drawing.Color.Black);

            //GL.PointSize(5.0f);
            GL.Rotate(_rotation, new Vector3d(0, 1, 0));

            GL.Begin(PrimitiveType.TriangleStrip);
            {
                GL.Color3(System.Drawing.Color.Red);
                GL.Vertex3(-50, 0, 0);
                GL.Color3(System.Drawing.Color.Green);
                GL.Vertex3(0, 50, 0);
                GL.Color3(System.Drawing.Color.Blue);
                GL.Vertex3(50, 0, 0);
            }
            GL.End();

            // GL.Finish();
        }
        */
    }
}
