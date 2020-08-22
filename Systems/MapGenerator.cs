using RogueSharp;
using RogueSharp.DiceNotation;
using RogueSharpDemo.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RogueSharpDemo.Systems
{
    class MapGenerator
    {
        private readonly int _width;
        private readonly int _height;
        private readonly int _MaxRooms;
        private readonly int _RoomMinSize;
        private readonly int _RoomMaxSize;

        private readonly DungeonMap _map;

        public MapGenerator(int width, int height, int rooms, int maxRoomSize, int minRoomSize)
        {
            _width = width;
            _height = height;
            _MaxRooms = rooms;
            _RoomMaxSize = maxRoomSize;
            _RoomMinSize = minRoomSize;

            _map = new DungeonMap();
        }

        public DungeonMap GenerateMap()
        {
            _map.Initialize(_width, _height);

            for (int r = _MaxRooms; r > 0; r--)
            {
                int roomWidth = Game.Random.Next(_RoomMinSize, _RoomMaxSize);
                int roomHeight = Game.Random.Next(_RoomMinSize, _RoomMaxSize);

                int roomXPos = Game.Random.Next(0, _width - roomWidth - 1);
                int roomYPos = Game.Random.Next(0, _height - roomHeight - 1);

                var newRoom = new Rectangle(roomXPos, roomYPos, roomWidth, roomHeight);

                bool newRoomIntersects = _map.rooms.Any(room => newRoom.Intersects(room));

                if (!newRoomIntersects)
                {
                    _map.rooms.Add(newRoom);
                }
            }

            foreach(Rectangle room in _map.rooms)
            {
                CreateRoom(room);
            }

            for(int r = 1; r < _map.rooms.Count; r++)
            {
                int PreviousRoomCenterX = _map.rooms[r - 1].Center.X;
                int PreviousRoomCenterY = _map.rooms[r - 1].Center.Y;
                int CurrentRoomCenterX = _map.rooms[r].Center.X;
                int CurrentRoomCenterY = _map.rooms[r].Center.Y;

                if(Game.Random.Next(1,2) == 1)
                {
                    CreateHorizontalTunnel(PreviousRoomCenterX, CurrentRoomCenterX, PreviousRoomCenterY);
                    CreateVerticalTunnel(PreviousRoomCenterY, CurrentRoomCenterY, CurrentRoomCenterX);
                }
                else
                {
                    CreateVerticalTunnel(PreviousRoomCenterY, CurrentRoomCenterY, PreviousRoomCenterX);
                    CreateHorizontalTunnel(PreviousRoomCenterX, CurrentRoomCenterX, CurrentRoomCenterY);
                }
            }

            foreach(Rectangle room in _map.rooms)
            {
                CreateDoors(room);
            }

            PlacePlayer();
            PlaceMonsters();

            return _map;
        }

        private void CreateRoom(Rectangle room)
        {
            for(int x = room.Left +1; x < room.Right; x++)
            {
                for(int y = room.Top +1; y < room.Bottom; y++)
                {
                    _map.SetCellProperties(x, y, true, true, false);
                }
            }
        }

        private void CreateDoors(Rectangle room)
        {
            int minX = room.Left;
            int maxX = room.Right;
            int minY = room.Top;
            int maxY = room.Bottom;

            List<ICell> borderCells = _map.GetCellsAlongLine(minX, minY, maxX, minY).ToList();
            borderCells.AddRange(_map.GetCellsAlongLine(minX, maxY, maxX, maxY));
            borderCells.AddRange(_map.GetCellsAlongLine(minX, minY, minX, maxY));
            borderCells.AddRange(_map.GetCellsAlongLine(maxX, minY, maxX, maxY));

            foreach(ICell cell in borderCells)
            {
                if (IsPotentialDoor(cell))
                {
                    _map.SetCellProperties(cell.X, cell.Y, false, true);
                    _map.doors.Add(new Door
                    {
                        X = cell.X,
                        Y = cell.Y,
                        IsOpen = false
                    });
                }
            }
        }

        private bool IsPotentialDoor(ICell cell)
        {
            if (!cell.IsWalkable)
            {
                return false;
            }

            ICell left = _map.GetCell(cell.X - 1, cell.Y);
            ICell right = _map.GetCell(cell.X + 1, cell.Y);
            ICell top = _map.GetCell(cell.X, cell.Y - 1);
            ICell bottom = _map.GetCell(cell.X, cell.Y + 1);

            if(_map.GetDoor(cell.X, cell.Y) != null ||
                _map.GetDoor(left.X, left.Y) != null ||
                _map.GetDoor(right.X, right.Y) != null ||
                _map.GetDoor(top.X, top.Y) != null ||
                _map.GetDoor(bottom.X, bottom.Y) != null)
            {
                return false;
            }

            if(left.IsWalkable && right.IsWalkable && !top.IsWalkable && !bottom.IsWalkable)
            {
                return true;
            }

            if (top.IsWalkable && bottom.IsWalkable && !left.IsWalkable && !right.IsWalkable)
            {
                return true;
            }

            return false;
        }

        private void CreateHorizontalTunnel(int xStart, int xEnd, int yPos)
        {
            for(int x = Math.Min(xStart, xEnd); x <= Math.Max(xStart, xEnd); x++)
            {
                _map.SetCellProperties(x, yPos, true, true);
            }
        }

        private void CreateVerticalTunnel(int yStart, int yEnd, int xPos)
        {
            for(int y = Math.Min(yStart, yEnd); y <= Math.Max(yStart, yEnd); y++)
            {
                _map.SetCellProperties(xPos, y, true, true);
            }
        }

        private void PlacePlayer()
        {
            Player player = Game._Player;
            if(player == null)
            {
                player = new Player();
            }

            player.X = _map.rooms[0].Center.X;
            player.Y = _map.rooms[0].Center.Y;

            _map.AddPlayer(player);
        }

        private void PlaceMonsters()
        {
            foreach(var room in _map.rooms)
            {
                if(Dice.Roll("1D10") < 7)
                {
                    var numMonsters = Dice.Roll("1D4");
                    for(int i = 0; i < numMonsters; i++)
                    {
                        Point randomPoint = _map.GetRandomWalkableLocationInRoom(room);

                        if (randomPoint != null || randomPoint.Equals(default))
                        {
                            var monster = Kobold.Create(1);
                            monster.X = randomPoint.X;
                            monster.Y = randomPoint.Y;
                            _map.AddMonster(monster);
                        }
                    }
                }
            }
        }
    }
}
