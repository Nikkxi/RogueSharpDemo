using RLNET;
using RogueSharp;
using System.Collections.Generic;
using System.Linq;

namespace RogueSharpDemo.Core
{
    class DungeonMap : Map
    {

        public List<Rectangle> rooms;
        public List<Door> doors { get; set; }

        private readonly List<Monster> _monsters;

        public DungeonMap()
        {
            rooms = new List<Rectangle>();
            doors = new List<Door>();
            _monsters = new List<Monster>();
        }
        
        public void Draw(RLConsole MapConsole, RLConsole StatConsole)
        {
            foreach(Cell Cx in GetAllCells())
            {
                SetConsoleSymbolForCell(MapConsole, Cx);
            }

            foreach(Door d in doors)
            {
                d.Draw(MapConsole, this);
            }

            int i = 0;
            foreach(Monster m in _monsters)
            {
                m.Draw(MapConsole, this);
             
                if (IsInFov(m.X, m.Y))
                {
                    m.DrawMonsterStats(StatConsole, i);
                    i++;
                }
                
            }
        }

        private void SetConsoleSymbolForCell(RLConsole MapConsole, Cell Cx)
        {
            if (!Cx.IsExplored)
            {
                return;
            }

            if (IsInFov(Cx.X, Cx.Y))
            {
                if (Cx.IsWalkable)
                {
                    MapConsole.Set(Cx.X, Cx.Y, Colors.FloorFov, Colors.FloorBackgroundFov, '.');
                }
                else
                {
                    MapConsole.Set(Cx.X, Cx.Y, Colors.WallFov, Colors.WallBackgroundFov, '#');
                }
            }
            else
            {
                if (Cx.IsWalkable)
                {
                    MapConsole.Set(Cx.X, Cx.Y, Colors.Floor, Colors.FloorBackground, '.');
                }
                else
                {
                    MapConsole.Set(Cx.X, Cx.Y, Colors.Wall, Colors.WallBackground, '#');
                }
            }
        }

        public void UpdatePlayerFieldOfView()
        {
            Player player = Game._Player;

            ComputeFov(player.X, player.Y, player.Awareness, true);

            foreach(Cell _Cell in GetAllCells())
            {
                if (IsInFov(_Cell.X, _Cell.Y))
                {
                    SetCellProperties(_Cell.X, _Cell.Y, _Cell.IsTransparent, _Cell.IsWalkable, true);
                }
            }
        }

        public bool SetActorPosition(Actor actor, int x, int y)
        {
            if (GetCell(x, y).IsWalkable)
            {
                SetIsWalkable(actor.X, actor.Y, true);
                OpenDoor(actor, x, y);

                actor.X = x;
                actor.Y = y;

                SetIsWalkable(actor.X, actor.Y, false);

                if(actor is Player)
                {
                    UpdatePlayerFieldOfView();
                }
                return true;
            }
            return false;
        }

        public void SetIsWalkable(int x, int y, bool isWalkable)
        {
            ICell cell = GetCell(x, y);
            SetCellProperties(cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored);
        }

        public void AddPlayer(Player player)
        {
            Game._Player = player;
            SetIsWalkable(player.X, player.Y, false);
            UpdatePlayerFieldOfView();
            Game._Scheduler.Add(player);
        }

        public void AddMonster(Monster monster)
        {
            _monsters.Add(monster);
            SetIsWalkable(monster.X, monster.Y, false);
            Game._Scheduler.Add(monster);
        }

        public void RemoveMonster(Monster monster)
        {
            _monsters.Remove(monster);
            SetIsWalkable(monster.X, monster.Y, true);
            Game._Scheduler.Remove(monster);
        }
        public Monster GetMonsterAt(int x, int y)
        {
            return _monsters.FirstOrDefault(m => m.X == x && m.Y == y);
        }

        public Point GetRandomWalkableLocationInRoom(Rectangle room)
        {
            if (DoesRoomHaveWalkableSpace(room))
            {
                for (int i = 0; i < 100; i++)
                {
                    int x = Game.Random.Next(1, room.Width - 2) + room.X;
                    int y = Game.Random.Next(1, room.Height - 2) + room.Y;
                    if (IsWalkable(x, y))
                    {
                        return new Point(x, y);
                    }
                }
            }
            return default;
        }

        public bool DoesRoomHaveWalkableSpace(Rectangle room)
        {
            for(int x = 1; x <= room.Width-2; x++)
            {
                for(int y = 1; y <= room.Height-2; y++)
                {
                    if (IsWalkable(x + room.X, y + room.Y))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public Door GetDoor(int x, int y)
        {
            return doors.SingleOrDefault(d => d.X == x && d.Y == y);
        }

        private void OpenDoor(Actor actor, int x, int y)
        {
            Door door = GetDoor(x, y);
            if(door!=null && !door.IsOpen)
            {
                door.IsOpen = true;
                var cell = GetCell(x, y);
                SetCellProperties(x, y, true, cell.IsWalkable, cell.IsExplored);
                Game._MessageLog.Add($"{actor.Name} opened a door.");
            }
        }

    }
}
