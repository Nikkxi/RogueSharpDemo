using RLNET;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace RogueSharpDemo.Interfaces
{
    public interface IDrawable
    {
        RLColor _Color { get; set; }
        char _Symbol { get; set; }
        int X { get; set; }
        int Y { get; set; }

        public abstract void Draw(RLConsole console, IMap map);
    }
}
