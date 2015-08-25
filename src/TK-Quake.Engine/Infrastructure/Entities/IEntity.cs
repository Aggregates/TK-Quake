using System;
using System.Collections;
using System.Collections.Generic;
using TKQuake.Engine.Infrastructure.Math;
using TKQuake.Engine.Infrastructure.Components;

namespace TKQuake.Engine.Infrastructure.Entities
{
    public interface IEntity
    {
        IList<IComponent> Components { get; set; }
        IList<IEntity> Children { get; set; }

        /// <summary>
        /// The current position of the entity in 3d space.
        /// </summary>
        Vector Position { get; set; }

        /// <summary>
        /// The angle (in radians) the entity is facing by rotating around the
        /// each of the axis. In OpenGL, the Y-axis is up
        /// </summary>
        Vector Rotation { get; set; }

        /// <summary>
        /// Unit Vector from the Position to any point in space, one unit away,
        /// that defines where the Camera is looking
        /// </summary>
        Vector ViewDirection { get; }

        /// <summary>
        /// The unit Vector orthogonal to the view direction
        /// </summary>
        Vector OrthogonalDirection { get; }

        void Update(double elapsedTime);
    }
}
