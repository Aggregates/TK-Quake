using System;
using System.Collections;
using System.Collections.Generic;
using TKQuake.Engine.Infrastructure.Math;
using TKQuake.Engine.Infrastructure.Components;
using System.Threading.Tasks;
using OpenTK;

namespace TKQuake.Engine.Infrastructure.Entities
{
    public class Entity : IEntity
    {
        public string Id { get; set; }
        public string TextureId { get; set; }

        public float Scale { get; set; } = 1;
        public IList<IComponent> Components { get; set; } = new List<IComponent>();
        public IList<IEntity> Children { get; set; } = new List<IEntity>();
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 ViewDirection { get; }
        public Vector3 OrthogonalDirection { get; }
        public Matrix4 Translation { get; set; }
        public Matrix4 Transform { get; set; }

        public event EventHandler Destroy;
        private List<IEntity> _removeEntities = new List<IEntity>(); 

        public Entity()
        {
            Translation = Matrix4.Identity;
        }

        public virtual void Update(double elapsedTime) {
            // Remove all components and children marked for deletion
            _removeEntities.ForEach(e => Children.Remove(e));
            _removeEntities.Clear();

            //update all components
            foreach (var component in Components)
            {
                component.Update(elapsedTime);
            }

            //update all children
            foreach (var entity in Children)
            {
                entity.Update(elapsedTime);
            }

            // Update the transform
            // Remember, this is done LEFT TO RIGHT!
            var rotation = Matrix4.CreateRotationX(Rotation.X) *
                           Matrix4.CreateRotationY(Rotation.Y) *
                           Matrix4.CreateRotationZ(Rotation.Z);
            this.Transform = Matrix4.CreateScale(Scale) * rotation * Translation * Matrix4.CreateTranslation(Position);
        }

        /// <summary>
        /// Destroys this entity
        /// </summary>
        public void DestroyEntity()
        {
            OnDestroy(null);
        }

        /// <summary>
        /// Handler for when an Entity is destroyed
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDestroy(EventArgs e)
        {
            Destroy?.Invoke(this, e);
        }

        /// <summary>
        /// Adds the entity to a queue to be removed on the next update cycle.
        /// The entity must exist in this entity's Children
        /// </summary>
        /// <param name="entity"></param>
        public void RemoveEntity(IEntity entity)
        {
            System.Diagnostics.Debug.Assert(Children.Contains(entity), "Entity is not a direct child of this entity");
            _removeEntities.Add(entity);
        }
    }
}
