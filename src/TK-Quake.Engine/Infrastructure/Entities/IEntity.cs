using System;
using System.Collections;
using System.Collections.Generic;
using OpenTK;
using TKQuake.Engine.Infrastructure.Math;
using TKQuake.Engine.Infrastructure.Components;

namespace TKQuake.Engine.Infrastructure.Entities
{
    public interface IEntity
    {
        string Id { get; set; }
        float Scale { get; set; }
        IList<IComponent> Components { get; set; }
        IList<IEntity> Children { get; set; }

        /// <summary>
        /// The current position of the entity in 3d space.
        /// </summary>
        Vector3 Position { get; set; }

        /// <summary>
        /// The angle (in radians) the entity is facing by rotating around the
        /// each of the axis. In OpenGL, the Y-axis is up
        /// </summary>
        Vector3 Rotation { get; set; }

        /// <summary>
        /// Unit Position from the Position to any point in space, one unit away,
        /// that defines where the Camera is looking
        /// </summary>
        Vector3 ViewDirection { get; }

        /// <summary>
        /// The unit Position orthogonal to the view direction
        /// </summary>
        Vector3 OrthogonalDirection { get; }

        Matrix4 Translation { get; set; }
        Matrix4 Transform { get; set; }

        void Update(double elapsedTime);
    }
}
