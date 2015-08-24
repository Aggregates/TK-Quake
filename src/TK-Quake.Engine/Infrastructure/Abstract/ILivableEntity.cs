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
    public interface ILivableEntity : IEntity
    {
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
