using GameLoop.Engine.Infrastructure.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLoop.Engine.Infrastructure.Font
{
    public class FontManager : ResourceManager<Font>
    {
        public FontManager() : base()
        {
        }

        public override void Add(string key, Font data)
        {
            base.Add(key, data);
        }

        public override Font Get(string key)
        {
            return base.Get(key);
        }

        public override void Remove(string key)
        {
            base.Remove(key);
        }

        public override bool Registered(string key)
        {
            return base.Registered(key);
        }

    }
}
