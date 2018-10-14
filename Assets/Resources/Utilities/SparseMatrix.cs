using System.Collections.Generic;

public class SparseMatrix<T>
{
    public int Width { get; private set; }
    public int Height { get; private set; }

    public Dictionary<Int2, T> cells = new Dictionary<Int2, T>();

    public SparseMatrix(int width, int height)
    {
        this.Width = width;
        this.Height = height;
    }

    public bool IsCellEmpty(int x, int y)
    {
        Int2 index = new Int2(x, y);
        return cells.ContainsKey(index);
    }

    public void Remove(int x, int y)
    {
        cells.Remove(new Int2(x, y));
    }
    public T this[int x, int y]
    {
        get
        {
            Int2 index = new Int2(x, y);
            T result;
            cells.TryGetValue(index, out result);
            return result;
        }
        set
        {
            Int2 index = new Int2(x, y);
            cells[index] = value;
        }
    }
}