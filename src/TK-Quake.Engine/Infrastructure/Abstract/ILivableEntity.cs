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
        /// Creates the object in the game world by making it "live"
        /// </summary>
        void Spawn();

        /// <summary>
        /// Kills the object and removes it from the world
        /// </summary>
        void Die();

        void Hit(double damage);
    }
}
