using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class TestVoronoi : Control
{
    SpinBox _sizeSpinBox;
    SpinBox _numberOfPointsSpinBox;
    CheckBox _detectBordersCheckBox;

    SpinBox _gridSizeSpinBox;

    TextureRect _diagramTextureRect;
    TextureRect _bordersTextureRect;

    int Size { get { return (int)Math.Floor(_sizeSpinBox.Value); } }
    int NumberOfPoints { get { return (int)Math.Floor(_numberOfPointsSpinBox.Value); } }
    bool DetectBorders { get { return _detectBordersCheckBox.Pressed; } }

    int GridSize { get { return (int)Math.Floor(_gridSizeSpinBox.Value); } }

    Vector2[] _points = null;
    Color[] _colors = null;

    public override void _Ready()
    {
        _sizeSpinBox = GetNode<SpinBox>("HBoxContainer/PanelContainer/VBoxContainer/SizeSpinBox");
        _numberOfPointsSpinBox = GetNode<SpinBox>("HBoxContainer/PanelContainer/VBoxContainer/PointsSpinBox");
        _detectBordersCheckBox = GetNode<CheckBox>("HBoxContainer/PanelContainer/VBoxContainer/DetectBordersCheckBox");

        _gridSizeSpinBox = GetNode<SpinBox>("HBoxContainer/PanelContainer/VBoxContainer/GridSizeSpinBox");

        _diagramTextureRect = GetNode<TextureRect>("HBoxContainer/ScrollContainer/PanelContainer/Diagram");
        _bordersTextureRect = GetNode<TextureRect>("HBoxContainer/ScrollContainer/PanelContainer/Borders");
    }

    public void OnGenerateDiagramPressed()
    {
        GD.Randomize();

        _diagramTextureRect.Texture = null;
        _bordersTextureRect.Texture = null;

        int size = Size;
        int numberOfPoints = NumberOfPoints;
        bool detectBorders = DetectBorders;
        int gridSize = GridSize + 4;
        int cellSize = size / gridSize;

        if (gridSize == 0)
        {
            _colors = new Color[numberOfPoints];
            for (int i = 0; i < numberOfPoints; i++)
            {
                _colors[i] = Colors.Black;
            }
            _points = new Vector2[numberOfPoints];
            for (int i = 0; i < numberOfPoints; i++)
            {
                _points[i] = new Vector2(GD.Randi() % size, GD.Randi() % size);
                var color = Colors.Black;
                do
                {
                    color = new Color(GD.Randf(), GD.Randf(), GD.Randf());
                } while (color == Colors.Aqua || _colors.Contains(color));
                _colors[i] = color;
            }
        }
        else
        {
            _colors = new Color[gridSize * gridSize];
            for (int i = 0; i < _colors.Length; i++)
            {
                _colors[i] = Colors.Black;
            }
            for (int i = 0; i < _colors.Length; i++)
            {
                var color = Colors.Black;
                do
                {
                    color = new Color(GD.Randf(), GD.Randf(), GD.Randf());
                } while (color == Colors.Aqua || _colors.Contains(color));
                _colors[i] = color;
            }
            
            _points = new Vector2[gridSize * gridSize];

            int cellX = 0;
            while (cellX < gridSize)
            {
                int cellY = 0;
                while (cellY < gridSize)
                {
                    int currentIdx = cellX * gridSize + cellY;

                    _points[currentIdx] = new Vector2(GD.Randi() % cellSize + cellX * cellSize, GD.Randi() % cellSize + cellY * cellSize);

                    cellY++;
                }
                cellX++;
            }
        }

        var diagramImage = new Image();
        diagramImage.Create(size, size, false, Image.Format.Rgba8);

        diagramImage.Lock();
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                // int distanceToLeftBorder = x;
                // int distanceToRightBorder = size - 1 - x;
                // int distanceToTopBorder = y;
                // int distanceToBottomBorder = size - 1 - y;

                float minDistance = Mathf.Inf;
                int bestIdx = -1;

                for (int i = 0; i < _points.Length; i++)
                {
                    float curDistance = _points[i].DistanceTo(new Vector2(x, y));
                    if (curDistance < minDistance)
                    {
                        minDistance = curDistance;
                        bestIdx = i;
                    }
                }

                // Color targetColor = Colors.Aqua;
                // if (minDistance < distanceToLeftBorder
                //     && minDistance < distanceToRightBorder
                //     && minDistance < distanceToTopBorder
                //     && minDistance < distanceToBottomBorder)
                // {
                //     targetColor = _colors[bestIdx];
                // }

                // diagramImage.SetPixel(x, y, targetColor);
                var point = _points[bestIdx];
                if (point.x < cellSize * 2 || point.y < cellSize * 2 || point.x > size - cellSize * 2 || point.y > size - cellSize * 2)
                    diagramImage.SetPixel(x, y, Colors.Aqua);
                else
                    diagramImage.SetPixel(x, y, _colors[bestIdx]);
            }
        }
        diagramImage.Unlock();

        if (detectBorders)
        {
            var borderImage = new Image();
            borderImage.Create(size, size, false, Image.Format.Rgba8);

            diagramImage.Lock();
            borderImage.Lock();
            for (int x = 0; x < size - 1; x++)
            {
                for (int y = 0; y < size - 1; y++)
                {
                    var curColor = diagramImage.GetPixel(x, y);
                    if (curColor != diagramImage.GetPixel(x + 1, y)
                        || curColor != diagramImage.GetPixel(x, y + 1)
                        || curColor != diagramImage.GetPixel(x + 1, y + 1))
                    {
                        borderImage.SetPixel(x, y, Colors.Black);
                        borderImage.SetPixel(x + 1, y, Colors.Black);
                        borderImage.SetPixel(x, y + 1, Colors.Black);
                        borderImage.SetPixel(x + 1, y + 1, Colors.Black);
                    }
                }
            }
            borderImage.Unlock();
            diagramImage.Unlock();

            var bordersTexture = new ImageTexture();
            bordersTexture.CreateFromImage(borderImage, 0);
            _bordersTextureRect.Texture = bordersTexture;
        }

        diagramImage.Lock();
        for (int i = 0; i < _points.Length; i++)
        {
            int x = Mathf.FloorToInt(_points[i].x);
            int y = Mathf.FloorToInt(_points[i].y);

            diagramImage.SetPixel(x, y, Colors.White);
            if (x < size - 1)
                diagramImage.SetPixel(x + 1, y, Colors.White);
            if (y < size - 1)
                diagramImage.SetPixel(x, y + 1, Colors.White);
            if (x > 0)
                diagramImage.SetPixel(x - 1, y, Colors.White);
            if (y > 0)
                diagramImage.SetPixel(x, y - 1, Colors.White);
        }
        diagramImage.Unlock();

        var diagramTexture = new ImageTexture();
        diagramTexture.CreateFromImage(diagramImage, 0);
        _diagramTextureRect.Texture = diagramTexture;
    }
}
