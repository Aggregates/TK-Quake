using GameLoop.Engine.Infrastructure.Math;
using GameLoop.Engine.Infrastructure.Texture;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLoop.Engine.Infrastructure.Font
{
    public class Font
    {
        private const int _headerLines = 4;

        private Texture.Texture _texture;

        public Dictionary<char, CharacterData> Characters { get; private set; }
        public string FilePath { get; set; }

        public Font(Texture.Texture texture, string filePath)
        {
            this._texture = texture;
            this.FilePath = filePath;
            Characters = Parse(filePath);
        }

        public Font(Texture.Texture texture, Dictionary<char, CharacterData> characters)
        {
            this._texture = texture;
            this.Characters = characters;
        }

        public CharacterSprite CreateSprite(char c)
        {
            CharacterData data = Characters[c];
            Sprite2 sprite = new Sprite2(this._texture);

            // Setup UV Data
            // We need to divide by the texture Width since OpenGL has UVs in range
            // 0 to 1
            Point topLeft = new Point(
                (float)data.X / (float)_texture.Width,
                (float)data.Y / (float)_texture.Height);

            Point bottomRight = new Point(
                topLeft.X + ((float)data.Width / (float)_texture.Width),
                topLeft.Y + ((float)data.Height / (float)_texture.Height)
                );

            sprite.SetUVs(topLeft, bottomRight);
            sprite.RenderWidth = data.Width;
            sprite.RenderHeight = data.Height;
            sprite.SetColor(new Color(1, 1, 1, 1));

            return new CharacterSprite(sprite, data);
        }

        private Dictionary<char, CharacterData> Parse(string fileName)
        {
            Dictionary<char, CharacterData> characterDictionary = new Dictionary<char, CharacterData>();
            string[] lines = File.ReadAllLines(fileName);

            int charCount = GetParameterValue(lines[3].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[1]);

            for (int i = _headerLines; i < (charCount + _headerLines); i++)
            {
                string line = lines[i];
                string[] parameters = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                CharacterData charData = new CharacterData
                {
                    Id          = GetParameterValue(parameters[1]),
                    X           = GetParameterValue(parameters[2]),
                    Y           = GetParameterValue(parameters[3]),
                    Width       = GetParameterValue(parameters[4]),
                    Height      = GetParameterValue(parameters[5]),
                    XOffset     = GetParameterValue(parameters[6]),
                    YOffset     = GetParameterValue(parameters[7]),
                    XAdvance    = GetParameterValue(parameters[8])
                };

                characterDictionary.Add((char)charData.Id, charData);
            }

            return characterDictionary;
        }

        /// <summary>
        /// Gets the value of a parameter that occurs directly after an equals sign
        /// </summary>
        /// <param name="s">The string assignment to parse</param>
        /// <returns>A numeric value of the parameter that occurs to the right of the equality</returns>
        private int GetParameterValue(string s)
        {
            string value = s.Substring(s.IndexOf('=') + 1);
            return Int32.Parse(value);
        }

        
        public Vector MeasureFont(string text, double maxWidth = -1)
        {
            Vector dimensions = new Vector();
            foreach(char c in text)
            {
                CharacterData data = Characters[c];
                dimensions.X += data.XAdvance;
                dimensions.Y = System.Math.Max(dimensions.Y, data.Height + data.YOffset);
            }
            return dimensions;
        }
    }
}
