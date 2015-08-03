using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLoop.Engine.Infrastructure.Font
{
    public class CharacterData
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        /// <summary>
        /// Used to align characters in relation to others
        /// </summary>
        public int XOffset { get; set; }

        /// <summary>
        /// Used to align characters in relation to others
        /// </summary>
        public int YOffset { get; set; }
        
        /// <summary>
        /// Represents the amount to advance on the x-axis once the current
        /// character has been rendered
        /// </summary>
        public int XAdvance { get; set; }

        /// <summary>
        /// Defines which "page" of the texture set the character is located
        /// on, if multiple pages are used to define a font
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Used for compression where different characters are written
        /// to different color channels
        /// </summary>
        public int Channel { get; set; }
    }
}
