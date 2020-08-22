using RLNET;
using RogueSharp;
using RogueSharpDemo.Interfaces;

namespace RogueSharpDemo.Core
{
    public class Actor : IActor, IDrawable, ISchedulable
    {
        public RLColor _Color { get; set; }
        public char _Symbol { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Time
        {
            get
            {
                return Speed;
            }
            
        }

        private int _Attack;
        private int _AttackChance;
        private int _Awareness;
        private int _Defense;
        private int _DefenseChance;
        private int _Gold;
        private int _Health;
        private int _MaxHealth;
        private string _Name;
        private int _Speed;


        public int Attack
        { 
            get { return _Attack; } 
            set { _Attack = value; } 
        }
        public int AttackChance
        {
            get { return _AttackChance; }
            set { _AttackChance = value; }
        }
        public int Awareness
        {
            get { return _Awareness; }
            set { _Awareness = value; }
        }
        public int Defense
        {
            get { return _Defense; }
            set { _Defense = value; }
        }
        public int DefenseChance
        {
            get { return _DefenseChance; }
            set { _DefenseChance = value; }
        }
        public int Gold
        {
            get { return _Gold; }
            set { _Gold = value; }
        }
        public int Health
        {
            get { return _Health; }
            set { _Health = value; }
        }
        public int MaxHealth
        {
            get { return _MaxHealth; }
            set { _MaxHealth = value; }
        }
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public int Speed
        {
            get { return _Speed; }
            set { _Speed = value; }
        }


        public void Draw(RLConsole console, IMap map)
        {
            if (!map.GetCell(X, Y).IsExplored)
            {
                return;
            }

            if (map.IsInFov(X, Y))
            {
                console.Set(X, Y, _Color, Colors.FloorBackgroundFov, _Symbol);
            }

        }
    }
}
