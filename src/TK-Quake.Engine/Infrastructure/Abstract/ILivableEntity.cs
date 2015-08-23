using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Infrastructure.Math;

namespace TKQuake.Engine.Infrastructure.Abstract
{
    /// <summary>
    /// A livable entity that resides in the game world such as the player or NPC
    /// </summary>
    public interface ILivableEntity : IGameObject
    {

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

        double MoveSpeed { get; set; }
        double RotationSpeed { get; set; }

        void Move(Vector position);
        void Rotate(Vector rotation);

        /// <summary>
        /// Spanws the entity in the world with initial health
        /// </summary>
        void Spawn(double health = 100);

        /// <summary>
        /// Kills the entity and removes it from the world
        /// </summary>
        void Die();

        /// <summary>
        /// Inflicts damage on the entity and kills them if their health falls below zero
        /// </summary>
        /// <param name="damage">The amount of damage to inflict</param>
        void Hit(double damage);
    }
}
