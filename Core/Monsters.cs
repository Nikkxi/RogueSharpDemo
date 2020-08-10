using RLNET;
using RogueSharp.DiceNotation;
using System;
using System.Collections.Generic;
using System.Text;

namespace RogueSharpDemo.Core
{
    public class Monster : Actor
    {
        public void DrawMonsterStats(RLConsole console, int pos)
        {
            int yPos = 13 + (pos * 2);

            console.Print(1, yPos, _Symbol.ToString(), Colors.Text);

            int width = Convert.ToInt32( ( (double)Health / (double)MaxHealth ) * 16.0);
            int remainingWidth = 16 - width;

            console.SetBackColor(3, yPos, width, 1, Palette.Primary);
            console.SetBackColor(3 + width, yPos, remainingWidth, 1, Palette.PrimaryDarkest);

            console.Print(2, yPos, $": {Name}", Palette.DbLight);
        }
    }

    public class Kobold : Monster
    {
        public static Kobold Create(int level)
        {
            int _Health = Dice.Roll("2D5");

            return new Kobold()
            {
                Attack = Dice.Roll("1D3") + level / 3,
                AttackChance = Dice.Roll("25D3"),
                Awareness = 10,
                _Color = Colors.Kobold,
                Defense = Dice.Roll("1D3") + level / 3,
                DefenseChance = Dice.Roll("10D4"),
                Gold = Dice.Roll("5D5"),
                Health = _Health,
                MaxHealth = _Health,
                Name = "Kobold",
                Speed = 14,
                _Symbol = 'k'
            };
        }
    }
}
