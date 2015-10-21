using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKQuake.Engine.Infrastructure.Abstract
{
    public abstract class ResourceManager<T>
    {
        protected Dictionary<string, T> Database;
        
        public ResourceManager()
        {
            this.Database = new Dictionary<string, T>();
        }

        public virtual bool Registered(string key)
        {
            return Database.ContainsKey(key); 
        }

        public virtual T Get(string key)
        {
            return Database[key];
        }
        
        public virtual void Add(string key, T data)
        {
            // Check that state has not already been added
            if (Registered(key))
                throw new Exception(string.Format("Key '{0}' has already been registered with the Resource Manager", key));
            else
                Database.Add(key, data);
        }

        public virtual void Remove(string key)
        {
            if (!Registered(key))
                throw new Exception(string.Format("Key '{0}' has not been registered with the Resource Manager", key));
            else
                Database.Remove(key);
        }
    }
}
