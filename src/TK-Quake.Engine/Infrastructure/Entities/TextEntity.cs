using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TKQuake.Engine.Core;
using TKQuake.Engine.Infrastructure.Abstract;
using TKQuake.Engine.Infrastructure.Math;
using TKQuake.Engine.Infrastructure.Texture;
using TKQuake.Engine.Infrastructure.Font;
using TKQuake.Engine.Infrastructure.Components;

namespace TKQuake.Engine.Infrastructure.Entities
{
    public class TextEntity : Entity
    {
        private Font.Font _font;
        private string _text;
        private Color _color;
        private double _maxWidth = -1;

        public Vector Dimensions { get; private set; }
        public double Width { get { return Dimensions.X; } }
        public double Height { get { return Dimensions.Y; } }

        public List<CharacterSprite> CharacterSprites { get; private set; }

        public TextEntity(string text, Font.Font font, double maxWidth = -1)
        {
            CharacterSprites = new List<CharacterSprite>();
            _text = text;
            _font = font;
            _maxWidth = maxWidth;
            _color = Color.Black;
            CreateText(0, 0, _maxWidth);

            Components.Add(new TextComponent(this));
        }

        private void CreateText(double x, double y, double maxWidth)
        {
            CharacterSprites.Clear();
            double currentX = 0;
            double currentY = 0;

            // Split text into words and calculate the width
            string[] words = _text.Split(' ');
            foreach(string word in words)
            {
                Vector nextWordLength = _font.MeasureFont(word);
                if (maxWidth != -1 && (currentX + nextWordLength.X) > maxWidth)
                {
                    currentX = 0; // This is correct
                    currentY += nextWordLength.Y;
                }

                string wordWithSpace = word + " ";

                foreach (char c in wordWithSpace)
                {
                    CharacterSprite sprite = _font.CreateSprite(c);

                    // Set up the parameters

                    // Sprites are positioned around their center, but the offset values are
                    // posititoned at the top-left. The following converts the offset to
                    // a central position by halving it
                    float xOffset = ((float)sprite.Data.XOffset) / 2;
                    float yOffset = (((float)sprite.Data.Height) * 0.5f) +
                        ((float) sprite.Data.YOffset);

                    sprite.Sprite.SetPosition(x + currentX + xOffset, y - currentY - yOffset);
                    currentX += sprite.Data.XAdvance;

                    // Add to the character sprites
                    CharacterSprites.Add(sprite);
                }

            }

            // Update the dimensions. Compiler won't let you simply set the Y value so
            // you have to set the whole variable
            Vector tempDimensions = _font.MeasureFont(this._text, this._maxWidth);
            this.Dimensions = maxWidth == -1 ? tempDimensions : new Vector(tempDimensions.X, currentY, tempDimensions.Z);

            // Set the color
            SetColor(this._color);

        }

        public void SetPosition(double x, double y)
        {
            CreateText(x, y, _maxWidth);
        }

        public void SetColor(Color color)
        {
            this._color = color;
            foreach (CharacterSprite s in CharacterSprites)
            {
                s.Sprite.SetColor(color);
            }
        }

        public void Render()
        {
        }
    }
}
