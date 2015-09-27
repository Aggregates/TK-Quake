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
        }


        public void DestroyEntity()
        {
            OnDestroy(null);
        }

        protected virtual void OnDestroy(EventArgs e)
        {
            Destroy?.Invoke(this, e);
        }

        public void RemoveEntity(IEntity entity)
        {
            System.Diagnostics.Debug.Assert(Children.Contains(entity), "Entity is not a direct child of this entity");
            _removeEntities.Add(entity);
        }
    }
}
