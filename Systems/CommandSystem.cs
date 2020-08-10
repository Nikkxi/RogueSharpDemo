using RogueSharp.DiceNotation;
using RogueSharpDemo.Core;
using System.Text;

namespace RogueSharpDemo.Systems
{
    class CommandSystem
    {
        public bool MovePlayer(Direction direction)
        {
            int x = Game._Player.X;
            int y = Game._Player.Y;

            switch (direction)
            {
                case Direction.Up:
                    {
                        y = Game._Player.Y - 1;
                        break;
                    }
                case Direction.Down:
                    {
                        y = Game._Player.Y + 1;
                        break;
                    }
                case Direction.Left:
                    {
                        x = Game._Player.X - 1;
                        break;
                    }
                case Direction.Right:
                    {
                        x = Game._Player.X + 1;
                        break;
                    }
                default:
                    {
                        return false;
                    }
            }

            if(Game._Map.SetActorPosition(Game._Player, x, y))
            {
                return true;
            }

            Monster monster = Game._Map.GetMonsterAt(x, y);
            if (monster != null)
            {
                Attack(Game._Player, monster);
                return true;
            }

            return false;
        }

        public void Attack(Actor attacker, Actor defender)
        {
            StringBuilder attackMessage = new StringBuilder();
            StringBuilder defendMessage = new StringBuilder();

            int hits = ResolveAttack(attacker, defender, attackMessage);
            int blocks = ResolveBlock(defender, hits, attackMessage, defendMessage);

            Game._MessageLog.Add(attackMessage.ToString());
            if (!string.IsNullOrWhiteSpace(defendMessage.ToString()))
            {
                Game._MessageLog.Add(defendMessage.ToString());
            }

            int damage = hits - blocks;
            ResolveDamage(defender, damage);
        }

        private int ResolveAttack(Actor attacker, Actor defender, StringBuilder attackMessage)
        {
            int hits = 0;

            attackMessage.AppendFormat("{0} attacks {1} and rolls: ", attacker.Name, defender.Name);

            DiceExpression attackDice = new DiceExpression().Dice(attacker.Attack, 100);
            DiceResult attackResult = attackDice.Roll();

            foreach(TermResult result in attackResult.Results)
            {
                attackMessage.Append(result.Value + " ");
                if(result.Value >= 100 - attacker.AttackChance)
                {
                    hits++;
                }
            }

            return hits;
        }

        private int ResolveBlock(Actor defender, int hits, StringBuilder attackMessage, StringBuilder defendMessage)
        {
            int blocks = 0;

            if(hits > 0)
            {
                attackMessage.AppendFormat("scoring {0} hits.", hits);
                defendMessage.AppendFormat("  {0} defends and rolls: ", defender.Name);

                DiceExpression blockDice = new DiceExpression().Dice(defender.Defense, 100);
                DiceResult blockResult = blockDice.Roll();

                foreach (TermResult result in blockResult.Results)
                {
                    defendMessage.Append(result.Value + " ");
                    if (result.Value >= 100 - defender.DefenseChance)
                    {
                        blocks++;
                    }
                }
                defendMessage.AppendFormat("resulting in {0} blocks.", blocks);
            }
            else
            {
                attackMessage.Append(" and misses completely.");
            }

            return blocks;
        }

        private void ResolveDamage(Actor defender, int damage)
        {
            if(damage > 0)
            {
                defender.Health -= damage;

                Game._MessageLog.Add($"    {defender.Name} was hit for {damage} damage.");

                if(defender.Health <= 0)
                {
                    ResolveDeath(defender);
                }
            }
            else
            {
                Game._MessageLog.Add($"    {defender.Name} blocked all damage.");
            }
        }

        private void ResolveDeath(Actor corpse)
        {
            if(corpse is Player)
            {
                Game._MessageLog.Add($"{corpse.Name} has died.   GAME OVER.");
            }
            else if(corpse is Monster)
            {
                Game._Map.RemoveMonster((Monster)corpse);
                Game._MessageLog.Add($"{corpse.Name} had died, dropping {corpse.Gold} gold.");
                Game._Player.Gold += corpse.Gold;
            }
        }
    }
}
