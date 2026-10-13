using Godot;
using System;

public struct Vector3Int
{
    public int x, y, z;

    public Vector3Int(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3Int(float x, float y, float z) : this((int)x, (int)y, (int)z)
    {
        
    }

    public Vector3Int(Vector3 vec) : this(vec.x, vec.y, vec.z)
    {

    }

    public int this[int i]
    {
        get
        {
            if (i == 0)
                return x;
            else if (i == 1)
                return y;
            else
                return z;
        }
        set
        {
            if (i == 0)
                x = value;
            else if (i == 1)
                y = value;
            else
                z = value;
        }
    }

    public static implicit operator Vector3(Vector3Int a)
    {
        return new Vector3(a.x, a.y, a.z);
    }

    public static Vector3Int operator +(Vector3Int a, Vector3Int b)
    {
        return new Vector3Int(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static Vector3Int operator -(Vector3Int a, Vector3Int b)
    {
        return new Vector3Int(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public static Vector3Int operator *(Vector3Int a, int b)
    {
        return new Vector3Int(a.x * b, a.y * b, a.z * b);
    }

    public override string ToString()
    {
        return "Vector3Int ( " + x + ", " + y + ", " + z + ")";
    }
}
