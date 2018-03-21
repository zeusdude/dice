//
// https://gist.github.com/ByronMayne/a22d41179a610aa4a1a06779c0434858
using UnityEngine;
using System;

[Serializable]
public struct IntPoint
{
    [SerializeField]
    public int x;
    [SerializeField]
    public int y;

    public IntPoint(int x, int y)
    {
        this.x = x;
        this.y = y; 
    }

    /// <summary>
    /// Gets the value at an index.
    /// </summary>
    /// <param name="index">The index you are trying to get.</param>
    /// <returns>The value at that index.</returns>
    public int this[int index]
    {
        get
        {
            int result;
            if(index != 0)
            {
                if(index != 1)
                {
                    throw new IndexOutOfRangeException("Index " + index.ToString() + " is out of range." );
                }
                result = y;
            }
            else
            {
                result = x;
            }
            return result;
        }
        set
        {
            if (index != 0)
            {
                if (index != 1)
                {
                    throw new IndexOutOfRangeException("Index " + index.ToString() + " is out of range.");
                }
                y = value;
            }
            else
            {
                x = value;
            }
        }
    }

    /// <summary>
    /// Sets the x and y components of an existing Point
    /// </summary>
    /// <param name="x">The new x value</param>
    /// <param name="y">The new y value</param>
    public void Set(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    /// <summary>
    /// Shorthand for writing new Point(0,0).
    /// </summary>
    public static IntPoint zero
    {
        get
        {
            return new IntPoint(0, 0);
        }
    }

    /// <summary>
    /// Shorthand for writing new Point(1,1).
    /// </summary>
    public static IntPoint one
    {
        get
        {
            return new IntPoint(1, 1);
        }
    }

    public static explicit operator Vector2(IntPoint point)
    {
        return new Vector2((float)point.x, (float)point.y);
    }

    public static explicit operator IntPoint(Vector2 vector2)
    {
        return new IntPoint((int)vector2.x, (int)vector2.y);
    }

    public static IntPoint operator +(IntPoint lhs, IntPoint rhs)
    {
        lhs.x += rhs.x;
        lhs.y += rhs.y;
        return lhs;
    }

    public static IntPoint operator -(IntPoint lhs, IntPoint rhs)
    {
        lhs.x -= rhs.x;
        lhs.y -= rhs.y;
        return lhs;
    }

    public static IntPoint operator *(IntPoint lhs, IntPoint rhs)
    {
        lhs.x *= rhs.x;
        lhs.y *= rhs.y;
        return lhs;
    }

    public static IntPoint operator /(IntPoint lhs, IntPoint rhs)
    {
        lhs.x /= rhs.x;
        lhs.y /= rhs.y;
        return lhs;
    }

    public static bool operator ==(IntPoint lhs, IntPoint rhs)
    {
       return lhs.x == rhs.x && lhs.y == rhs.x;
    }

    public static bool operator !=(IntPoint lhs, IntPoint rhs)
    {
        return lhs.x != rhs.x || lhs.y != rhs.x;
    }

    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            int hash = (int) 2166136261;
            hash = (hash * 16777619) ^ x;
            hash = (hash * 16777619) ^ y;
            return hash;
        }
    }

    public override bool Equals(object other)
    {
        if(!(other is IntPoint))
        {
            return false;
        }

        IntPoint point = (IntPoint)other;
        return x == point.x && y == point.y;
    }

    public override string ToString()
    {
        return string.Join(", ", new string[] { x.ToString(), y.ToString() });
    }
}