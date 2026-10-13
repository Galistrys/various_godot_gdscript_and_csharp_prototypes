using Godot;
using HorrorGame.Player;
using System;

namespace HorrorGame
{
    public class TestRTL : RichTextLabel
    {
        [Export] public NodePath _playerNodePath;
        PlayerEntity _playerEntity;

        public override void _Ready()
        {
            _playerEntity = GetNode<PlayerEntity>(_playerNodePath);
        }

        public override void _PhysicsProcess(float delta)
        {
            this.Text = $@"IsOnFloor: {_playerEntity.IsOnFloor()},
Velocity: {_playerEntity.velocity}
            ";
        }
    }
}
