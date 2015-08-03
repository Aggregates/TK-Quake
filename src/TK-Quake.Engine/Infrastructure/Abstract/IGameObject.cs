using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKQuake.Engine.Infrastructure.Abstract
{
    /// <summary>
    /// Defines the core mechanics for any game object used in an OpenTK game
    /// </summary>
    public interface IGameObject
    {
        /// <summary>
        /// Update the state of the object
        /// </summary>
        /// <param name="elapsedTime">The time between two frames being rendered</param>
         void Update(double elapsedTime);

        /// <summary>
        /// Renders the game object to the game window using OpenTK
        /// </summary>
         void Render();
    }
}
