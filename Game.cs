using RLNET;
using RogueSharp.Random;
using RogueSharpDemo.Core;
using RogueSharpDemo.Systems;
using System;

namespace RogueSharpDemo
{
    class Game
    {

        // MAIN CONSOLE
        private static readonly int _ScreenWidth = 100;
        private static readonly int _ScreenHeight = 70;
        private static RLRootConsole _rootConsole;

        // MAP CONSOLE
        private static readonly int _MapWidth = 80;
        private static readonly int _MapHeight = 48;
        private static RLConsole _MapConsole;

        // MESSAGE CONSOLE
        private static readonly int _MsgWidth = 80;
        private static readonly int _MsgHeight = 11;
        private static RLConsole _MessageConsole;

        // STAT CONSOLE
        private static readonly int _StatWidth = 20;
        private static readonly int _StatHeight = 70;
        private static RLConsole _StatConsole;

        // INVENTORY CONSOLE
        private static readonly int _InvWidth = 80;
        private static readonly int _InvHeight = 11;
        private static RLConsole _InventoryConsole;

        public static DungeonMap _Map { get; private set; }
        public static Player _Player { get; set; }

        private static bool _RenderRequired = true;

        public static CommandSystem _CommandSystem { get; private set; }
        
        public static IRandom Random { get; private set; }

        public static MessageLog _MessageLog { get; private set; }

        public static SchedulingSystem _Scheduler { get; private set; }

        static void Main(string[] args)
        {
            // SETUP

            int seed = (int)DateTime.UtcNow.Ticks;
            Random = new DotNetRandom(seed);

            string FontFileName = "terminal8x8.png";
            string ConsoleTitle = $"RogueSharp Tutorial - Level 1 - Seed: {seed}";


            // Set up each frame in the console
            _rootConsole = new RLRootConsole(FontFileName, _ScreenWidth, _ScreenHeight, 8, 8, 1f, ConsoleTitle);
            _MapConsole = new RLConsole(_MapWidth, _MapHeight);
            _MessageConsole = new RLConsole(_MsgWidth, _MsgHeight);
            _StatConsole = new RLConsole(_StatWidth, _StatHeight);
            _InventoryConsole = new RLConsole(_InvWidth, _InvHeight);

            // Set Console background colors
            _MapConsole.SetBackColor(0, 0, _MapWidth, _MapHeight, Colors.FloorBackground);
            _MapConsole.Print(1, 1, "Map", Colors.TextHeading);
            
            _InventoryConsole.SetBackColor(0, 0, _InvWidth, _InvHeight, Palette.DbWood);
            _InventoryConsole.Print(1, 1, "Inventory", Colors.TextHeading);


            // Set up Messages Console
            _MessageLog = new MessageLog();
            _MessageLog.Add("The Rogue arrives on level 1");
            _MessageLog.Add($"Level created with seed '{seed}'");


            // Set Up Scheduling System
            _Scheduler = new SchedulingSystem();

            // Set Up Command System
            _CommandSystem = new CommandSystem();

            // Create Actors & Map
            //_Player = new Player();
            MapGenerator MapGen = new MapGenerator(_MapWidth, _MapHeight, 20, 13, 7);
            _Map = MapGen.GenerateMap();
            _Map.UpdatePlayerFieldOfView();


            // UPDATE, RENDER, AND RUN
            _rootConsole.Update += OnRootConsoleUpdate;
            _rootConsole.Render += OnRootConsoleRender;

            _rootConsole.Run();
        }

        private static void OnRootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            bool DidPlayerAct = false;
            RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();

            if (_CommandSystem.IsPlayerTurn)
            {
                if (keyPress != null)
                {
                    if (keyPress.Key == RLKey.Up)
                    {
                        DidPlayerAct = _CommandSystem.MovePlayer(Direction.Up);
                    }
                    else if (keyPress.Key == RLKey.Down)
                    {
                        DidPlayerAct = _CommandSystem.MovePlayer(Direction.Down);
                    }
                    else if (keyPress.Key == RLKey.Left)
                    {
                        DidPlayerAct = _CommandSystem.MovePlayer(Direction.Left);
                    }
                    else if (keyPress.Key == RLKey.Right)
                    {
                        DidPlayerAct = _CommandSystem.MovePlayer(Direction.Right);
                    }
                    else if (keyPress.Key == RLKey.Escape)
                    {
                        _rootConsole.Close();
                    }
                }

                if (DidPlayerAct)
                {
                    _RenderRequired = true;
                    _CommandSystem.EndPlayerTurn();
                }
            }
            else
            {
                _CommandSystem.ActivateMonsters();
                _RenderRequired = true;
            }
        }

        private static void OnRootConsoleRender(object sender, UpdateEventArgs e)
        {
            if (_RenderRequired)
            {
                _MapConsole.Clear();
                _StatConsole.Clear();
                _MessageConsole.Clear();

                _Map.Draw(_MapConsole, _StatConsole);
                _Player.Draw(_MapConsole, _Map);
                _Player.DrawStats(_StatConsole);
                _MessageLog.Draw(_MessageConsole);

                RLConsole.Blit(_MapConsole, 0, 0, _MapWidth, _MapHeight, _rootConsole, 0, _InvHeight);
                RLConsole.Blit(_StatConsole, 0, 0, _StatWidth, _StatHeight, _rootConsole, _MapWidth, 0);
                RLConsole.Blit(_MessageConsole, 0, 0, _MapWidth, _MapHeight, _rootConsole, 0, _ScreenHeight - _MsgHeight);
                RLConsole.Blit(_InventoryConsole, 0, 0, _InvWidth, _InvHeight, _rootConsole, 0, 0);

                _rootConsole.Draw();

                _RenderRequired = false;
            }
        }
    }
}
