using Godot;
using System;

namespace SpaceShooter.GUI.HUD
{
	public class PointerControl : Control
	{
		Line2D _lineToPointer;
		Sprite _pointer;
		[Export] float _pointerMargin = 0;

        public override void _Ready()
        {
            _lineToPointer = GetNode<Line2D>("LineToPointer");
			_pointer = GetNode<Sprite>("Pointer");
        }

        public override void _Process(float delta)
        {
			_lineToPointer.ClearPoints();
			Vector2 pointerPosition = _pointer.Position;
			if (pointerPosition.Length() > HUDController.Instance.PointerFromCircleCenterMargin)
			{
				_lineToPointer.AddPoint(HUDController.Instance.PointerFromCircleCenterMargin * pointerPosition.Normalized());
				_lineToPointer.AddPoint((pointerPosition.Length() - _pointerMargin) * pointerPosition.Normalized());
			}
        }
	}
}