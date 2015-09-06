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
    public class CollisionDetector : CollisionEntity
    {
        private static CollisionDetector _instance;
        public static CollisionDetector Singleton()
        {
            return _instance ?? (_instance = new CollisionDetector());
        }

        // Instance variables
        public bool Active { get; set; }

        private List<CollisionComponent> _colliders;

        public CollisionDetector()
        {
            _colliders = new List<CollisionComponent>();
        }

        public void RegisterCollider(CollisionComponent collider)
        {
            _colliders.Add(collider);
        }

        public void RemoveCollider(CollisionComponent collider)
        {
            _colliders.Remove(collider);
        }


        public void DetectCollisions()
        {
            // Detect Bounding Box Collisions
            // Complexity if O(n^2). Try to improve this if possible
            var boxes = _colliders.OfType<BoundingBoxComponent>();
            foreach (var box in boxes)
            {
                foreach(var otherBox in boxes)
                {
                    if (box != otherBox && !box.Equals(otherBox))
                    {
                        box.CheckCollision(otherBox);
                    }
                }
            }

        }

        public override void Update(double elapsedTime)
        {
            if (Active)
                DetectCollisions();
        }
    }
}
