using Godot;
using System;

namespace KayEth.GUI.HUD
{
    public class HUDController : MarginContainer
    {
        public override void _Process(float delta)
        {
            float scale_x = OS.WindowSize.x / 1920.0f;
            float scale_y = OS.WindowSize.y / 1080.0f;
            float scale = Mathf.Min(scale_x, scale_y);

            this.RectScale = new Vector2(scale, scale);
            this.RectSize = OS.WindowSize / scale;
        }
    }
}
