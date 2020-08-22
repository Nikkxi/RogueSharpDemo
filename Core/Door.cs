using RLNET;
using RogueSharp;
using RogueSharpDemo.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RogueSharpDemo.Core
{
    public class Door : IDrawable
    {

        public RLColor _Color { get; set; }
        public RLColor _BackgroundColor { get; set; }
        public char _Symbol { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsOpen { get; set; }

        public Door()
        {
            _Symbol = '+';
            _Color = Colors.Door;
            _BackgroundColor = Colors.DoorBackground;
        }

        public void Draw(RLConsole console, IMap map)
        {
            if (!map.GetCell(X, Y).IsExplored)
            {
                return;
            }

            _Symbol = IsOpen ? '-' : '+';

            if (map.IsInFov(X, Y))
            {
                _Color = Colors.DoorFoV;
                _BackgroundColor = Colors.DoorBackgroundFoV;
            }
            else
            {
                _Color = Colors.Door;
                _BackgroundColor = Colors.DoorBackground;
            }

            console.Set(X, Y, _Color, _BackgroundColor, _Symbol);
        }
    }
}
