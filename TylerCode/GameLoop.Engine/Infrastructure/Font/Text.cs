﻿using GameLoop.Engine.Core;
using GameLoop.Engine.Infrastructure.Abstract;
using GameLoop.Engine.Infrastructure.Math;
using GameLoop.Engine.Infrastructure.Texture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLoop.Engine.Infrastructure.Font
{
    public class Text : IGameObject
    {
        private Font _font;
        private string _text;
        private Color _color;
        private double _maxWidth = -1;

        public Vector Dimensions { get; private set; }
        public double Width { get { return Dimensions.X; } }
        public double Height { get { return Dimensions.Y; } }

        public List<CharacterSprite> CharacterSprites { get; private set; }

        public Text(string text, Font font, double maxWidth = -1)
        {
            CharacterSprites = new List<CharacterSprite>();
            _text = text;
            _font = font;
            _maxWidth = maxWidth;
            CreateText(0, 0, _maxWidth);
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
            this.Dimensions = new Vector(tempDimensions.X, currentY, tempDimensions.Z);

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
        
        public void Update(double elapsedTime)  { }

        public void Render()
        {
            Renderer renderer = new Renderer();
            renderer.DrawText(this);
        }
    }
}
