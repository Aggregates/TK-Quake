using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using TKQuake.Engine.Infrastructure.Components;
using TKQuake.Engine.Infrastructure.Entities;

namespace TKQuake.Engine.Infrastructure.Physics
{
    /// <summary>
    /// Detects all collisions between registered collision volumes.
    /// Uses a singleton to define collision for the entire game
    /// </summary>
    public class CollisionDetector : Entity
    {

        // TODO: Replace CollisionDetector singleton with an instance per GameScreen...
        // Has not been done yet due to hidden components/entities. Once all Module 1
        // features have been merged in, these components need to be openned to allow
        // for instance-use of the CollisionDetector

        // Instance variables
        private readonly List<Entity> _colliders;
        private static CollisionDetector _instance;

        private CollisionDetector()
        {
            _colliders = new List<Entity>();
        }
        
        /// <summary>
        /// Returns an singleton instance of the Collision Detector
        /// </summary>
        /// <returns></returns>
        public static CollisionDetector Singleton()
        {
            return _instance ?? (_instance = new CollisionDetector());
        }

        /// <summary>
        /// Adds a collision entity to the list of colliders maintained by the Collision Detector
        /// </summary>
        /// <param name="collider">The new collider to track in the collision detection routine</param>
        public void RegisterCollider(Entity collider)
        {
            _colliders.Add(collider);
        }

        /// <summary>
        /// Removes a collision entity from the Collision Detector and stops
        /// tracing during the collision detection routine
        /// </summary>
        /// <param name="collider">The entity to stop tracking in the collision detection routine</param>
        public void RemoveCollider(Entity collider)
        {
            _colliders.Remove(collider);
        }

        /// <summary>
        /// Updates the Collision Detector and runs the collision detection routine
        /// </summary>
        /// <param name="elapsedTime"></param>
        public override void Update(double elapsedTime)
        {
            DetectCollisions();
        }

        /// <summary>
        /// Attempts to check all collisions of entities in the world
        /// </summary>
        private void DetectCollisions()
        {
            // Detect Bounding Box Collisions
            // Complexity if O(n^2). Try to improve this if possible
            var boxes = _colliders.OfType<BoundingBoxEntity>();
            foreach (var box in boxes)
            {
                foreach (var otherBox in boxes)
                {
                    if (box != otherBox && !box.Equals(otherBox))
                    {
                        box.CheckCollision(otherBox);
                    }
                }
            }

        }
    }
}
