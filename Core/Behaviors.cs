using RogueSharp;
using RogueSharpDemo.Interfaces;
using RogueSharpDemo.Systems;

namespace RogueSharpDemo.Core
{
    public class StandardMoveAndAttack : IBehavior
    {
        public bool Act(Monster monster, CommandSystem commandSystem)
        {
            DungeonMap map = Game._Map;
            Player player = Game._Player;
            FieldOfView monsterFoV = new FieldOfView(map);

            if (!monster.TurnsAlerted.HasValue)
            {
                monsterFoV.ComputeFov(monster.X, monster.Y, monster.Awareness, true);
                if(monsterFoV.IsInFov(player.X, player.Y))
                {
                    Game._MessageLog.Add($"{monster.Name} is eager to fight {player.Name}");
                    monster.TurnsAlerted = 1;
                }
            }

            if (monster.TurnsAlerted.HasValue)
            {
                map.SetIsWalkable(monster.X, monster.Y, true);
                map.SetIsWalkable(player.X, player.Y, true);

                PathFinder pathFinder = new PathFinder(map);
                Path path = null;

                try
                {
                    path = pathFinder.ShortestPath(map.GetCell(monster.X, monster.Y), map.GetCell(player.X, player.Y));
                }
                catch (PathNotFoundException)
                {
                    Game._MessageLog.Add($"{monster.Name} waits for a turn.");
                }

                map.SetIsWalkable(monster.X, monster.Y, false);
                map.SetIsWalkable(player.X, player.Y, false);

                if(path != null)
                {
                    try
                    {
                        commandSystem.MoveMonster(monster, path.StepForward());
                    }
                    catch (NoMoreStepsException)
                    {
                        Game._MessageLog.Add($"{monster.Name} growls in frustration.");
                    }
                }

                monster.TurnsAlerted++;

                if(monster.TurnsAlerted >= 15)
                {
                    monster.TurnsAlerted = null;
                }
            }
            return true;
        }
    }
}
