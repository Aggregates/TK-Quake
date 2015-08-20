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
    public interface ILivableEntity
    {
        void Move(Vector position);

        /// <summary>
        /// Spanws the player in the world with initial health
        /// </summary>
        void Spawn(double health = 100);

        /// <summary>
        /// Kills the object and removes it from the world
        /// </summary>
        void Die();

        /// <summary>
        /// Inflicts damage on the player and kills them if their health falls below zero
        /// </summary>
        /// <param name="damage">The amount of damage to inflict</param>
        void Hit(double damage);
    }
}
