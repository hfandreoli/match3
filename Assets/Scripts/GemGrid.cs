using System;
using UnityEngine;

public class GemGrid : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;

    private Gem[,] _grid;

    private void Start()
    {
        _grid = new Gem[width, height];
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public Gem GetValue(MatrixIndex index)
    {
        return GetValue(index.i, index.j);
    }

    public Gem GetValue(int i, int j)
    {
        return _grid[j, i];
    }

    public void SetValue(MatrixIndex index, Gem gem)
    {
        _grid[index.j, index.i] = gem;
    }

    public void SetValue(int i, int j, Gem gem)
    {
        _grid[j, i] = gem;
    }

    public Vector2 GetWorldPositionFromIndex(MatrixIndex index)
    {
        var myTransform = transform;
        return (Vector2) myTransform.position + new Vector2(index.j, -index.i) * myTransform.lossyScale;
    }

    public MatrixIndex GetIndexFromWorldPosition(Vector2 position)
    {
        var myTransform = transform;
        var gridVector = position - (Vector2) myTransform.position;
        gridVector /= myTransform.lossyScale.x;
        var i = (int) Mathf.Round(gridVector.y) * -1;
        var j = (int) Mathf.Round(gridVector.x);

        return new MatrixIndex(i, j);
    }

    public Vector2 IndexToPosition(int i, int j)
    {
        return new Vector2(j, -i);
    }
}

public struct MatrixIndex : IComparable
{
    public int i;
    public int j;

    public MatrixIndex(int newI, int newJ)
    {
        i = newI;
        j = newJ;
    }
    
    public int CompareTo(object obj)
    {
        var other = (MatrixIndex) obj;
        return i * 100 + j - (other.i * 100 + other.j);
    }
}