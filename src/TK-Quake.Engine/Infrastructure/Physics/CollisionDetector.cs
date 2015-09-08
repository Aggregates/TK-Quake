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
    public class CollisionDetector : Entity
    {
        private static CollisionDetector _instance;
        public static CollisionDetector Singleton()
        {
            return _instance ?? (_instance = new CollisionDetector { Active = true });
        }

        // Instance variables
        public bool Active { get; set; }

        private List<Entity> _colliders;

        public CollisionDetector()
        {
            _colliders = new List<Entity>();
        }

        public void RegisterCollider(Entity collider)
        {
            _colliders.Add(collider);
        }

        public void RemoveCollider(Entity collider)
        {
            _colliders.Remove(collider);
        }


        public void DetectCollisions()
        {
            // Detect Bounding Box Collisions
            // Complexity if O(n^2). Try to improve this if possible
            var boxes = _colliders.OfType<BoundingBoxEntity>();
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
