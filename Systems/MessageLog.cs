using RLNET;
using System;
using System.Collections.Generic;
using System.Text;

namespace RogueSharpDemo.Systems
{
    class MessageLog
    {
        private static readonly int _MAXLINES = 9;
        private readonly Queue<string> _lines;

        public MessageLog()
        {
            _lines = new Queue<string>();
        }

        public void Add(string message)
        {
            _lines.Enqueue(message);

            if(_lines.Count > _MAXLINES)
            {
                _lines.Dequeue();
            }
        }

        public void Draw(RLConsole console)
        {
            string[] lines = _lines.ToArray();

            for(int i = 0; i < lines.Length; i++)
            {
                console.Print(1, i+1, lines[i], RLColor.White);
            }
        }
    }
}
