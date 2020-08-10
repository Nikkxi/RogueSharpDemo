using RLNET;

namespace RogueSharpDemo.Core
{
    class Player : Actor
    {
        public Player()
        {
            _Color = Colors.Player;
            _Symbol = '@';
            X = 10;
            Y = 10;

            Attack = 2;
            AttackChance = 50;
            Awareness = 15;
            Defense = 2;
            DefenseChance = 40;
            Gold = 0;
            Health = 100;
            MaxHealth = 100;
            Name = "Rogue";
            Speed = 10;

        }

        public void DrawStats(RLConsole console)
        {
            console.Print(1, 1, $"Name:    {Name}", Colors.Text);
            console.Print(1, 3, $"Health:  {Health}/{MaxHealth}", Colors.Text);
            console.Print(1, 5, $"Attack:  {Attack}", Colors.Text);
            console.Print(1, 7, $"Defense: {Defense}", Colors.Text);
            console.Print(1, 9, $"Gold:    {Gold}", Colors.Gold);
        }
    }
}
