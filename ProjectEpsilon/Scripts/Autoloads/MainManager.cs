using Godot;
using KayEth.Tools;
using System;

namespace KayEth.Autoloads
{
    public class MainManager : Node
    {
        Logger _logger;

        public MainManager()
        {
            if (!Engine.EditorHint)
            {
                _logger = new Logger();
                _logger.Initialize();
            }
        }

        public override void _Ready()
        {
            
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventKey iek)
            {
                if (iek.Pressed && iek.Scancode == (int)KeyList.F11)
                {
                    OS.WindowFullscreen = !OS.WindowFullscreen;
                }
            }
        }
    }
}
