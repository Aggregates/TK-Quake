using GameLoop.Engine.Infrastructure.Texture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLoop.Engine.Infrastructure.Font
{
    public class CharacterSprite
    {
        public Sprite2 Sprite { get; set; }
        public CharacterData Data { get; set; }

        public CharacterSprite(Sprite2 sprite, CharacterData data)
        {
            this.Sprite = sprite;
            this.Data = data;
        }
    }
}
